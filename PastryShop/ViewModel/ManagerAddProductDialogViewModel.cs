using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Globalization;
using Microsoft.Win32;
using PastryShop.Command;
using PastryShop.Data.DTO;
using PastryShop.Enum;
using PastryShop.Service;
using PastryShop.Utility;

namespace PastryShop.ViewModel
{
    public partial class ManagerAddProductDialogViewModel : ValidatableBaseViewModel
    {
        private readonly IProductService _productService;

        public AddFoodProductDTO FoodProductDTO { get; set; } = new();
        public AddDrinkProductDTO DrinkProductDTO { get; set; } = new();
        public AddAccessoryProductDTO AccessoryProductDTO { get; set; } = new();

        private AddProductDTO? _currentProductDTO;
        public AddProductDTO? CurrentProductDTO
        {
            get => _currentProductDTO;
            set
            {
                _currentProductDTO = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProductType> ProductTypes { get; }

        private ProductType? _selectedProductType;
        public ProductType? SelectedProductType
        {
            get => _selectedProductType;
            set
            {
                if (SetProperty(ref _selectedProductType, value))
                {
                    ClearAllErrors();
                    UpdateVisibility();
                    ValidateSelectedProductType();
                }
            }
        }

        private bool _isFoodProductVisible;
        public bool IsFoodProductVisible
        {
            get => _isFoodProductVisible;
            set => SetProperty(ref _isFoodProductVisible, value);
        }

        private bool _isDrinkProductVisible;
        public bool IsDrinkProductVisible
        {
            get => _isDrinkProductVisible;
            set => SetProperty(ref _isDrinkProductVisible, value);
        }

        private bool _isAccessoryVisible;
        public bool IsAccessoryVisible
        {
            get => _isAccessoryVisible;
            set => SetProperty(ref _isAccessoryVisible, value);
        }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value))
                {
                    ValidateName();
                }
            }
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set
            {
                if (SetProperty(ref _description, value))
                {
                    ValidateDescription();
                }
            }
        }

        private string? _priceText;
        public string? PriceText
        {
            get => _priceText;
            set
            {
                if (SetProperty(ref _priceText, value))
                {
                    ValidatePrice();
                }
            }
        }

        private string? _selectedImagePath;
        public string? SelectedImagePath
        {
            get => _selectedImagePath;
            set
            {
                _selectedImagePath = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FoodType> FoodTypes { get; }

        private FoodType? _selectedFoodType;
        public FoodType? SelectedFoodType
        {
            get => _selectedFoodType;
            set
            {
                if (SetProperty(ref _selectedFoodType, value))
                {
                    if (SelectedProductType == ProductType.Food)
                    {
                        ValidateSelectedFoodType();
                    }
                }
            }
        }

        private string? _weightText;
        public string? WeightText
        {
            get => _weightText;
            set
            {
                if (SetProperty(ref _weightText, value))
                {
                    if (SelectedProductType == ProductType.Food)
                    {
                        ValidateWeight();
                    }
                }
            }
        }

        private string? _caloriesText;
        public string? CaloriesText
        {
            get => _caloriesText;
            set
            {
                if (SetProperty(ref _caloriesText, value))
                {
                    if (SelectedProductType == ProductType.Food)
                    {
                        ValidateCalories();
                    }
                }
            }
        }

        private string? _volumeText;
        public string? VolumeText
        {
            get => _volumeText;
            set
            {
                if (SetProperty(ref _volumeText, value))
                {
                    if (SelectedProductType == ProductType.Drink)
                    {
                        ValidateVolume();
                    }
                }
            }
        }

        private string? _materialText;
        public string? MaterialText
        {
            get => _materialText;
            set
            {
                if (SetProperty(ref _materialText, value))
                {
                    if (SelectedProductType == ProductType.Accessory)
                    {
                        ValidateMaterial();
                    };
                }
            }
        }

        private string? _dimensionsText;
        public string? DimensionsText
        {
            get => _dimensionsText;
            set
            {
                if (SetProperty(ref _dimensionsText, value))
                {
                    if (SelectedProductType == ProductType.Accessory)
                    {
                        ValidateDimensions();
                    }
                }
            }
        }

        public ICommand AddCommand { get; }
        public ICommand BrowseImageCommand { get; }

        public ManagerAddProductDialogViewModel(IProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));

            ProductTypes =
            [
                ProductType.Food,
                ProductType.Drink,
                ProductType.Accessory
            ];
            SelectedProductType = null;

            FoodTypes =
            [
                FoodType.Pastry,
                FoodType.Sweet,
                FoodType.Bakery,
                FoodType.Cake
            ];
            SelectedFoodType = null;

            AddCommand = new AsyncRelayCommand(_ => AddProduct());

            BrowseImageCommand = new AsyncRelayCommand(_ => BrowseImage());

        }

        private void UpdateVisibility()
        {
            IsFoodProductVisible = SelectedProductType == ProductType.Food;
            IsDrinkProductVisible = SelectedProductType == ProductType.Drink;
            IsAccessoryVisible = SelectedProductType == ProductType.Accessory;

            CurrentProductDTO = SelectedProductType switch
            {
                ProductType.Food => FoodProductDTO,
                ProductType.Drink => DrinkProductDTO,
                ProductType.Accessory => AccessoryProductDTO,
                _ => throw new InvalidOperationException("Nepoznat tip proizvoda.")
            };

            if (SelectedProductType == ProductType.Food)
            {
                FoodProductDTO.ProductType = ProductType.Food;
            }
            else if (SelectedProductType == ProductType.Drink)
            {
                DrinkProductDTO.ProductType = ProductType.Drink;
            }
            else if (SelectedProductType == ProductType.Accessory)
            {
                AccessoryProductDTO.ProductType = ProductType.Accessory;
            }
        }

        public void Validate()
        {
            ValidateSelectedProductType();
            ValidateName();
            ValidateDescription();
            ValidatePrice();
            if (SelectedProductType == ProductType.Food)
            {
                ValidateSelectedFoodType();
                ValidateWeight();
                ValidateCalories();
            }
            else if (SelectedProductType == ProductType.Drink)
            {
                ValidateVolume();
            }
            else if (SelectedProductType == ProductType.Accessory)
            {
                ValidateMaterial();
                ValidateDimensions();
            }
        }

        private void ValidateSelectedProductType()
        {
            ClearErrors(nameof(SelectedProductType));
            if (SelectedProductType == null)
            {
                AddError(nameof(SelectedProductType), GetLocalizedString("ValidateProductTypeMustSelectMessage"));
            }
        }

        private void ValidateName()
        {
            ClearErrors(nameof(Name));

            const int minLength = 2;
            const int maxLength = 50;

            if (string.IsNullOrWhiteSpace(Name))
            {
                AddError(nameof(Name), GetLocalizedString("ValidateCannotBeEmptyMessage"));
            }
            else if (Name.Length < minLength)
            {
                AddError(nameof(Name), string.Format(GetLocalizedString("ValidateLeastCharactersLongMessage"), minLength));
            }
            else if (Name.Length > maxLength)
            {
                AddError(nameof(Name), string.Format(GetLocalizedString("ValidateMoreThanCharactersLongMessage"), maxLength));
            }
        }

        private void ValidateDescription()
        {
            ClearErrors(nameof(Description));

            const int minLength = 2;
            const int maxLength = 200;

            if (string.IsNullOrWhiteSpace(Description))
            {
                AddError(nameof(Description), GetLocalizedString("ValidateCannotBeEmptyMessage"));
            }
            else if (Description.Length < minLength)
            {
                AddError(nameof(Description), string.Format(GetLocalizedString("ValidateLeastCharactersLongMessage"), minLength));
            }
            else if (Description.Length > maxLength)
            {
                AddError(nameof(Description), string.Format(GetLocalizedString("ValidateMoreThanCharactersLongMessage"), maxLength));
            }
        }

        private void ValidatePrice()
        {
            ClearErrors(nameof(PriceText));

            if (string.IsNullOrWhiteSpace(PriceText))
            {
                AddError(nameof(PriceText), GetLocalizedString("ValidateCannotBeEmptyMessage"));
            }
            else if (!decimal.TryParse(PriceText, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsedPrice))
            {
                AddError(nameof(PriceText), GetLocalizedString("ValidateMustBeNumberMessage"));
            }
            else if (parsedPrice < 0)
            {
                AddError(nameof(PriceText), GetLocalizedString("ValidateCannotBeNegativeMessage"));
            }
        }

        private void ValidateSelectedFoodType()
        {
            ClearErrors(nameof(SelectedFoodType));
            if (SelectedFoodType == null)
            {
                AddError(nameof(SelectedFoodType), GetLocalizedString("ValidateFoodTypeMustSelectMessage"));
            }
        }

        private void ValidateWeight()
        {
            ClearErrors(nameof(WeightText));

            if (string.IsNullOrWhiteSpace(WeightText))
            {
                AddError(nameof(WeightText), GetLocalizedString("ValidateCannotBeEmptyMessage"));
            }
            else if (!decimal.TryParse(WeightText, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsedWeight))
            {
                AddError(nameof(WeightText), GetLocalizedString("ValidateMustBeNumberMessage"));
            }
            else if (parsedWeight < 0)
            {
                AddError(nameof(WeightText), GetLocalizedString("ValidateCannotBeNegativeMessage"));

            }
        }

        private void ValidateCalories()
        {
            ClearErrors(nameof(CaloriesText));

            if (string.IsNullOrWhiteSpace(CaloriesText))
            {
                return;
            }
            if (!int.TryParse(CaloriesText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedCalories))
            {
                AddError(nameof(CaloriesText), GetLocalizedString("ValidateWholeNumberMessage"));
            }
            else if (parsedCalories < 0)
            {
                AddError(nameof(CaloriesText), GetLocalizedString("ValidateCannotBeNegativeMessage"));
            }
        }

        private void ValidateVolume()
        {
            ClearErrors(nameof(VolumeText));

            if (string.IsNullOrWhiteSpace(VolumeText))
            {
                return;
            }
            if (!decimal.TryParse(VolumeText, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsedVolume))
            {
                AddError(nameof(VolumeText), GetLocalizedString("ValidateMustBeNumberMessage"));
            }
            else if (parsedVolume < 0)
            {
                AddError(nameof(VolumeText), GetLocalizedString("ValidateCannotBeNegativeMessage"));
            }
        }

        private void ValidateMaterial()
        {
            ClearErrors(nameof(MaterialText));

            if (string.IsNullOrWhiteSpace(MaterialText))
            {
                AddError(nameof(MaterialText), GetLocalizedString("ValidateCannotBeEmptyMessage"));
            }
        }

        private void ValidateDimensions()
        {
            ClearErrors(nameof(DimensionsText));
            if (string.IsNullOrWhiteSpace(DimensionsText))
            {
                return;
            }
            if (!ValidDimensionsRegex.IsMatch(DimensionsText))
            {
                AddError(nameof(DimensionsText), GetLocalizedString("ValidateIncorrectFormatMessage"));
            }
        }

        private async Task AddProduct()
        {
            try
            {
                Validate();

                if (HasErrors)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningInvalidFieldValuesMessage"));
                    return;
                }

                CurrentProductDTO!.Name = Name;
                CurrentProductDTO.Description = Description;
                CurrentProductDTO.Price = decimal.Parse(PriceText!, CultureInfo.InvariantCulture);
                CurrentProductDTO.ImagePath = SelectedImagePath;

                if (SelectedProductType == ProductType.Food)
                {
                    FoodProductDTO.FoodType = SelectedFoodType;
                    FoodProductDTO.Weight = decimal.TryParse(WeightText, NumberStyles.Number,
                        CultureInfo.InvariantCulture, out decimal parsedWeight)
                        ? parsedWeight
                        : (decimal?)null;
                    FoodProductDTO.Calories = int.TryParse(CaloriesText, NumberStyles.Integer,
                        CultureInfo.InvariantCulture, out int parsedCalories)
                        ? parsedCalories
                        : (int?)null;

                    await _productService.CreateProduct(FoodProductDTO);
                }

                if (SelectedProductType == ProductType.Drink)
                {
                    DrinkProductDTO.Volume = decimal.TryParse(VolumeText, NumberStyles.Number,
                        CultureInfo.InvariantCulture, out decimal parsedVolume)
                        ? parsedVolume
                        : (decimal?)null;

                    await _productService.CreateProduct(DrinkProductDTO);
                }

                if (SelectedProductType == ProductType.Accessory)
                {
                    AccessoryProductDTO.Material = MaterialText!;
                    AccessoryProductDTO.Dimensions = DimensionsText;

                    await _productService.CreateProduct(AccessoryProductDTO);

                }

                ShowInfoDialog(GetLocalizedString("DialogInfoProductAddedMessage"));

                CloseWindow();

            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška pri dodavanju proizvoda: {ex.Message}", ex);
            }
        }

        private async Task BrowseImage()
        {
            await Task.Run(() =>
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png",
                    Title = "Select Profile Image"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        string destinationFolder = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "PastryShop",
                            "Resource",
                            "Images",
                            "Products");

                        if (!Directory.Exists(destinationFolder))
                            Directory.CreateDirectory(destinationFolder);

                        string fileName = Path.GetFileName(openFileDialog.FileName);
                        string destinationPath = Path.Combine(destinationFolder, fileName);

                        if (File.Exists(destinationPath))
                        {
                            string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                            string extension = Path.GetExtension(fileName);
                            fileName = $"{nameWithoutExt}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                            destinationPath = Path.Combine(destinationFolder, fileName);
                        }

                        File.Copy(openFileDialog.FileName, destinationPath, true);

                        SelectedImagePath = fileName;
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Logger.LogError("Nemate dozvolu za kopiranje fajla. " + ex.Message, ex);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Došlo je do greške: " + ex.Message, ex);
                    }
                }
            });
        }

        private static void CloseWindow()
        {
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            if (window != null)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

    }
}