using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProyectoFinal
{
    public partial class MovieDetailsWindow : Window
    {
        public Movie SelectedMovie { get; set; }
        public ObservableCollection<Schedule> Schedules { get; set; }
        public ICommand SelectScheduleCommand { get; }

        public MovieDetailsWindow(Movie movie)
        {
            InitializeComponent();
            SelectedMovie = movie;
            Schedules = new ObservableCollection<Schedule>();
            SelectScheduleCommand = new RelayCommand(SelectSchedule);
            this.DataContext = this;
            LoadSchedulesFromDatabase();
        }

        private void LoadSchedulesFromDatabase()
        {
            string connectionString = "Host=hansken.db.elephantsql.com;Username=fvlwmckt;Password=Axo0Bex988-66RWSC_tnApCZrm7hn7k3;Database=fvlwmckt";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ScheduleID, ScheduleDate, SalaNumber FROM schedules WHERE MovieID = @MovieID";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("MovieID", SelectedMovie.MovieID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Schedules.Add(new Schedule
                            {
                                ScheduleID = reader.GetInt32(0),
                                ScheduleDate = reader.GetDateTime(1),
                                SalaNumber = reader.GetInt32(2),
                                MovieTitle = SelectedMovie.Title
                            });
                        }
                    }
                }
            }
        }

        private void SelectSchedule(object parameter)
        {
            if (parameter is Schedule schedule)
            {
                var seatSelectionWindow = new SeatSelectionWindow(SelectedMovie.MovieID, schedule.ScheduleID);
                seatSelectionWindow.Show();
            }
        }
    }

    public class Schedule
    {
        public int ScheduleID { get; set; }
        public DateTime ScheduleDate { get; set; }
        public int SalaNumber { get; set; }
        public string MovieTitle { get; set; }
    }
}