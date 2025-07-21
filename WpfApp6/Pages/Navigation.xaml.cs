using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;

namespace WpfApp5.Pages
{
    public partial class Navigation : Page
    {
        private string? _username;
        private List<string>? _userRoles;

        // Constructor that accepts username and user roles
        public Navigation(string? username, List<string>? userRoles = null)
        {
            InitializeComponent();
            _username = username;
            _userRoles = userRoles;

            if (!(NavigationFrame.Content is Home))
            {
                NavigationFrame.Navigate(new Home(_username));
            }

            AnimateElement(NavigationFrame);
        }

        // Default constructor
        public Navigation()
        {
            InitializeComponent();
        }

        private void AnimateElement(UIElement element)
        {
            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(.2)
            };

            TranslateTransform transform = new TranslateTransform();
            element.RenderTransform = transform;

            var slideIn = new DoubleAnimation
            {
                From = -20,
                To = 0,
                Duration = TimeSpan.FromSeconds(.3),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            element.Visibility = Visibility.Visible;
            element.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            transform.BeginAnimation(TranslateTransform.YProperty, slideIn);
        }

        private void NavigationViewItem_Click_1(object sender, RoutedEventArgs e) // Library
        {
            SetActiveNav(LibraryNav);

            if (!(NavigationFrame.Content is Library))
            {
                NavigationFrame.Navigate(new Library());
            }

            AnimateElement(NavigationFrame);
        }

        private void NavigationViewItem_Click_2(object sender, RoutedEventArgs e) // Home
        {
            string path = Globals.CurrentPath;

            if (string.IsNullOrEmpty(path) || path == "Path must contain FortniteGame and Engine folders!")
            {
                // Optional: show message or handle invalid path
            }

            Properties.Settings.Default.Path = path;
            Properties.Settings.Default.Save();

            SystemSounds.Question.Play();
            SetActiveNav(HomeNav);

            if (!(NavigationFrame.Content is Home))
            {
                NavigationFrame.Navigate(new Home(_username));
            }

            AnimateElement(NavigationFrame);
        }

        private void NavigationViewItem_Click_3(object sender, RoutedEventArgs e) // Settings
        {
            SetActiveNav(SettingsNav);

            if (!(NavigationFrame.Content is Settings))
            {
                NavigationFrame.Navigate(new Settings());
            }

            AnimateElement(NavigationFrame);
        }

        private void NavigationViewItem_Click_4(object sender, RoutedEventArgs e) // Shop
        {
            SetActiveNav(ShopNav);

            if (!(NavigationFrame.Content is Shop))
            {
                NavigationFrame.Navigate(new Shop());
            }

            AnimateElement(NavigationFrame);
        }

        private void NavigationViewItem_Click_ServerStatus(object sender, RoutedEventArgs e)
        {
            SetActiveNav(ServerStatusNav);

            if (!(NavigationFrame.Content is Servers))
            {
                NavigationFrame.Navigate(new Servers());
            }

            AnimateElement(NavigationFrame);
        }


        private void NavigationViewItem_Click_Shop(object sender, RoutedEventArgs e)
        {
            SetActiveNav(ShopNav);

            if (!(NavigationFrame.Content is Shop))
            {
                NavigationFrame.Navigate(new Shop());
            }

            AnimateElement(NavigationFrame);
        }

        private void ShowAccessDenied()
        {
            MessageBox.Show("To access Exit +, Please head over to the discord and donate.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void NavigateToExitPage()
        {
         
            SetActiveNav(ExitNav);

            if (!(NavigationFrame.Content is Exit_))
            {
                NavigationFrame.Navigate(new Exit_(_username));
            }

            AnimateElement(NavigationFrame);
        }

        private void NavigationViewItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            SetActiveNav(ManageNav);

            if (!(NavigationFrame.Content is MngeClrs))
            {
                NavigationFrame.Navigate(new MngeClrs());
            }

            AnimateElement(NavigationFrame);
        }





        private bool UserHasRole(string roleId)
        {
            if (_userRoles == null)
                return false;

            return _userRoles.Contains(roleId);
        }

        private void SetActiveNav(NavigationViewItem activeItem)
        {
            HomeNav.IsActive = false;
            LibraryNav.IsActive = false;
            SettingsNav.IsActive = false;
            ShopNav.IsActive = false;
            ServerStatusNav.IsActive = false;
            ExitNav.IsActive = false;
            ManageNav.IsActive = false;


            activeItem.IsActive = true;
        }

        private void NavigationFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // Optional: handle navigation events here
        }
    }
}
