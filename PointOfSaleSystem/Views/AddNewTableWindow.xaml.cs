using System;
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
using Microsoft.UI.Xaml.Navigation;
using PointOfSaleSystem.Views.ViewModels;
using PointOfSaleSystem.Services;
using PointOfSaleSystem.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddNewTableWindow : Window
    {
        private TableViewModel _viewModel { get; set; } = new TableViewModel();
        public AddNewTableWindow(TableViewModel ViewModel)
        {
            this.InitializeComponent();
            _viewModel = ViewModel;
        }

        public async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var table = new Table
            {
                Name = tableNameTextBox.Text,
                State = "empty",
                BookTime = null
            };
            var dao = Services.Services.GetKeyedSingleton<IDao>();
            dao.Tables.Create(table);
            _viewModel.Tables.Add(table);

            var dialog = new ContentDialog
            {
                Title = "Thành công",
                Content = "Bàn mới đã được thêm vào cơ sở dữ liệu",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            await dialog.ShowAsync();

            this.Close();
        }
    }
}
