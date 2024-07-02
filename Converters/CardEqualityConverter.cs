using AppExp.Models;
using System.Globalization;
using System.Windows.Data;

namespace AppExp.Converters
{
    public class CardEqualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Card? selectedCard = value as Card;
            Card? currentCard = parameter as Card;
            return selectedCard != null && currentCard != null && selectedCard.Name == currentCard.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

}
