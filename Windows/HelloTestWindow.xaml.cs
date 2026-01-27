using System;
using System.Windows;
using Windows.Security.Credentials.UI;

namespace FinalYearProject
{
    public partial class HelloTestWindow : Window
    {
        public HelloTestWindow()
        {
            InitializeComponent();
        }

        private async void TestHello_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var availability = await UserConsentVerifier.CheckAvailabilityAsync().AsTask();

                if (availability == UserConsentVerifierAvailability.Available)
                {
                    var result = await UserConsentVerifier.RequestVerificationAsync("Verify with Windows Hello").AsTask();

                    if (result == UserConsentVerificationResult.Verified)
                        MessageBox.Show("✅ Windows Hello verification successful!");
                    else
                        MessageBox.Show("❌ Verification failed: " + result);
                }
                else
                {
                    MessageBox.Show("⚠️ Windows Hello is not available on this device.");
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
