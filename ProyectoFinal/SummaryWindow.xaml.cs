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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProyectoFinal
{
    public partial class SummaryWindow : Window
    {
        public Movie SelectedMovie { get; set; }
        public List<TicketInfo> Tickets { get; set; }
        public ICommand ReturnToMainMenuCommand { get; }

        public SummaryWindow(Movie selectedMovie, List<TicketInfo> tickets)
        {
            InitializeComponent();
            SelectedMovie = selectedMovie;
            Tickets = tickets;
            ReturnToMainMenuCommand = new RelayCommand(ReturnToMainMenu);

            DataContext = this;
        }

        private void ReturnToMainMenu(object parameter)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}