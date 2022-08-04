using System;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace test
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssessPage : ContentPage
    {
        private int CourseID;
        private int TermID;
        public AssessPage(int ID, int tID)
        {
            InitializeComponent();
            CourseID = ID;
            TermID = tID;
        }
        //
        private void ToolbarBack_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new CourseList(TermID));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            using (SQLite.SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Assessments>();
                var assess = conn.Query<Assessments>("select * from Assessments where CourseID=?", CourseID);
                assessListView.ItemsSource = assess;
            }
        }
        //
        private void EditAssessmentButton_Clicked(object sender, EventArgs e)
        {
            var item = (Assessments)assessListView.SelectedItem;
            if (item is null)
            {
                DisplayAlert("Assessments:", "Select an assessment to edit.", "Ok");
                return;
            }
            else
            {
                App.Current.MainPage = new NavigationPage(new AssessEditPage(CourseID, item.ID, TermID));
            }
        }

        private void DeleteAssessmentButton_Clicked(object sender, EventArgs e)
        {
            var item = (Assessments)assessListView.SelectedItem;
            if (item is null)
            {
                DisplayAlert("Assessments:", "Select an assessment to delete.", "Ok");
                return;
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                var result = await this.DisplayAlert("Confirm Delete?", "Do you really want to delete this Assessment?", "Yes", "No");

                    if (result)
                    {
                        using (SQLite.SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                        {
                            conn.CreateTable<Assessments>();
                            var assess = conn.Query<Assessments>("delete from Assessments where ID=?", item.ID);
                            assess = conn.Query<Assessments>("select * from Assessments where CourseID=?", CourseID);
                            assessListView.ItemsSource = assess;
                        }
                    }
                    else
                    {
                        return;
                    }
                });
            }
        }

        private void AddAsssessmentButton_Clicked(object sender, EventArgs e)
        {
            bool isPerformance = false;
            bool isObjective = false;

            using (SQLite.SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Assessments>();
                var assess = conn.Query<Assessments>("select * from Assessments where CourseID=?", CourseID);
                foreach (Assessments i in assess)
                {
                    if (i.Type == AsType.Objective)
                    {
                        isObjective = true;
                    }
                    else if (i.Type == AsType.Performance)
                    {
                        isPerformance = true;
                    }
                }
                //  Add/replace missing assessments.
                if (isPerformance && isObjective)
                {
                    //  Course has 2 assessments, do nothing
                    DisplayAlert("Assessments:", "This course has 2 assessments, none added.", "Ok");
                    return;
                }
                if (!isObjective)
                {
                    //  create Objective Assessment
                    Assessments objective = new Assessments
                    {
                        Name = "Objective Assessment",
                        Type = AsType.Objective,
                        Start = DateTime.Now,
                        AlertsStart = false,
                        End = DateTime.Now,
                        AlertsEnd = false,
                        CourseID = CourseID
                    };
                    conn.Insert(objective);
                }
                if (!isPerformance)
                {
                    //  create Performance Assessment
                    Assessments performance = new Assessments
                    {
                        Name = "Performance Assessment",
                        Type = AsType.Performance,
                        Start = DateTime.Now,
                        AlertsStart = false,
                        End = DateTime.Now,
                        AlertsEnd = false,
                        CourseID = CourseID
                    };
                    conn.Insert(performance);
                }
                assess = conn.Query<Assessments>("select * from Assessments where CourseID=?", CourseID);
                assessListView.ItemsSource = assess;
                //  ends sqlite connection
            }
        }
        //  ----------------
    }
}