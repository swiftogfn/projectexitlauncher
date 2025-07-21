using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WpfApp5.Pages
{
    public partial class Launcher : Page
    {

        public Launcher()
        {
            InitializeComponent();

        }

        private void LauncherSettings_Loaded(object sender, RoutedEventArgs e)
        {

            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(.1)
            };

            DoubleAnimation slideDownAnimation = new DoubleAnimation
            {
                From = -20,
                To = 0,
                Duration = TimeSpan.FromSeconds(.2)
            };

            TranslateTransform translateTransform = new TranslateTransform();
            LauncherSettingsGrid.RenderTransform = translateTransform;

            LauncherSettingsGrid.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
            translateTransform.BeginAnimation(TranslateTransform.XProperty, slideDownAnimation);

        }

        private void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Loggedin = false;
            Properties.Settings.Default.Save();
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new Login());
            }
            else
            {
                System.Windows.MessageBox.Show("MainFrame is null. Navigation failed.");
            }
        }

    }
}