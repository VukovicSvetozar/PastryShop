using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace PastryShop.Converter
{
    public class FilterOptionToImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string option = (value is System.Enum ? value.ToString() : value as string) ?? string.Empty;

            if (string.IsNullOrEmpty(option))
            {
                return DependencyProperty.UnsetValue;
            }

            string result = option switch
            {
                "All" => "/Resource/Images/Application/Filter_Status_All.png",
                "UserManagementAllUsersKey" => "/Resource/Images/Application/Filter_Status_All.png",
                "UserManagementActiveKey" => "/Resource/Images/Application/Filter_Status_Completed.png",
                "UserManagementInactiveKey" => "/Resource/Images/Application/Filter_Status_Cancelled.png",
                "ProductManagementAllProductsKey" => "/Resource/Images/Application/Filter_Product_All.png",
                "ProductManagementBakeryKey" => "/Resource/Images/Application/Filter_Product_Bakery.png",
                "ProductManagementCakeKey" => "/Resource/Images/Application/Filter_Product_Cake.png",
                "ProductManagementPastryKey" => "/Resource/Images/Application/Filter_Product_Pastry.png",
                "ProductManagementSweetKey" => "/Resource/Images/Application/Filter_Product_Sweet.png",
                "ProductManagementDrinkKey" => "/Resource/Images/Application/Filter_Product_Drink.png",
                "ProductManagementAccessoryKey" => "/Resource/Images/Application/Filter_Product_Accessory.png",
                "PosManagementAllProductsKey" => "/Resource/Images/Application/Filter_Product_All.png",
                "PosManagementBakeryKey" => "/Resource/Images/Application/Filter_Product_Bakery.png",
                "PosManagementCakeKey" => "/Resource/Images/Application/Filter_Product_Cake.png",
                "PosManagementPastryKey" => "/Resource/Images/Application/Filter_Product_Pastry.png",
                "PosManagementSweetKey" => "/Resource/Images/Application/Filter_Product_Sweet.png",
                "PosManagementDrinkKey" => "/Resource/Images/Application/Filter_Product_Drink.png",
                "PosManagementAccessoryKey" => "/Resource/Images/Application/Filter_Product_Accessory.png",
                "OrderFilterAllOrderKey" => "/Resource/Images/Application/Filter_Status_All.png",
                "OrderFilterCompletedKey" => "/Resource/Images/Application/Filter_Status_Completed.png",
                "OrderFilterCancelledKey" => "/Resource/Images/Application/Filter_Status_Cancelled.png",
                "OrderFilterOnHoldKey" => "/Resource/Images/Application/Filter_Status_On_Hold.png",
                "English" => "/Resource/Images/Application/Base_Icon_Language_English.png",
                "Serbian" => "/Resource/Images/Application/Base_Icon_Language_Serbian.png",
                "Light" => "/Resource/Images/Application/Base_Icon_Theme_Light.png",
                "Dark" => "/Resource/Images/Application/Base_Icon_Theme_Dark.png",
                "Blue" => "/Resource/Images/Application/Base_Icon_Theme_Blue.png",
                _ => string.Empty
            };

            return result;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

}