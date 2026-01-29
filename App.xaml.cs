using System.Windows;

namespace FinalYearProject
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var loginScreen = new Windows.LoginScreen();
            loginScreen.Show();
        }

    }
}
