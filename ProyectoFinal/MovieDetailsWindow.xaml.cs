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
        public ObservableCollection<Schedule> Schedules { get; set; }
        public ICommand SelectScheduleCommand { get; }

        public MovieDetailsWindow(Movie movie)
        {
            InitializeComponent();
            DataContext = movie;
            Schedules = new ObservableCollection<Schedule>();
            SelectScheduleCommand = new RelayCommand(SelectSchedule);
            LoadSchedulesFromDatabase(movie.MovieID);
        }

        private void LoadSchedulesFromDatabase(int movieID)
        {
            string connectionString = "Host=hansken.db.elephantsql.com;Username=fvlwmckt;Password=Axo0Bex988-66RWSC_tnApCZrm7hn7k3;Database=fvlwmckt";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ScheduleID, schedule_time FROM schedules WHERE MovieID = @MovieID";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("MovieID", movieID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Schedules.Add(new Schedule
                            {
                                ScheduleID = reader.GetInt32(0),
                                Time = reader.GetString(1)
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
                var seatSelectionWindow = new SeatSelectionWindow(schedule.MovieID, schedule.ScheduleID, DataContext as Movie);
                seatSelectionWindow.Show();
                this.Close();
            }
        }
    }

    public class Schedule
    {
        public int ScheduleID { get; set; }
        public int MovieID { get; set; }
        public string Time { get; set; }
    }
}