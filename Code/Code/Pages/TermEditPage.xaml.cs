using System;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace test
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermEditPage : ContentPage
    {
        private int TermID;
        public TermEditPage(int ID)
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
                conn.CreateTable<Terms>();
                var term = conn.Query<Terms>("select * from Terms where ID=?", TermID);
                termTitle.Text = term[0].Title;
                termStart.Date = term[0].Start;
                notiStart.IsChecked = term[0].AlertsStart;
                termEnd.Date = term[0].End;
                notiEnd.IsChecked = term[0].AlertsEnd;
            }
        }


        private void SaveTermButton_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (termStart.Date > termEnd.Date)
                {
                    await DisplayAlert("Error:", "The Start Date must be before the End Date", "Ok");
                    return;
                }
                else if (String.IsNullOrEmpty(termTitle.Text))
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
                            conn.CreateTable<Terms>();
                            Terms term = new Terms
                            {
                                ID = TermID,
                                Title = termTitle.Text,
                                Start = termStart.Date,
                                AlertsStart = notiStart.IsChecked,
                                End = termEnd.Date,
                                AlertsEnd = notiEnd.IsChecked
                            };
                            conn.Update(term);
                            App.Current.MainPage = new MainPage();
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            });
        }
        //-----------------------
    }
}