using Npgsql;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProyectoFinal
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Movie> _movies;
        public ObservableCollection<Movie> Movies
        {
            get => _movies;
            set
            {
                _movies = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Movie> _filteredMovies;
        public ObservableCollection<Movie> FilteredMovies
        {
            get => _filteredMovies;
            set
            {
                _filteredMovies = value;
                OnPropertyChanged();
            }
        }

        private Movie _featuredMovie;
        public Movie FeaturedMovie
        {
            get => _featuredMovie;
            set
            {
                _featuredMovie = value;
                OnPropertyChanged();
            }
        }

        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenUserManagementCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainWindow(User loggedInUser)
        {
            InitializeComponent();
            LoggedInUser = loggedInUser;
            Movies = new ObservableCollection<Movie>();
            FilteredMovies = new ObservableCollection<Movie>();
            DataContext = this;
            LoadMoviesFromDatabase();
            CheckAdminPermissions();

            OpenUserManagementCommand = new RelayCommand(OpenUserManagement);
            LogoutCommand = new RelayCommand(Logout);
        }

        public User LoggedInUser { get; set; }

        private void CheckAdminPermissions()
        {
            if (LoggedInUser.Permissions == "Administrador")
            {
                AdminButton.Visibility = Visibility.Visible;
            }
            else
            {
                AdminButton.Visibility = Visibility.Collapsed;
            }
        }

        private async void LoadMoviesFromDatabase()
        {
            try
            {
                string connectionString = "Host=hansken.db.elephantsql.com;Username=fvlwmckt;Password=Axo0Bex988-66RWSC_tnApCZrm7hn7k3;Database=fvlwmckt";
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "SELECT MovieID::int, Title, Description, ImageURL FROM movies";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var movie = new Movie
                            {
                                MovieID = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Description = reader.GetString(2),
                                ImageURL = reader.GetString(3)
                            };
                            Movies.Add(movie);
                            FilteredMovies.Add(movie); // Initialize FilteredMovies with all movies
                        }
                    }
                }

                // Set a random featured movie
                if (Movies.Count > 0)
                    FeaturedMovie = Movies[new Random().Next(Movies.Count)];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading movies: {ex.Message}");
            }
        }

        private void Movie_Click(object sender, RoutedEventArgs e)
        {
            if (sender is StackPanel panel && panel.DataContext is Movie movie)
            {
                var detailsWindow = new MovieDetailsWindow(movie, LoggedInUser); // Pass loggedInUser
                detailsWindow.Show();
                this.Close();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return; // Safeguard against null reference.

            var filter = textBox.Text.ToLower();

            // Ensure FilteredMovies is not null
            if (FilteredMovies == null)
                FilteredMovies = new ObservableCollection<Movie>();

            FilteredMovies.Clear(); // Clear the existing entries

            // Ensure Movies is not null and filter movies, checking for null titles
            if (Movies != null)
            {
                foreach (var movie in Movies.Where(m => m.Title != null && m.Title.ToLower().Contains(filter)))
                {
                    FilteredMovies.Add(movie);
                }
            }
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "Search Movies...")
            {
                textBox.Text = "";
                textBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Search Movies...";
                textBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void OpenUserManagement(object parameter)
        {
            var userManagementWindow = new UserManagementWindow(LoggedInUser);
            userManagementWindow.Show();
            this.Close();
        }

        private void Logout(object parameter)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            OpenUserManagement(null);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Logout(null);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class Movie : INotifyPropertyChanged
    {
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

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private string _imageURL;
        public string ImageURL
        {
            get => _imageURL;
            set
            {
                _imageURL = value;
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