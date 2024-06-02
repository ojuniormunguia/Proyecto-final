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
    public partial class UserManagementWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<User> Users { get; set; }
        public User SelectedUser { get; set; }
        public User NewUser { get; set; }
        public ICommand SaveUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public User LoggedInUser { get; set; }

        public UserManagementWindow(User loggedInUser)
        {
            InitializeComponent();
            LoggedInUser = loggedInUser;
            Users = new ObservableCollection<User>();
            NewUser = new User();
            SaveUserCommand = new RelayCommand(SaveUser);
            DeleteUserCommand = new RelayCommand(DeleteUser, CanDeleteUser);
            DataContext = this;
            LoadUsersFromDatabase();
        }

        private void LoadUsersFromDatabase()
        {
            string connectionString = "Host=hansken.db.elephantsql.com;Username=fvlwmckt;Password=Axo0Bex988-66RWSC_tnApCZrm7hn7k3;Database=fvlwmckt";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT UserID, Name, Username, Password, Permissions FROM Users";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Users.Add(new User
                        {
                            UserID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Username = reader.GetString(2),
                            Password = reader.GetString(3),
                            Permissions = reader.GetString(4)
                        });
                    }
                }
            }
        }

        private void SaveUser(object parameter)
        {
            string connectionString = "Host=hansken.db.elephantsql.com;Username=fvlwmckt;Password=Axo0Bex988-66RWSC_tnApCZrm7hn7k3;Database=fvlwmckt";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                if (NewUser.UserID == 0) // New user
                {
                    string query = "INSERT INTO Users (Name, Username, Password, Permissions) VALUES (@Name, @Username, @Password, @Permissions) RETURNING UserID";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("Name", NewUser.Name);
                        cmd.Parameters.AddWithValue("Username", NewUser.Username);
                        cmd.Parameters.AddWithValue("Password", NewUser.Password);
                        cmd.Parameters.AddWithValue("Permissions", NewUser.Permissions);
                        NewUser.UserID = (int)cmd.ExecuteScalar();
                        Users.Add(NewUser);
                    }
                }
                else // Update existing user
                {
                    string query = "UPDATE Users SET Name = @Name, Username = @Username, Password = @Password, Permissions = @Permissions WHERE UserID = @UserID";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("Name", NewUser.Name);
                        cmd.Parameters.AddWithValue("Username", NewUser.Username);
                        cmd.Parameters.AddWithValue("Password", NewUser.Password);
                        cmd.Parameters.AddWithValue("Permissions", NewUser.Permissions);
                        cmd.Parameters.AddWithValue("UserID", NewUser.UserID);
                        cmd.ExecuteNonQuery();

                        var user = Users.FirstOrDefault(u => u.UserID == NewUser.UserID);
                        if (user != null)
                        {
                            user.Name = NewUser.Name;
                            user.Username = NewUser.Username;
                            user.Password = NewUser.Password;
                            user.Permissions = NewUser.Permissions;
                        }
                    }
                }

                NewUser = new User();
                OnPropertyChanged(nameof(NewUser));
            }
        }

        private bool CanDeleteUser(object parameter)
        {
            return SelectedUser != null;
        }

        private void DeleteUser(object parameter)
        {
            if (SelectedUser == null) return;

            string connectionString = "Host=hansken.db.elephantsql.com;Username=fvlwmckt;Password=Axo0Bex988-66RWSC_tnApCZrm7hn7k3;Database=fvlwmckt";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM Users WHERE UserID = @UserID";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("UserID", SelectedUser.UserID);
                    cmd.ExecuteNonQuery();
                }

                Users.Remove(SelectedUser);
                SelectedUser = null;
                OnPropertyChanged(nameof(SelectedUser));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class User : INotifyPropertyChanged
    {
        private int _userID;
        public int UserID
        {
            get => _userID;
            set
            {
                _userID = value;
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

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private string _permissions;
        public string Permissions
        {
            get => _permissions;
            set
            {
                _permissions = value;
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