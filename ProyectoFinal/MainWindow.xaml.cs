using Npgsql;
using System.Collections.ObjectModel;
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
using Npgsql;

namespace ProyectoFinal
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Movie> Movies { get; set; }
        public ObservableCollection<Movie> FilteredMovies { get; set; }
        public Movie FeaturedMovie { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Movies = new ObservableCollection<Movie>();
            FilteredMovies = new ObservableCollection<Movie>();
            this.DataContext = this;
            LoadMoviesFromDatabase();
        }

        private void LoadMoviesFromDatabase()
        {
            string connectionString = "Host=hansken.db.elephantsql.com;Username=fvlwmckt;Password=Axo0Bex988-66RWSC_tnApCZrm7hn7k3;Database=fvlwmckt";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT MovieID::int, Title, Description, ImageURL FROM movies";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
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

        private void Movie_Click(object sender, RoutedEventArgs e)
        {
            if (sender is StackPanel panel && panel.DataContext is Movie movie)
            {
                var detailsWindow = new MovieDetailsWindow(movie);
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
    }

    public class Movie
    {
        public int MovieID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
    }
}