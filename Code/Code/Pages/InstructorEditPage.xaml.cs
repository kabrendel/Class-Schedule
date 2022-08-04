using System;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace test
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InstructorEditPage : ContentPage
    {
        private int InstructorID;
        private int TermID;
        private int CourseID;

        public InstructorEditPage(int tID, int ID, int cID)
        {
            InitializeComponent();
            TermID = tID;
            InstructorID = ID;
            CourseID = cID;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            using (SQLite.SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Instructors>();
                var assess = conn.Query<Instructors>("select * from Instructors where ID=?", InstructorID);
                instructorName.Text = assess[0].Name;
                instructorEmail.Text = assess[0].Email;
                instructorPhone.Text = assess[0].Phone;
            }
        }

        private void ToolbarBack_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new InstructorPage(CourseID, TermID, InstructorID));
        }

        private void SaveInstructorButton_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (String.IsNullOrEmpty(instructorName.Text))
                {
                    await DisplayAlert("Error:", "The instructor has a name?", "Ok");
                    return;
                }
                else if (!Android.Util.Patterns.EmailAddress.Matcher(instructorEmail.Text).Matches())
                {
                    await DisplayAlert("Error:", "Check the email address.", "Ok");
                    return;
                }
                else
                {
                    var result = await this.DisplayAlert("Save:", "Do you really want to save these changes?", "Yes", "No");

                    if (result)
                    {
                        using (SQLite.SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                        {
                            conn.CreateTable<Instructors>();
                            Instructors instructor = new Instructors
                            {
                                ID = InstructorID,
                                Name = instructorName.Text,
                                Email = instructorEmail.Text,
                                Phone = instructorPhone.Text
                            };
                            conn.Update(instructor);
                            App.Current.MainPage = new NavigationPage(new InstructorPage(CourseID, TermID, InstructorID));
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            });
        }
    }
}