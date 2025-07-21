using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WpfApp5.Pages
{
    public partial class Game : Page
    {
        public Game()
        {
            InitializeComponent();
            EORSwitch.IsChecked = Properties.Settings.Default.bUseEOR;
            DPESwitch.IsChecked = Properties.Settings.Default.bUseDPE;
        }

        private void GameSettings_Loaded(object sender, RoutedEventArgs e)
        {

            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(.2)
            };

            DoubleAnimation slideDownAnimation = new DoubleAnimation
            {
                From = -20,
                To = 0,
                Duration = TimeSpan.FromSeconds(.3)
            };

            TranslateTransform translateTransform = new TranslateTransform();
            GameGrid.RenderTransform = translateTransform;

            GameGrid.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
            translateTransform.BeginAnimation(TranslateTransform.XProperty, slideDownAnimation);

        }

        private void EOR_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.bUseEOR = true;
            Properties.Settings.Default.Save();
        }

        private void EOR_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.bUseEOR = false;
            Properties.Settings.Default.Save();
        }

        private void DPE_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.bUseDPE = true;
            Properties.Settings.Default.Save();
        }

        private void DPE_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.bUseDPE = false;
            Properties.Settings.Default.Save();
        }
    }
}
