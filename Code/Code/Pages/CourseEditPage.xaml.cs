using System;
using System.Collections.Generic;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace test
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseEditPage : ContentPage
    {
        private int CourseID;
        private int TermID;
        private int InstructorID;
        public CourseEditPage(int ID)
        {
            InitializeComponent();
            CourseID = ID;
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
                List<string> StatusList = new List<string>();
                StatusList.Add("In progress");
                StatusList.Add("Completed");
                StatusList.Add("Dropped");
                StatusList.Add("Plan to take");
                courseStatus.ItemsSource = StatusList;

                conn.CreateTable<Courses>();
                var assess = conn.Query<Courses>("select * from Courses where ID=?", CourseID);
                courseName.Text = assess[0].Name;
                courseStart.Date = assess[0].Start;
                notiStart.IsChecked = assess[0].AlertsStart;
                courseEnd.Date = assess[0].End;
                notiEnd.IsChecked = assess[0].AlertsEnd;
                courseStatus.SelectedIndex = (int)assess[0].Status;
                courseNotes.Text = assess[0].Notes;
                TermID = assess[0].TermId;
                InstructorID = assess[0].InstructorID;
            }
        }

        private void SaveCourseButton_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (courseStart.Date > courseEnd.Date)
                {
                    await DisplayAlert("Error:", "The Start Date must be before the End Date", "Ok");
                    return;
                }
                else if (String.IsNullOrEmpty(courseName.Text))
                {
                    await DisplayAlert("Error:", "The term title can not be blank.", "Ok");
                    return;
                }
                else
                {
                    var result = await this.DisplayAlert("Save:", "Do you really want to save these changes?", "Yes", "No");

                    if (result)
                    {
                        using (SQLite.SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                        {
                            conn.CreateTable<Courses>();
                            Courses course = new Courses
                            {
                                ID = CourseID,
                                Name = courseName.Text,
                                Start = courseStart.Date,
                                AlertsStart = notiStart.IsChecked,
                                End = courseEnd.Date,
                                AlertsEnd = notiEnd.IsChecked,
                                Status = (StatusType)courseStatus.SelectedIndex,
                                Notes = courseNotes.Text,
                                TermId = TermID,
                                InstructorID = InstructorID
                            };
                            conn.Update(course);
                            App.Current.MainPage = new NavigationPage(new CourseList(TermID));
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            });
        }

        private async void ShareNotesButton_Clicked(object sender, EventArgs e)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = courseNotes.Text,
                Title = "Share Text"
            });
        }
        //
    }
}