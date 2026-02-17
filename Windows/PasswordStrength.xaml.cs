using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FinalYearProject.Windows
{
    public partial class PasswordGenerator : Window
    {
        public PasswordGenerator()
        {
            InitializeComponent();

            // Default mascot state (Investigating)
            MascotImage.Source = LoadMascot("Investigating.png");
            FeedbackText.Text = "Let's see what you've got.";
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string password = PasswordBox.Password;
            int score = EvaluateStrength(password);

            if (string.IsNullOrEmpty(password))
            {
                MascotImage.Source = LoadMascot("Investigating.png");
                FeedbackText.Text = "Let's see what you've got!";
                return;
            }

            if (score < 3)
            {
                MascotImage.Source = LoadMascot("Worried.png");
                FeedbackText.Text = "...This is horrible.";
            }
            else if (score < 6)
            {
                MascotImage.Source = LoadMascot("Investigating.png");
                FeedbackText.Text = "You can make this better! Keep going.";
            }
            else
            {
                MascotImage.Source = LoadMascot("Happy.png");
                FeedbackText.Text = "Excellent! This password is worthy of ScaleLock.";
            }
        }

        private int EvaluateStrength(string password)
        {
            if (string.IsNullOrEmpty(password)) return 0;

            int score = 0;
            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;
            if (password.Any(char.IsUpper)) score++;
            if (password.Any(char.IsLower)) score++;
            if (password.Any(char.IsDigit)) score++;
            if (password.Any(ch => !char.IsLetterOrDigit(ch))) score++;

            return score;
        }



        private BitmapImage LoadMascot(string fileName)
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Windows", "Assets", fileName);
            return new BitmapImage(new Uri(path, UriKind.Absolute));
        }
    }
}
