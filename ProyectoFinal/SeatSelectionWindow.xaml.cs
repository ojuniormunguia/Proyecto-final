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
        public ICommand ReservarCommand { get; }
        public Movie SelectedMovie { get; set; }

        private int MovieID { get; }
        private int ScheduleID { get; }

        public SeatSelectionWindow(int movieID, int scheduleID, Movie selectedMovie)
        {
            InitializeComponent();
            MovieID = movieID;
            ScheduleID = scheduleID;
            SelectedMovie = selectedMovie ?? throw new ArgumentNullException(nameof(selectedMovie));
            Seats = new ObservableCollection<Seat>();
            ToggleSeatCommand = new RelayCommand(ToggleSeat);
            ReservarCommand = new RelayCommand(Reservar);
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

            // Agregar asientos restantes si no todos los 93 asientos están en la base de datos
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

        private void ToggleSeat(object? parameter)
        {
            if (parameter is Seat seat)
            {
                seat.IsSelected = !seat.IsSelected;
            }
        }

        private void Reservar(object? parameter)
        {
            string connectionString = "Host=hansken.db.elephantsql.com;Username=fvlwmckt;Password=Axo0Bex988-66RWSC_tnApCZrm7hn7k3;Database=fvlwmckt";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    foreach (var seat in Seats.Where(s => s.IsSelected))
                    {
                        string query = "UPDATE seats SET IsAvailable = @IsAvailable WHERE SeatID = @SeatID";
                        using (var cmd = new NpgsqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("SeatID", seat.SeatID);
                            cmd.Parameters.AddWithValue("IsAvailable", false);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }
            MessageBox.Show("¡Los asientos han sido reservados con éxito!");
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
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

}