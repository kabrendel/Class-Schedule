using System;
using Xamarin.Forms;
using SQLite;
using System.IO;
using Plugin.LocalNotifications;

namespace test
{
    public partial class App : Application
    {
        public static string FilePath;
        
        public App(string filePath)
        {
            //  Singleton functions
            //  
            CreateDB(filePath);
            //
            InitializeComponent();
            FilePath = filePath;
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            int notificationID = 0;
            using (SQLite.SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                //  Terms
                conn.CreateTable<Terms>();
                var terms = conn.Table<Terms>().ToList();
                foreach (Terms t in terms)
                {
                    if (t.AlertsStart && t.Start == DateTime.Today)
                    {
                        CrossLocalNotifications.Current.Show("Term", t.Title + " starts on " + DateTime.Today.ToString("MMMM dd, yyyy"), notificationID++ , DateTime.Now);
                    }
                    if (t.AlertsEnd && t.End == DateTime.Today)
                    {
                        CrossLocalNotifications.Current.Show("Term", t.Title + " ends on " + DateTime.Today.ToString("MMMM dd, yyyy"), notificationID++, DateTime.Now);
                    }
                }
                //  Courses
                conn.CreateTable<Courses>();
                var courses = conn.Table<Courses>().ToList();
                foreach (Courses c in courses)
                {
                    if (c.AlertsStart && c.Start == DateTime.Today)
                    {
                        CrossLocalNotifications.Current.Show("Courses", c.Name + " starts on " + DateTime.Today.ToString("MMMM dd, yyyy"), notificationID++, DateTime.Now);
                    }
                    if (c.AlertsEnd && c.End == DateTime.Today)
                    {
                        CrossLocalNotifications.Current.Show("Courses", c.Name + " ends on " + DateTime.Today.ToString("MMMM dd, yyyy"), notificationID++, DateTime.Now);
                    }
                }
                //  Assessments
                conn.CreateTable<Assessments>();
                var assessments = conn.Table<Assessments>().ToList();
                foreach (Assessments a in assessments)
                {
                    if (a.AlertsStart && a.Start == DateTime.Today)
                    {
                        
                        CrossLocalNotifications.Current.Show(courses.Find(x => x.ID == a.CourseID).Name + " Assessment", a.Name + " starts on " + DateTime.Today.ToString("MMMM dd, yyyy"), notificationID++, DateTime.Now);
                    }
                    if (a.AlertsEnd && a.End == DateTime.Today)
                    {
                        CrossLocalNotifications.Current.Show(courses.Find(x => x.ID == a.CourseID).Name + " Assessment", a.Name + " ends on " + DateTime.Today.ToString("MMMM dd, yyyy"), notificationID++, DateTime.Now);
                    }
                }

            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private void CreateDB(string FilePath)
        {
            //  Create database file if it doesn't exist and insert initial data.
            if (!File.Exists(FilePath))
            {
                using (SQLiteConnection conn = new SQLiteConnection(FilePath))
                {
                    conn.CreateTable<Instructors>();
                    Instructors instructor = new Instructors
                    {
                        Name = "Kristopher Brendel",
                        Email = "kbrend1@wgu.edu",
                        Phone = "2604799724"
                    };
                    int rows = conn.Insert(instructor);
                    //
                    conn.CreateTable<Terms>();
                    Terms term = new Terms
                    {
                        Title = "Spring 2022",
                        Start = DateTime.Now.AddDays(-7),
                        AlertsStart = false,
                        End = DateTime.Now.AddDays(7),
                        AlertsEnd = false
                    };
                    conn.Insert(term);
                    //
                    string notes = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean euismod iaculis nisl, ultricies sagittis mi ultrices et. Fusce et placerat augue, nec tempus ex. Fusce pellentesque orci at est pulvinar pharetra. Nam sollicitudin ligula et tellus porta, volutpat finibus orci sagittis. Nulla sed arcu volutpat, pulvinar nibh eu, pharetra lectus. Maecenas sodales eros sed justo sollicitudin porta. Nullam pulvinar non ante in lacinia. Curabitur in eros non tellus ornare euismod. Ut nec erat commodo, auctor libero vitae, mollis odio. Morbi iaculis mollis placerat. Aliquam semper, quam non elementum viverra, turpis mauris viverra magna, in pretium mi dui quis est. Morbi ultricies porta hendrerit.";
                    conn.CreateTable<Courses>();
                    Courses course = new Courses
                    {
                        Name = "Web Development Applications C777",
                        Start = DateTime.Now.AddDays(-7),
                        AlertsStart = false,
                        End = DateTime.Now,
                        AlertsEnd = false,
                        Status = StatusType.InProgress,
                        InstructorID = 1,
                        Notes = notes,
                        TermId = 1
                    };
                    conn.Insert(course);
                    //
                    conn.CreateTable<Assessments>();
                    Assessments assess = new Assessments
                    {
                        Name = "Performance Assessment",
                        Type = AsType.Performance,
                        Start = DateTime.Now,
                        AlertsStart = false,
                        End = DateTime.Now,
                        AlertsEnd = false,
                        CourseID = 1
                    };
                    conn.Insert(assess);
                    assess = new Assessments
                    {
                        Name = "Objective Assessment",
                        Type = AsType.Objective,
                        Start = DateTime.Now,
                        AlertsStart = false,
                        End = DateTime.Now,
                        AlertsEnd = false,
                        CourseID = 1
                    };
                    conn.Insert(assess);
                }
            }
            else
            {
                return;
            }
        }
    }
}
