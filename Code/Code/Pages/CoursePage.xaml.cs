using SQLite;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace test
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseList : ContentPage
    {
        private int TermID;
        public CourseList(int ID)
        {
            InitializeComponent();
            TermID = ID;
        }

        private void ToolbarBack_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new MainPage();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            using (SQLite.SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Courses>();
                var courses = conn.Query<Courses>("select * from Courses where TermID=?", TermID);
                coursesListView.ItemsSource = courses;
            }
        }

        private void AssessButton_Clicked(object sender, EventArgs e)
        {
            var item = (Courses)coursesListView.SelectedItem;
            if (item is null)
            {
                DisplayAlert("Courses:", "Select a course to view assessments.", "Ok");
                return;
            }
            else
            {
                App.Current.MainPage = new NavigationPage(new AssessPage(item.ID, item.TermId));
            }
        }

        private void InstructorsButton_Clicked(object sender, EventArgs e)
        {
            var item = (Courses)coursesListView.SelectedItem;
            if (item is null)
            {
                DisplayAlert("Courses:", "Select a course to view instructor.", "Ok");
                return;
            }
            else
            {
                App.Current.MainPage = new NavigationPage(new InstructorPage(item.ID, item.TermId, item.InstructorID));
            }
        }

        private void DeleteCourseButton_Clicked(object sender, EventArgs e)
        {
            var item = (Courses)coursesListView.SelectedItem;
            if (item is null)
            {
                DisplayAlert("Courses:", "Select a course to delete.", "Ok");
                return;
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var result = await this.DisplayAlert("Confirm Delete?", "Do you really want to delete this Course?", "Yes", "No");

                    if (result)
                    {
                        using (SQLite.SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                        {
                            conn.CreateTable<Courses>();
                            var assess = conn.Query<Courses>("delete from Courses where ID=?", item.ID);
                            assess = conn.Query<Courses>("select * from Courses where TermID=?", TermID);
                            coursesListView.ItemsSource = assess;
                            coursesListView.SelectedItem = null;
                        }
                    }
                    else
                    {
                        return;
                    }
                });
            }
        }

        private void AddCourseButton_Clicked(object sender, EventArgs e)
        {
            using (SQLite.SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Courses>();
                var courseList = conn.Query<Courses>("select * from Courses where TermID=?", TermID); ;
                if (courseList.Count == 6)
                {
                    DisplayAlert("Courses:", "Max of 6 courses per term, no courses added.", "Ok");
                    return;
                }
                else
                {
                    Courses course = new Courses
                    {
                        Name = "New Course",
                        Start = DateTime.Now,
                        AlertsStart = false,
                        End = DateTime.Now,
                        AlertsEnd = false,
                        Status = StatusType.InProgress,
                        InstructorID = -1,
                        Notes = "Notes",
                        TermId = TermID
                    };
                    conn.Insert(course);
                    coursesListView.ItemsSource = conn.Query<Courses>("select * from Courses where TermID=?", TermID);
                }
            }
        }

        private void EditCourseButton_Clicked(object sender, EventArgs e)
        {
            var item = (Courses)coursesListView.SelectedItem;
            if (item is null)
            {
                DisplayAlert("Courses:", "Select a course to edit.", "Ok");
                return;
            }
            else
            {
                App.Current.MainPage = new NavigationPage(new CourseEditPage(item.ID));
            }
        }
    }
}