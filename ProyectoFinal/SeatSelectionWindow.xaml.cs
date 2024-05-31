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
    public partial class SeatSelectionWindow : Window
    {
        public ObservableCollection<Seat> Seats { get; set; }
        public ICommand ToggleSeatCommand { get; }
        public ICommand SiguienteCommand { get; }

        private int MovieID { get; }
        private int ScheduleID { get; }

        public SeatSelectionWindow(int movieID, int scheduleID)
        {
            InitializeComponent();
            MovieID = movieID;
            ScheduleID = scheduleID;
            Seats = new ObservableCollection<Seat>();
            ToggleSeatCommand = new RelayCommand(ToggleSeat);
            SiguienteCommand = new RelayCommand(Siguiente);
            this.DataContext = this;
            LoadSeatsFromDatabase();
        }

        private void LoadSeatsFromDatabase()
        {
            string connectionString = "Host=hansken.db.elephantsql.com;Username=fvlwmckt;Password=Axo0Bex988-66RWSC_tnApCZrm7hn7k3;Database=fvlwmckt";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT SeatID, SeatNumber, IsAvailable FROM seats WHERE ScheduleID = @ScheduleID";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("ScheduleID", ScheduleID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Seats.Add(new Seat
                            {
                                SeatID = reader.GetInt32(0),
                                SeatNumber = reader.GetInt32(1),
                                IsAvailable = reader.GetBoolean(2),
                                IsSelected = false
                            });
                        }
                    }
                }
            }

            // Add remaining seats if not all 93 seats are in the database
            for (int i = 1; i <= 93; i++)
            {
                if (Seats.All(s => s.SeatNumber != i))
                {
                    Seats.Add(new Seat
                    {
                        SeatNumber = i,
                        IsAvailable = true,
                        IsSelected = false
                    });
                }
            }
        }

        private void ToggleSeat(object parameter)
        {
            if (parameter is Seat seat)
            {
                seat.IsSelected = !seat.IsSelected;
            }
        }

        private void Siguiente(object parameter)
        {
            string connectionString = "Host=hansken.db.elephantsql.com;Username=fvlwmckt;Password=Axo0Bex988-66RWSC_tnApCZrm7hn7k3;Database=fvlwmckt";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    foreach (var seat in Seats.Where(s => s.IsSelected))
                    {
                        string query = "INSERT INTO seats (ScheduleID, SeatNumber, IsAvailable) VALUES (@ScheduleID, @SeatNumber, @IsAvailable)";
                        using (var cmd = new NpgsqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("ScheduleID", ScheduleID);
                            cmd.Parameters.AddWithValue("SeatNumber", seat.SeatNumber);
                            cmd.Parameters.AddWithValue("IsAvailable", false); // Assume seat becomes unavailable once selected
                            cmd.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }
            MessageBox.Show("Seats have been successfully updated!");
            this.Close();
        }
    }

    public class Seat
    {
        public int SeatID { get; set; }
        public int SeatNumber { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsSelected { get; set; }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}