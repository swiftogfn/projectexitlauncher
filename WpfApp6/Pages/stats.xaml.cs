using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp5.Pages
{
    public partial class stats : Page
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public stats()
        {
            InitializeComponent();
        }

        private async void FetchNumberButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Replace this URL with your backend URL if remote
                string url = "http://86.25.153.166:3551/api/number";

                string response = await httpClient.GetStringAsync(url);

                // The API returns just a number (like 5), so display it directly
                NumberTextBlock.Text = $"{response}";
            }
            catch (Exception ex)
            {
                NumberTextBlock.Text = $"Error fetching number: {ex.Message}";
            }
        }
    }
}
