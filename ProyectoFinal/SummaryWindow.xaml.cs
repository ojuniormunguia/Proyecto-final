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
        public List<Seat> SelectedSeats { get; set; }
        public string QRCodeString { get; set; }
        public ICommand ReturnToMainMenuCommand { get; }

        public SummaryWindow(Movie selectedMovie, List<Seat> selectedSeats, string qrCodeString)
        {
            InitializeComponent();
            SelectedMovie = selectedMovie;
            SelectedSeats = selectedSeats;
            QRCodeString = qrCodeString;
            ReturnToMainMenuCommand = new RelayCommand(ReturnToMainMenu);

            foreach (var seat in SelectedSeats)
            {
                seat.QRCodeImage = GenerateQRCodeImage($"Pelicula: {SelectedMovie.Title}\nSala: A\nFormato: 2D DUB\nFecha: 4 de octubre 2024\nHora: 22:00\nAsiento numero: {seat.SeatNumber}");
            }

            DataContext = this;
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

        private void ReturnToMainMenu(object parameter)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}