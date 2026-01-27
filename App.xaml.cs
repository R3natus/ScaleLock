using System.Windows;

namespace FinalYearProject
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var hello = new HelloTestWindow();
            hello.Show();
        }
    }
}
