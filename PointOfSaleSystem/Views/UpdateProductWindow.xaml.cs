using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using PointOfSaleSystem.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UpdateProductWindow : Window
    {
        Product product;
        public UpdateProductWindow(Product product)
        {
            this.product = product;
            this.InitializeComponent();
            this.Activated += UpdateWindowActivated;
        }

        private void UpdateWindowActivated(object sender, WindowActivatedEventArgs args)
        {
            productInfo.DataContext = product;
        }

        public async void UpdateProduct(object sender, RoutedEventArgs e)
        {
            // Get an instance of the product repository
            ProductRepository productRepo = ProductRepository.GetInstance();

            // Get information from the form
            string? prodName = productName.Text != "" ? productName.Text : null;
            string? prodBrand = brand.Text != "" ? brand.Text : null;
            int? prodQuantity = null;
            int? prodCostPrice = null;
            int? prodSellingPrice = null;
            string? prodCat = category.Text != "" ? category.Text : null;

            // Constraint checking
            ContentDialog dialog;
            if (prodName == null)
            {
                dialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Tên sản phẩm không được để trống.",
                    CloseButtonText = "Đóng"
                };

                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
                return;
            }

            try
            {
                if (quantity.Text != "")
                {
                    prodQuantity = int.Parse(quantity.Text);
                }
            }
            catch (FormatException ex)
            {
                dialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Số lượng sản phẩm tồn kho phải là một số nguyên.",
                    CloseButtonText = "Đóng"
                };

                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();

                return;
            }

            try
            {
                if (costPrice.Text != "")
                {
                    prodCostPrice = int.Parse(costPrice.Text);
                }
            }
            catch (FormatException ex)
            {
                dialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Giá vốn phải là một số nguyên.",
                    CloseButtonText = "Đóng"
                };

                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();

                return;
            }

            try
            {
                if (sellingPrice.Text != "")
                {
                    prodSellingPrice = int.Parse(sellingPrice.Text);
                }
            }
            catch (FormatException ex)
            {
                dialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Giá bán phải là một số nguyên.",
                    CloseButtonText = "Đóng"
                };

                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();

                return;
            }

            // If the category is detailed, add it to the database 
            if (prodCat != null)
            {
                CategoryRepository catRepo = CategoryRepository.GetInstance();
                var categories = catRepo.GetAll();
                Category? foundCat = categories.FirstOrDefault(cat => cat.Name == prodCat);
                if (foundCat == null)
                {
                    Category newCat = new Category()
                    {
                        Name = prodCat,
                    };
                    catRepo.Create(newCat);
                }
            }

            // Update the product
            product.Name = prodName;
            product.Brand = prodBrand;
            product.Quantity = prodQuantity;
            product.CostPrice = prodCostPrice;
            product.SellingPrice = prodSellingPrice;
            product.Category = prodCat;
            product.Image = productImage.Tag as string;

            // Add the product to the database
            productRepo.Update(product);

            dialog = new ContentDialog
            {
                Title = "Thành công",
                Content = "Sản phẩm đã được chỉnh sửa trong cơ sở dữ liệu.",
                CloseButtonText = "Đóng"
            };

            dialog.XamlRoot = this.Content.XamlRoot;
            await dialog.ShowAsync();
            this.Close();
        }

        public async void AddPhoto(object sender, RoutedEventArgs e)
        {
            // Create a FileOpenPicker
            var picker = new FileOpenPicker();

            // Make sure the picker belongs to the AddProductWindow window
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            // Configure the file picker
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            // Only permit image files
            picker.FileTypeFilter.Clear();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");

            // Open the picker for choosing file
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Get the file absolute path
                string absolutePath = file.Path;

                // Show image
                var bitmapImage = new BitmapImage(new Uri(absolutePath));
                productImage.Source = bitmapImage;
                productImage.Tag = absolutePath;
            }
        }
    }
}
