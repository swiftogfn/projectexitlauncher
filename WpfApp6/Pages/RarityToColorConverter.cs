using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfApp5.Pages
{
    public class RarityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string rarity = value as string;
            if (string.IsNullOrEmpty(rarity))
                return Brushes.Transparent;

            return rarity.ToLower() switch
            {
                "common" => Brushes.Gray,
                "uncommon" => Brushes.Green,
                "rare" => Brushes.Blue,
                "epic" => Brushes.Purple,
                "legendary" => Brushes.Orange,
                "mythic" => Brushes.Gold,
                "exotic" => Brushes.Red,
                _ => Brushes.Transparent
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
