using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Globalization;
using Microsoft.Win32;
using PastryShop.Service;
using PastryShop.Data.DTO;
using PastryShop.Command;
using PastryShop.Enum;
using PastryShop.Utility;

namespace PastryShop.ViewModel
{
    public partial class ManagerEditProductProfileDialogViewModel : ValidatableBaseViewModel
    {
        private readonly IProductService _productService;

        public IEnumerable<FoodType> FoodTypes { get; } = System.Enum.GetValues(typeof(FoodType)).Cast<FoodType>();

        public EditProductProfileDTO ProductProfile { get; set; }

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

        private string? _weightText;
        public string? WeightText
        {
            get => _weightText;
            set
            {
                if (SetProperty(ref _weightText, value))
                {
                    if (ProductProfile?.ProductType == ProductType.Food)
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
                    if (ProductProfile?.ProductType == ProductType.Food)
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
                    if (ProductProfile?.ProductType == ProductType.Drink)
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
                    if (ProductProfile?.ProductType == ProductType.Accessory)
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
                    if (ProductProfile?.ProductType == ProductType.Accessory)
                    {
                        ValidateDimensions();
                    }
                }
            }
        }

        public ICommand BrowseImageCommand { get; }
        public ICommand SaveCommand { get; }

        public ManagerEditProductProfileDialogViewModel(EditProductProfileDTO productProfile, IProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            ProductProfile = productProfile ?? throw new ArgumentNullException(nameof(productProfile));

            Name = ProductProfile.Name;
            Description = ProductProfile.Description;
            SelectedImagePath = ProductProfile.ImagePath;

            WeightText = ProductProfile.ProductType == ProductType.Food
                ? ProductProfile.Weight?.ToString(CultureInfo.InvariantCulture)
                : string.Empty;
            CaloriesText = ProductProfile.ProductType == ProductType.Food
                ? ProductProfile.Calories?.ToString(CultureInfo.InvariantCulture)
                : string.Empty;

            VolumeText = ProductProfile.ProductType == ProductType.Drink
                ? ProductProfile.Volume?.ToString(CultureInfo.InvariantCulture)
                : string.Empty;

            MaterialText = ProductProfile.ProductType == ProductType.Accessory
                ? ProductProfile.Material
                : string.Empty;
            DimensionsText = ProductProfile.ProductType == ProductType.Accessory
                ? ProductProfile.Dimensions
                : string.Empty;

            BrowseImageCommand = new AsyncRelayCommand(_ => BrowseImage());
            SaveCommand = new AsyncRelayCommand(_ => SaveProductProfile());
        }

        public void Validate()
        {
            ValidateName();
            ValidateDescription();
            if (ProductProfile.ProductType == ProductType.Food)
            {
                ValidateWeight();
                ValidateCalories();
            }
            if (ProductProfile.ProductType == ProductType.Drink)
            {
                ValidateVolume();
            }
            if (ProductProfile.ProductType == ProductType.Accessory)
            {
                ValidateMaterial();
                ValidateDimensions();
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

        private async Task SaveProductProfile()
        {
            try
            {
                Validate();

                if (HasErrors)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningInvalidFieldValuesMessage"));
                    return;
                }

                ProductProfile.Name = Name;
                ProductProfile.Description = Description;
                ProductProfile.ImagePath = SelectedImagePath;


                if (ProductProfile.ProductType == ProductType.Food)
                {
                    ProductProfile.Weight = decimal.TryParse(WeightText, NumberStyles.Number,
                        CultureInfo.InvariantCulture, out decimal parsedWeight)
                        ? parsedWeight
                        : (decimal?)null;

                    ProductProfile.Calories = int.TryParse(CaloriesText, NumberStyles.Integer,
                        CultureInfo.InvariantCulture, out int parsedCalories)
                        ? parsedCalories
                        : (int?)null;
                }

                if (ProductProfile.ProductType == ProductType.Drink)
                {
                    ProductProfile.Volume = decimal.TryParse(VolumeText, NumberStyles.Number,
                        CultureInfo.InvariantCulture, out decimal parsedVolume)
                        ? parsedVolume
                        : (decimal?)null;
                }

                if (ProductProfile.ProductType == ProductType.Accessory)
                {
                    ProductProfile.Material = MaterialText;
                    ProductProfile.Dimensions = DimensionsText;
                }

                await _productService.EditProductProfile(ProductProfile);

                ShowInfoDialog(GetLocalizedString("DialogInfoProductEditProfileSuccessfullyMessage"));

                CloseWindow();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom ažuriranja: {ex.Message}", ex);
            }
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