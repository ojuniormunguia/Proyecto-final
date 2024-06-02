using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class MovieDetailsWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Schedule> _schedules;
        public ObservableCollection<Schedule> Schedules
        {
            get => _schedules;
            set
            {
                _schedules = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectScheduleCommand { get; }

        public Movie SelectedMovie { get; set; }
        private User LoggedInUser { get; set; } // Add this line

        public MovieDetailsWindow(Movie movie, User loggedInUser) // Add loggedInUser parameter
        {
            InitializeComponent();
            SelectedMovie = movie;
            LoggedInUser = loggedInUser; // Initialize LoggedInUser
            DataContext = this;
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
                string query = "SELECT ScheduleID, ScheduleDate FROM schedules WHERE MovieID = @MovieID";
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
                                MovieID = movieID,
                                Time = reader.GetDateTime(1).ToString("g") // Formato general de fecha y hora
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
                var seatSelectionWindow = new SeatSelectionWindow(schedule.MovieID, schedule.ScheduleID, SelectedMovie, LoggedInUser); // Pass loggedInUser
                seatSelectionWindow.Show();
                this.Close();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class Schedule : INotifyPropertyChanged
    {
        private int _scheduleID;
        public int ScheduleID
        {
            get => _scheduleID;
            set
            {
                _scheduleID = value;
                OnPropertyChanged();
            }
        }

        private int _movieID;
        public int MovieID
        {
            get => _movieID;
            set
            {
                _movieID = value;
                OnPropertyChanged();
            }
        }

        private string _time;
        public string Time
        {
            get => _time;
            set
            {
                _time = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }

}