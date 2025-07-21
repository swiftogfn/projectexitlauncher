using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WpfApp5.Pages
{
    public partial class Shop : Page
    {
        private readonly Dictionary<string, (string Name, string IconUrl)> cosmeticsCache = new();

        public Shop()
        {
            InitializeComponent();
            _ = InitializeShopAsync();


        }


        

    




    public class ItemShopEntry
        {
            public string[] itemGrants { get; set; }
            public int price { get; set; }
        }

        public class ItemShopItem
        {
            public string Key { get; set; }
            public string ItemId { get; set; }
            public int Price { get; set; }
            public string ImageUrl { get; set; }
            public string DisplayName { get; set; }
        }

        private async Task InitializeShopAsync()
        {
            try
            {
                await LoadCosmeticsAsync();
                await LoadShopAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing shop:\n" + ex.Message);
            }
        }

        private async Task LoadCosmeticsAsync()
        {

            using HttpClient client = new();
            string url = "https://fortnite-api.com/v2/cosmetics/br";
            string response = await client.GetStringAsync(url);

            using JsonDocument doc = JsonDocument.Parse(response);
            var root = doc.RootElement;

            if (root.GetProperty("status").GetInt32() == 200)
            {
                foreach (var cosmetic in root.GetProperty("data").EnumerateArray())
                {
                    string id = cosmetic.GetProperty("id").GetString();
                    string name = cosmetic.GetProperty("name").GetString();

                    string icon = cosmetic.TryGetProperty("images", out var images) &&
                                  images.TryGetProperty("icon", out var iconProp)
                        ? iconProp.GetString()
                        : $"https://fortnite-api.com/images/cosmetics/br/{id}/icon.png";

                    if (!string.IsNullOrEmpty(id))
                    {
                        cosmeticsCache[id] = (name, icon);
                    }
                }
            }
            else
            {
                throw new Exception("Failed to load cosmetics data.");
            }
        }

        private async Task LoadShopAsync()
        {
            string url = "http://86.25.153.166:3551/api/shop";

            using HttpClient client = new();
            string json = await client.GetStringAsync(url);

            using JsonDocument doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var shopDict = new Dictionary<string, ItemShopEntry>();

            foreach (JsonProperty prop in root.EnumerateObject())
            {
                if (prop.Name == "//") continue;

                var entry = JsonSerializer.Deserialize<ItemShopEntry>(prop.Value.GetRawText());
                if (entry != null)
                    shopDict[prop.Name] = entry;
            }

            var dailyItems = new List<ItemShopItem>();
            var featuredItems = new List<ItemShopItem>();
            featuretext.Visibility = Visibility.Visible;
            dailytetx.Visibility = Visibility.Visible;
            loadingtext.Visibility = Visibility.Collapsed;
            foreach (var kvp in shopDict)
            {
                if (!kvp.Key.StartsWith("daily", StringComparison.OrdinalIgnoreCase) &&
                    !kvp.Key.StartsWith("featured", StringComparison.OrdinalIgnoreCase))
                    continue;

                var entry = kvp.Value;
                if (entry?.itemGrants == null || entry.itemGrants.Length == 0)
                    continue;

                string grant = entry.itemGrants[0];
                string[] parts = grant.Split(':');
                if (parts.Length != 2)
                    continue;

                string itemId = parts[1];

                if (!cosmeticsCache.TryGetValue(itemId, out var cosmetic))
                {
                    cosmetic = (itemId, $"https://fortnite-api.com/images/cosmetics/br/{itemId}/icon.png");
                }

                var item = new ItemShopItem
                {
                    Key = kvp.Key,
                    ItemId = itemId,
                    Price = entry.price,
                    ImageUrl = cosmetic.IconUrl,
                    DisplayName = cosmetic.Name
                };

                if (kvp.Key.StartsWith("daily", StringComparison.OrdinalIgnoreCase))
                    dailyItems.Add(item);
                else
                    featuredItems.Add(item);
            }

            DailyListView.ItemsSource = dailyItems;
            FeaturedListView.ItemsSource = featuredItems;
        }
    }
}
