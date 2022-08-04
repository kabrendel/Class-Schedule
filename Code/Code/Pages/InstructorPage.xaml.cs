using System;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace test
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InstructorPage : ContentPage
    {
        private int CourseID;
        private int TermID;
        private int InstructorID;
        public InstructorPage(int cID, int tID, int iID)
        {
            InitializeComponent();
            CourseID = cID;
            TermID = tID;
            InstructorID = iID;
        }

        private void ToolbarBack_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new CourseList(TermID));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            using (SQLite.SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Instructors>();
                var courses = conn.Query<Instructors>("select * from Instructors where ID=?", InstructorID);
                instructorListView.ItemsSource = courses;
            }
        }

        private void DeleteInsButton_Clicked(object sender, EventArgs e)
        {
            var item = (Instructors)instructorListView.SelectedItem;
            if (item is null)
            {
                DisplayAlert("Instructor:", "Select an instructor to delete.", "Ok");
                return;
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var result = await this.DisplayAlert("Confirm Delete?", "Do you really want to delete this Instructor?", "Yes", "No");

                    if (result)
                    {
                        using (SQLite.SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                        {
                            conn.CreateTable<Instructors>();
                            var instructors = conn.Query<Instructors>("delete from Instructors where ID=?", item.ID);
                            
                            conn.CreateTable<Courses>();
                            var course = conn.Query<Courses>("select * from Courses where ID=?", CourseID);
                            course[0].InstructorID = -1;
                            InstructorID = course[0].InstructorID;
                            
                            instructors = conn.Query<Instructors>("select * from Instructors where ID=?", InstructorID);
                            instructorListView.ItemsSource = instructors;
                            //  SelectedItem doesn't clear..
                            instructorListView.SelectedItem = null;
                        }
                    }
                    else
                    {
                        return;
                    }
                });
            }
        }

        private void EditInsButton_Clicked(object sender, EventArgs e)
        {
            var item = (Instructors)instructorListView.SelectedItem;
            if (item is null)
            {
                DisplayAlert("Instructor:", "Select an instructor to edit.", "Ok");
                return;
            }
            else
            {
                App.Current.MainPage = new NavigationPage(new InstructorEditPage(TermID, item.ID, CourseID));
            }
        }

        private void AddInsButton_Clicked(object sender, EventArgs e)
        {
            using (SQLite.SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Instructors>();
                var instructors = conn.Query<Instructors>("select * from Instructors where ID=?", InstructorID);
                if (instructors.Count == 0)
                {
                    //  No instructor for course, add instructor
                    //  Must add the new instructor and update the course
                    Instructors instructor = new Instructors
                    {
                        Name = "Substitute Teacher",
                        Email = "hassome@email.edu",
                        Phone = "8675309"
                    };
                    conn.Insert(instructor);
                    InstructorID = instructor.ID;
                    conn.CreateTable<Courses>();
                    var course = conn.Query<Courses>("select * from Courses where ID=?", CourseID);
                    course[0].InstructorID = instructor.ID;
                    conn.Update(course[0]);
                    // Update view                    
                    instructors = conn.Query<Instructors>("select * from Instructors where ID=?", instructor.ID);
                    instructorListView.ItemsSource = instructors;
                }
                else
                {
                    //  Course has instructor, do nothing
                    DisplayAlert("Instructors:", "This course has an instructor, none added.", "Ok");
                    return;
                }
            }
        }
    }
}