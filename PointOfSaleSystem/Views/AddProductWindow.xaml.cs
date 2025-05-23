﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using PointOfSaleSystem.Models;
using Windows.Storage.Pickers;
using System.Security.Cryptography.X509Certificates;
using System.Numerics;
using Windows.ApplicationModel.Activation;
using PointOfSaleSystem.Views.ViewModels;
using PointOfSaleSystem.Services;
using PointOfSaleSystem.Utils.Checkers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// A window that allows the user to add a new product to the system.
    /// </summary>
    public sealed partial class AddProductWindow : Window
    {
        private ProductViewModel _productViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddProductWindow"/> class.
        /// </summary>
        /// <param name="ViewModel">The ProductViewModel used to manage product data.</param>
        public AddProductWindow(ProductViewModel ViewModel)
        {
            this.InitializeComponent();
            _productViewModel = ViewModel;
        }

        /// <summary>
        /// Handles the click event for the Add Product button.
        /// Retrieves product information from the input fields, performs validation,
        /// adds the new product to the database, displays a success or error dialog,
        /// and closes the window upon successful addition.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async void AddProduct(object sender, RoutedEventArgs e)
        {
            // Get an instance of the database access object
            IDao dao = Services.Services.GetKeyedSingleton<IDao>();

            // Get information from the form
            string? prodName = productName.Text != "" ? productName.Text : null;
            string? prodBrand = brand.Text != "" ? brand.Text : null;
            int? prodQuantity = null;
            int? prodCostPrice = null;
            int? prodSellingPrice = null;
            string? prodCat = category.Text != "" ? category.Text : null;

            // Constraint checking
            ContentDialog dialog;
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
                    Title = "Lỗi giá trị",
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
                    Title = "Lỗi giá trị",
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
                    Title = "Lỗi giá trị",
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
                var categories = dao.Categories.GetAll();
                Category? foundCat = categories.FirstOrDefault(cat => cat.Name == prodCat);
                if (foundCat == null)
                {
                    Category newCat = new Category()
                    {
                        Name = prodCat,
                    };
                    dao.Categories.Create(newCat);
                }
            }

            // Creating the product
            Product product = new Product()
            {
                Name = prodName,
                Brand = prodBrand,
                Quantity = prodQuantity,
                CostPrice = prodCostPrice,
                SellingPrice = prodSellingPrice,
                Category = prodCat,
                Image = productImage.Tag as string
            };

            // Check format
            var checkRes = product.AcceptForChecking(new FormatChecker());
            if(checkRes is not null)
            {
                dialog = new ContentDialog
                {
                    Title = "Lỗi định dạng",
                    Content = checkRes,
                    CloseButtonText = "Đóng"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
                return;
            }

            // Check required field
            checkRes = product.AcceptForChecking(new RequiredFieldChecker());
            if(checkRes is not null)
            {
                dialog = new ContentDialog
                {
                    Title = "Nhập thiếu thông tin cần thiết",
                    Content = checkRes,
                    CloseButtonText = "Đóng"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
                return;
            }

            // Check value
            checkRes = product.AcceptForChecking(new ValueChecker());
            if (checkRes is not null)
            {
                dialog = new ContentDialog
                {
                    Title = "Nhập thông tin không hợp lệ",
                    Content = checkRes,
                    CloseButtonText = "Đóng"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
                return;
            }

            // Add the product to the database
            dao.Products.Create(product);
            // Update real time viewModel
            _productViewModel.Products.Add(product);

            dialog = new ContentDialog
            {
                Title = "Thành công",
                Content = "Sản phẩm đã được thêm vào cơ sở dữ liệu.",
                CloseButtonText = "Đóng"
            };

            dialog.XamlRoot = this.Content.XamlRoot;
            await dialog.ShowAsync();
            this.Close();
        }

         /// <summary>
        /// Handles the click event for the Add Photo button.
        /// Opens a file picker to allow the user to select an image file,
        /// displays the selected image in the UI, and stores its absolute path.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
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