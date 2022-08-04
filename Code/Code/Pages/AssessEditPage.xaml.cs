using System;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace test
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssessEditPage : ContentPage
    {
        private int CourseID;
        private int AssessmentID;
        private int TermID;
        private AsType type;
        public AssessEditPage(int cID, int ID, int tID)
        {
            InitializeComponent();
            CourseID = cID;
            AssessmentID = ID;
            TermID = tID;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            using (SQLite.SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Assessments>();
                var assess = conn.Query<Assessments>("select * from Assessments where ID=?", AssessmentID);
                type = assess[0].Type;
                assessName.Text = assess[0].Name;
                assessStart.Date = assess[0].Start;
                notiStart.IsChecked = assess[0].AlertsStart;
                assessEnd.Date = assess[0].End;
                notiEnd.IsChecked = assess[0].AlertsEnd;
            }
        }

        private void ToolbarBack_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new AssessPage(CourseID,TermID));
        }

        private void SaveAssessmentButton_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {

                if (assessStart.Date > assessEnd.Date)
                {
                    await DisplayAlert("Error:", "The Start Date must be before the End Date", "Ok");
                    return;
                }
                else if (String.IsNullOrEmpty(assessName.Text))
                {
                    await DisplayAlert("Error:", "An assessment needs a name.", "Ok");
                    return;
                }
                else
                {
                    var result = await this.DisplayAlert("Save:", "Do you really want to save these changes?", "Yes", "No");

                    if (result)
                    {
                        using (SQLite.SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                        {
                            conn.CreateTable<Assessments>();
                            Assessments assess = new Assessments
                            {
                                ID = AssessmentID,
                                Type = type,
                                Name = assessName.Text,
                                Start = assessStart.Date,
                                AlertsStart = notiStart.IsChecked,
                                End = assessEnd.Date,
                                AlertsEnd = notiEnd.IsChecked,
                                CourseID = CourseID
                            };
                            conn.Update(assess);
                            App.Current.MainPage = new NavigationPage(new AssessPage(CourseID, TermID));
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            });
        }

        //  ----------------------
    }
}