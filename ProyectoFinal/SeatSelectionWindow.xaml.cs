using Npgsql;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
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
    public partial class SeatSelectionWindow : Window, INotifyPropertyChanged
    {
        public int MovieID { get; set; }
        public int ScheduleID { get; set; }
        public Movie SelectedMovie { get; set; }
        public ObservableCollection<Seat> Seats { get; set; }
        public ICommand ToggleSeatCommand { get; }
        public ICommand ReservarCommand { get; }

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

        private BitmapImage GenerateQRCodeImage(string qrText)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    using (Bitmap qrCodeBitmap = qrCode.GetGraphic(20))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            qrCodeBitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                            memoryStream.Position = 0;

                            BitmapImage qrCodeImage = new BitmapImage();
                            qrCodeImage.BeginInit();
                            qrCodeImage.StreamSource = memoryStream;
                            qrCodeImage.CacheOption = BitmapCacheOption.OnLoad;
                            qrCodeImage.EndInit();

                            return qrCodeImage;
                        }
                    }
                }
            }
        }

        private string GenerateQRCodeString()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 32);
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
                string query = "SELECT SeatNumber, IsAvailable FROM seats WHERE ScheduleID = @ScheduleID";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("ScheduleID", ScheduleID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int seatNumber = reader.GetInt32(0);
                            bool isAvailable = reader.GetBoolean(1);

                            var seat = Seats.FirstOrDefault(s => s.SeatNumber == seatNumber);
                            if (seat != null)
                            {
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
                        string query = "INSERT INTO seats (ScheduleID, SeatNumber, IsAvailable) VALUES (@ScheduleID, @SeatNumber, @IsAvailable)";
                        using (var cmd = new NpgsqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("ScheduleID", ScheduleID);
                            cmd.Parameters.AddWithValue("SeatNumber", seat.SeatNumber);
                            cmd.Parameters.AddWithValue("IsAvailable", false);
                            cmd.ExecuteNonQuery();
                        }
                        // Generar código QR
                        seat.QRCodeImage = GenerateQRCodeImage(seat.SeatNumber.ToString());
                    }
                    transaction.Commit();
                }
            }

            // Mostrar la ventana de resumen
            var summaryWindow = new SummaryWindow(SelectedMovie, Seats.Where(s => s.IsSelected).ToList(), GenerateQRCodeString());
            summaryWindow.Show();
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