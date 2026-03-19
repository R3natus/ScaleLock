using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using FinalYearProject.MethodsMan;

namespace FinalYearProject.Windows
{
    public partial class PasswordGenerator : Window
    {
        public PasswordGenerator()
        {
            InitializeComponent();

            // Default mascot state (Investigating)
            MascotImage.Source = LoadMascot("Investigating.png");
            FeedbackText.Text = "Please enter your password!";
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) // Copy and pasted from LoginScreen.xaml.cs for dragging the window around
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e) // Copy and pasted from LoginScreen.xaml.cs for minimizing the window
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) // Copy and pasted from LoginScreen.xaml.cs for closing the window
        {
            this.Close();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string password = PasswordBox.Text;
            int score = EvaluateStrength(password);

            if (string.IsNullOrEmpty(password))
            {
                MascotImage.Source = LoadMascot("Investigating.png");
                FeedbackText.Text = "Please enter your password!";
                return;
            }

            // This is me messing around, ignore this :)

            if (password.Equals("dragon", StringComparison.OrdinalIgnoreCase))
            {
                MascotImage.Source = LoadMascot("Happy.png");
                FeedbackText.Text = "I'm glad you noticed!";
                return;
            }

            // These are the top 20 most common password, according to Wikipedia and Nordpass

            if (password.Equals("123456", StringComparison.OrdinalIgnoreCase) ||
                password.Equals("admin", StringComparison.OrdinalIgnoreCase) ||
                password.Equals("12345678", StringComparison.OrdinalIgnoreCase) ||
                password.Equals("123456789", StringComparison.OrdinalIgnoreCase) ||
                password.Equals("12345", StringComparison.OrdinalIgnoreCase) ||
                password.Equals("password", StringComparison.OrdinalIgnoreCase) ||
                password.Equals("Aa123456", StringComparison.OrdinalIgnoreCase) ||
                password.Equals("1234567890", StringComparison.OrdinalIgnoreCase) ||
                password.Equals("Pass@123", StringComparison.OrdinalIgnoreCase) ||
                password.Equals("admin123", StringComparison.OrdinalIgnoreCase) ||
                password.Equals("1234567", StringComparison.OrdinalIgnoreCase) ||
                password.Equals("123123", StringComparison.OrdinalIgnoreCase) ||
                password.Equals("111111", StringComparison.OrdinalIgnoreCase) ||
                password.Equals("12345678910", StringComparison.OrdinalIgnoreCase) ||
                password.Equals("P@ssw0rd", StringComparison.OrdinalIgnoreCase) ||
                password.Equals("Password", StringComparison.OrdinalIgnoreCase) ||
                password.Equals("Aa@123456", StringComparison.OrdinalIgnoreCase) ||
                password.Equals("admintelecom", StringComparison.OrdinalIgnoreCase) ||
                password.Equals("Admin@123", StringComparison.OrdinalIgnoreCase) ||
                password.Equals("112233", StringComparison.OrdinalIgnoreCase))
            {
                MascotImage.Source = LoadMascot("Worried.png");
                FeedbackText.Text = "I'm sorry, this password won't suffice...";
                return;
            }

            // I use a score system here, depending on if there's uppercase, lowercase, numbers and special characters
            // The score goes up if you keep adding those things <3

            if (score < 3)
            {
                MascotImage.Source = LoadMascot("Worried.png");
                FeedbackText.Text = "I'm sorry, this does not seem like a reliable password!";
            }
            else if (score < 6)
            {
                MascotImage.Source = LoadMascot("Investigating.png");
                FeedbackText.Text = "You can definitely make this better! Keep going.";
            }
            else
            {
                MascotImage.Source = LoadMascot("Happy.png");
                FeedbackText.Text = "Excellent! This password seems powerful!.";
            }
        }

        // Below is the scoring system, think of it as a fancy counter!

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

        private void GeneratePassword_Click(object sender, RoutedEventArgs e)
        {
            string strongPassword = PasswordGen.GenerateStrongPassword(16);
            PasswordBox.Text = strongPassword;
            FeedbackText.Text = "You've got it!";
        }


        // This is the program summoning the dragon mascot, via the assets folder
        private BitmapImage LoadMascot(string fileName)
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Windows", "Assets", fileName);
            return new BitmapImage(new Uri(path, UriKind.Absolute));
        }
    }
}
