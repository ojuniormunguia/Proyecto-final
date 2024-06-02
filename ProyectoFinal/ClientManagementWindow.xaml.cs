using Npgsql;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public partial class ClientManagementWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Client> Clients { get; set; }
        public Client SelectedClient { get; set; }
        public Client NewClient { get; set; }
        public bool IsPremium { get; set; }
        public ICommand AddClientCommand { get; }
        public ICommand ContinueCommand { get; }

        private int ScheduleID { get; set; }
        private List<Seat> SelectedSeats { get; set; }
        private Movie SelectedMovie { get; set; }

        public ClientManagementWindow(int scheduleID, List<Seat> selectedSeats, Movie selectedMovie)
        {
            InitializeComponent();
            ScheduleID = scheduleID;
            SelectedSeats = selectedSeats;
            SelectedMovie = selectedMovie;
            Clients = new ObservableCollection<Client>();
            NewClient = new Client();
            AddClientCommand = new RelayCommand(AddClient);
            ContinueCommand = new RelayCommand(Continue);
            DataContext = this;
            LoadClientsFromDatabase();
        }

        private void LoadClientsFromDatabase()
        {
            string connectionString = "Host=hansken.db.elephantsql.com;Username=fvlwmckt;Password=Axo0Bex988-66RWSC_tnApCZrm7hn7k3;Database=fvlwmckt";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ClientID, Name, Gender, Age, Email, Phone FROM Clients";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Clients.Add(new Client
                        {
                            ClientID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Gender = reader.GetString(2),
                            Age = reader.GetInt32(3),
                            Email = reader.GetString(4),
                            Phone = reader.GetString(5)
                        });
                    }
                }
            }
        }

        private void AddClient(object parameter)
        {
            string connectionString = "Host=hansken.db.elephantsql.com;Username=fvlwmckt;Password=Axo0Bex988-66RWSC_tnApCZrm7hn7k3;Database=fvlwmckt";
            int newClientID;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    string query = "INSERT INTO Clients (Name, Gender, Age, Email, Phone) VALUES (@Name, @Gender, @Age, @Email, @Phone) RETURNING ClientID";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("Name", NewClient.Name);
                        cmd.Parameters.AddWithValue("Gender", NewClient.Gender);
                        cmd.Parameters.AddWithValue("Age", NewClient.Age);
                        cmd.Parameters.AddWithValue("Email", NewClient.Email);
                        cmd.Parameters.AddWithValue("Phone", NewClient.Phone);
                        newClientID = (int)cmd.ExecuteScalar();
                    }

                    if (IsPremium)
                    {
                        string premiumQuery = "INSERT INTO PremiumClients (ClientID, Points) VALUES (@ClientID, 0)";
                        using (var cmd = new NpgsqlCommand(premiumQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("ClientID", newClientID);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
            }

            Clients.Add(new Client
            {
                ClientID = newClientID,
                Name = NewClient.Name,
                Gender = NewClient.Gender,
                Age = NewClient.Age,
                Email = NewClient.Email,
                Phone = NewClient.Phone
            });

            NewClient = new Client();
            OnPropertyChanged(nameof(NewClient));
            IsPremium = false;
            OnPropertyChanged(nameof(IsPremium));

            MessageBox.Show("Cliente agregado correctamente. Ahora puede seleccionar el cliente para continuar.");
        }

        private void Continue(object parameter)
        {
            if (SelectedClient == null)
            {
                MessageBox.Show("Por favor, seleccione un cliente para continuar.");
                return;
            }

            string connectionString = "Host=hansken.db.elephantsql.com;Username=fvlwmckt;Password=Axo0Bex988-66RWSC_tnApCZrm7hn7k3;Database=fvlwmckt";
            List<TicketInfo> newTickets = new List<TicketInfo>();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    foreach (var seat in SelectedSeats)
                    {
                        string ticketCode = Guid.NewGuid().ToString();
                        string query = "INSERT INTO Tickets (ClientID, ScheduleID, SeatID, TicketCode) VALUES (@ClientID, @ScheduleID, @SeatID, @TicketCode)";
                        using (var cmd = new NpgsqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("ClientID", SelectedClient.ClientID);
                            cmd.Parameters.AddWithValue("ScheduleID", ScheduleID);
                            cmd.Parameters.AddWithValue("SeatID", seat.SeatID);
                            cmd.Parameters.AddWithValue("TicketCode", ticketCode);
                            cmd.ExecuteNonQuery();

                            newTickets.Add(new TicketInfo
                            {
                                ClientName = SelectedClient.Name,
                                ScheduleTime = "4 de octubre 2024 22:00",
                                SeatNumber = seat.SeatNumber.ToString(),
                                TicketCode = ticketCode,
                                QRCodeImage = GenerateQRCodeImage(ticketCode)
                            });
                        }
                    }
                    transaction.Commit();
                }
            }

            var summaryWindow = new SummaryWindow(SelectedMovie, newTickets);
            summaryWindow.Show();
            this.Close();
        }

        private BitmapImage GenerateQRCodeImage(string qrText)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImage = qrCode.GetGraphic(20, new byte[] { 0, 0, 0 }, new byte[] { 255, 255, 255 }, true);

            using (var ms = new MemoryStream(qrCodeImage))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class Client : INotifyPropertyChanged
    {
        private int _clientID;
        public int ClientID
        {
            get => _clientID;
            set
            {
                _clientID = value;
                OnPropertyChanged();
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private string _gender;
        public string Gender
        {
            get => _gender;
            set
            {
                _gender = value;
                OnPropertyChanged();
            }
        }

        private int _age;
        public int Age
        {
            get => _age;
            set
            {
                _age = value;
                OnPropertyChanged();
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _phone;
        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
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