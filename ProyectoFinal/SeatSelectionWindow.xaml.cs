using Npgsql;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ProyectoFinal
{
    public partial class SeatSelectionWindow : Window, INotifyPropertyChanged
    {
        public int MovieID { get; set; }
        public int ScheduleID { get; set; }
        public Movie SelectedMovie { get; set; }
        public ObservableCollection<Seat> Seats { get; set; }
        public ICommand ToggleSeatCommand { get; }
        public ICommand ReservarCommand { get; }
        public ICommand ContinueCommand { get; }

        private string _selectedSeatIDs;
        public string SelectedSeatIDs
        {
            get => _selectedSeatIDs;
            set
            {
                _selectedSeatIDs = value;
                OnPropertyChanged();
            }
        }

        public SeatSelectionWindow(int movieID, int scheduleID, Movie selectedMovie)
        {
            InitializeComponent();
            MovieID = movieID;
            ScheduleID = scheduleID;
            SelectedMovie = selectedMovie ?? throw new ArgumentNullException(nameof(selectedMovie));
            Seats = new ObservableCollection<Seat>();
            ToggleSeatCommand = new RelayCommand(ToggleSeat);
            ReservarCommand = new RelayCommand(Reservar);
            ContinueCommand = new RelayCommand(OpenClientManagementWindow);
            DataContext = this;
            GenerateSeats();
            LoadSeatsFromDatabase();
        }

        private void GenerateSeats()
        {
            for (int i = 1; i <= 93; i++)
            {
                Seats.Add(new Seat
                {
                    SeatNumber = i,
                    IsAvailable = true,
                    IsSelected = false
                });
            }
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
                            int seatID = reader.GetInt32(0);
                            int seatNumber = reader.GetInt32(1);
                            bool isAvailable = reader.GetBoolean(2);

                            var seat = Seats.FirstOrDefault(s => s.SeatNumber == seatNumber);
                            if (seat != null)
                            {
                                seat.SeatID = seatID;
                                seat.IsAvailable = isAvailable;
                                seat.IsSelected = !isAvailable;
                            }
                        }
                    }
                }
            }

            // Asegurar que los asientos estén ordenados por SeatNumber
            Seats = new ObservableCollection<Seat>(Seats.OrderBy(s => s.SeatNumber));
            UpdateSelectedSeatIDs(); // Initialize the selected seats
        }

        private void ToggleSeat(object parameter)
        {
            if (parameter is Seat seat)
            {
                seat.IsSelected = !seat.IsSelected;
                seat.IsAvailable = !seat.IsSelected; // Update IsAvailable based on IsSelected
                UpdateSelectedSeatIDs();
            }
        }

        private void UpdateSelectedSeatIDs()
        {
            SelectedSeatIDs = string.Join(", ", Seats.Where(s => s.IsSelected).Select(s => s.SeatNumber));
            Console.WriteLine($"Selected Seats: {SelectedSeatIDs}"); // Debugging output
        }

        private void Reservar(object parameter)
        {
            string connectionString = "Host=hansken.db.elephantsql.com;Username=fvlwmckt;Password=Axo0Bex988-66RWSC_tnApCZrm7hn7k3;Database=fvlwmckt";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    foreach (var seat in Seats.Where(s => s.IsSelected))
                    {
                        // First, try to update the seat if it exists
                        string updateSeatQuery = @"
                        UPDATE seats
                        SET IsAvailable = false
                        WHERE ScheduleID = @ScheduleID AND SeatNumber = @SeatNumber;
                        ";

                        using (var cmd = new NpgsqlCommand(updateSeatQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("ScheduleID", ScheduleID);
                            cmd.Parameters.AddWithValue("SeatNumber", seat.SeatNumber);
                            int rowsAffected = cmd.ExecuteNonQuery();

                            // If no rows were updated, insert a new seat
                            if (rowsAffected == 0)
                            {
                                string insertSeatQuery = @"
                                INSERT INTO seats (ScheduleID, SeatNumber, IsAvailable)
                                VALUES (@ScheduleID, @SeatNumber, false)
                                RETURNING SeatID;
                                ";

                                using (var insertCmd = new NpgsqlCommand(insertSeatQuery, conn))
                                {
                                    insertCmd.Parameters.AddWithValue("ScheduleID", ScheduleID);
                                    insertCmd.Parameters.AddWithValue("SeatNumber", seat.SeatNumber);
                                    seat.SeatID = (int)insertCmd.ExecuteScalar();
                                }
                            }
                        }
                    }
                    transaction.Commit();
                }
            }

            MessageBox.Show("¡Los asientos han sido reservados con éxito!");
            OpenClientManagementWindow(null);
        }

        private void OpenClientManagementWindow(object parameter)
        {
            var selectedSeats = Seats.Where(s => s.IsSelected).ToList();
            var clientManagementWindow = new ClientManagementWindow(ScheduleID, selectedSeats, SelectedMovie);
            clientManagementWindow.Show();
            this.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class Seat : INotifyPropertyChanged
    {
        private int _seatID;
        public int SeatID
        {
            get => _seatID;
            set
            {
                _seatID = value;
                OnPropertyChanged();
            }
        }

        private int _seatNumber;
        public int SeatNumber
        {
            get => _seatNumber;
            set
            {
                _seatNumber = value;
                OnPropertyChanged();
            }
        }

        private bool _isAvailable;
        public bool IsAvailable
        {
            get => _isAvailable;
            set
            {
                _isAvailable = value;
                OnPropertyChanged();
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAvailable)); // Notify that IsAvailable has changed too
            }
        }

        private BitmapImage _qrCodeImage;
        public BitmapImage QRCodeImage
        {
            get => _qrCodeImage;
            set
            {
                _qrCodeImage = value;
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