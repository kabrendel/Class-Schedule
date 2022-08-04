using System;
using Xamarin.Forms;
using SQLite;

namespace test
{
    public partial class MainPage : ContentPage
    {
        //
        public MainPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            using (SQLite.SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Terms>();
                var terms = conn.Table<Terms>().ToList();
                termsListView.ItemsSource = terms;
            }
        }

        private void CoursesButton_Clicked(object sender, EventArgs e)
        {
            var item = (Terms)termsListView.SelectedItem;
            if (item is null)
            {
                DisplayAlert("Term:", "Select a term to view courses.", "Ok");
                return;
            }
            else
            {
                App.Current.MainPage = new NavigationPage(new CourseList(item.ID));
            }
        }

        private void DeleteTermButton_Clicked(object sender, EventArgs e)
        {
            var item = (Terms)termsListView.SelectedItem;
            if (item is null)
            {
                DisplayAlert("Term:", "Select a term to delete.", "Ok");
                return;
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var result = await this.DisplayAlert("Confirm Delete?", "Do you really want to delete this term?", "Yes", "No");

                    if (result)
                    {
                        using (SQLite.SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                        {
                            conn.CreateTable<Terms>();
                            var term = conn.Query<Terms>("delete from Terms where ID=?", item.ID);
                            term = conn.Table<Terms>().ToList();
                            termsListView.ItemsSource = term;
                            termsListView.SelectedItem = null;
                        }
                    }
                    else
                    {
                        return;
                    }
                });
            }
        }

        private void AddTermButton_Clicked(object sender, EventArgs e)
        {
            using (SQLite.SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Terms>();
                Terms term = new Terms
                {
                    Title = "New Term",
                    Start = DateTime.Now.AddDays(7),
                    AlertsStart = false,
                    End = DateTime.Now.AddDays(14),
                    AlertsEnd = false
                };
                conn.Insert(term);
                termsListView.ItemsSource = conn.Query<Terms>("select * from Terms");
            }
        }

        private void EditTermButton_Clicked(object sender, EventArgs e)
        {
            var item = (Terms)termsListView.SelectedItem;
            if (item is null)
            {
                DisplayAlert("Term:", "Select a term to edit.", "Ok");
                return;
            }
            else
            {
                App.Current.MainPage = new NavigationPage(new TermEditPage(item.ID));
            }
        }
    }
}
