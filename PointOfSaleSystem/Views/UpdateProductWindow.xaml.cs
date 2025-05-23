﻿using System;
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
using PointOfSaleSystem.Services;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using System.Threading.Tasks;
using PointOfSaleSystem.Utils.Checkers; // Added for async/await

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// A window that allows the user to update the details of an existing product.
    /// </summary>
    public sealed partial class UpdateProductWindow : Window
    {
        Product product;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateProductWindow"/> class.
        /// Sets the product to be updated and subscribes to the Activated event.
        /// </summary>
        /// <param name="product">The product object whose details are to be updated.</param>
        /// <returns>A new instance of the UpdateProductWindow.</returns>
        public UpdateProductWindow(Product product)
        {
            this.product = product;
            this.InitializeComponent();
            this.Activated += UpdateWindowActivated;
        }

        /// <summary>
        /// Handles the Activated event of the UpdateProductWindow.
        /// Sets the DataContext for the product information when the window is activated.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">Event data.</param>
        /// <returns>This method does not return a value.</returns>
        private void UpdateWindowActivated(object sender, WindowActivatedEventArgs args)
        {
            productInfo.DataContext = product;
        }

        /// <summary>
        /// Handles the click event for the Update Product button.
        /// Retrieves updated product information from the input fields, performs validation,
        /// updates the product in the database, displays a success or error dialog,
        /// and closes the window upon successful update.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async void UpdateProduct(object sender, RoutedEventArgs e)
        {
            // Get an instance of the product repository
            IDao dao = Services.Services.GetKeyedSingleton<IDao>();
            var productRepo = dao.Products;

            // Get information from the form
            string? prodName = productName.Text != "" ? productName.Text : null;
            string? prodBrand = brand.Text != "" ? brand.Text : null;
            int? prodQuantity = null;
            int? prodCostPrice = null;
            int? prodSellingPrice = null;
            string? prodCat = category.Text != "" ? category.Text : null;
            string? prodImage = productImage.Tag != null ? productImage.Tag as string : product.Image;

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
                var catRepo = dao.Categories;
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

            // Create a new product for checking
            Product checkedProduct = new Product()
            {
                Name = prodName,
                Brand = prodBrand,
                Quantity = prodQuantity,
                CostPrice = prodCostPrice,
                SellingPrice = prodSellingPrice,
                Category = prodCat,
                Image = prodImage
            };

            // Check format
            string? res = checkedProduct.AcceptForChecking(new FormatChecker());
            if(res is not null)
            {
                dialog = new ContentDialog
                {
                    Title = "Lỗi định dạng",
                    Content = res,
                    CloseButtonText = "Đóng"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
                return;
            }

            // Check required fields
            res = checkedProduct.AcceptForChecking(new RequiredFieldChecker());
            if(res is not null)
            {
                dialog = new ContentDialog
                {
                    Title = "Nhập thiếu thông tin cần thiết",
                    Content = res,
                    CloseButtonText = "Đóng"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
                return;
            }

            // Check value
            res = checkedProduct.AcceptForChecking(new ValueChecker());
            if (res is not null)
            {
                dialog = new ContentDialog
                {
                    Title = "Nhập thông tin không hợp lệ",
                    Content = res,
                    CloseButtonText = "Đóng"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
                return;
            }

            // Update the product
            product.Name = prodName;
            product.Brand = prodBrand;
            product.Quantity = prodQuantity;
            product.CostPrice = prodCostPrice;
            product.SellingPrice = prodSellingPrice;
            product.Category = prodCat;
            product.Image = prodImage;

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
