using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSaleSystem.Models;
using PointOfSaleSystem.Services;
using PointOfSaleSystem.Utils;

namespace PointOfSaleSystem.Views.ViewModels
{
    public class OrderViewModel : INotifyPropertyChanged
    {
        public FullObservableCollection<Order> Orders { get; set; }
        public ObservableCollection<Product> OrderProducts { get; set; } = new ObservableCollection<Product>();
        private Product? _selectedProduct;
        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                RaisePropertyChanged(nameof(SelectedProduct));
            }
        }
        public int Total => OrderProducts?.Sum(p => (p.SellingPrice ?? 0) * (p.Quantity ?? 0)) ?? 0;

        public OrderViewModel()
        {
            var dao = Services.Services.GetKeyedSingleton<IDao>();
            var orderRepository = dao.Orders;
            Orders = new FullObservableCollection<Order>(orderRepository.GetAll());
            // Lắng nghe thay đổi trong danh sách OrderProducts
            OrderProducts.CollectionChanged += OnOrderProductsChanged;

            // Ghi ra giá trị hiện tại của Total
            Debug.WriteLine($"Total: {Total}");
        }

        private void OnOrderProductsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // Lắng nghe sự kiện PropertyChanged của các sản phẩm được thêm vào
            if (e.NewItems != null)
            {
                foreach (Product product in e.NewItems)
                {
                    product.PropertyChanged += OnProductPropertyChanged;
                }
            }

            // Ngừng lắng nghe sự kiện PropertyChanged của các sản phẩm bị xóa
            if (e.OldItems != null)
            {
                foreach (Product product in e.OldItems)
                {
                    product.PropertyChanged -= OnProductPropertyChanged;
                }
            }

            // Cập nhật Total
            RaisePropertyChanged(nameof(Total));
            // Ghi ra giá trị hiện tại của Total
            Debug.WriteLine($"Total: {Total}");
        }

        private void OnProductPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Nếu Quantity hoặc SellingPrice thay đổi, cập nhật Total
            if (e.PropertyName == nameof(Product.Quantity) || e.PropertyName == nameof(Product.SellingPrice))
            {
                RaisePropertyChanged(nameof(Total));
            }
        }

        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();
        private Category? _selectedCategory;
        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                RaisePropertyChanged(nameof(SelectedCategory));
                FilterProductsByCategory(); // Lọc sản phẩm khi Category thay đổi
            }
        }

        public void LoadCategories()
        {
            IDao dao = Services.Services.GetKeyedSingleton<IDao>();
            var categories = dao.Categories.GetAll();
            Categories.Clear();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }

        public ObservableCollection<Product> AllProducts { get; set; } = new ObservableCollection<Product>(); // Tất cả sản phẩm
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>(); // Sản phẩm được lọc
        public void LoadProducts()
        {
            IDao dao = Services.Services.GetKeyedSingleton<IDao>();
            var products = dao.Products.GetAll();
            AllProducts.Clear();
            foreach (var product in products)
            {
                AllProducts.Add(product);
            }

            // Hiển thị tất cả sản phẩm ban đầu
            FilterProductsByCategory();
        }

        private void FilterProductsByCategory()
        {
            Products.Clear();
            if (SelectedCategory == null)
            {
                // Nếu không có Category nào được chọn, hiển thị tất cả sản phẩm
                foreach (var product in AllProducts)
                {
                    Products.Add(product);
                }
            }
            else
            {
                // Lọc sản phẩm theo Category được chọn
                foreach (var product in AllProducts.Where(p => p.Category == SelectedCategory.Name))
                {
                    Products.Add(product);
                }
            }
        }

        public void AddProductToOrder(Product product)
        {
            // Kiểm tra nếu sản phẩm hết hàng hoặc không có giá bán
            if (product.Quantity <= 0)
            {
                Debug.WriteLine($"Không thể thêm sản phẩm '{product.Name}' vì đã hết hàng.");
                return;
            }

            if (product.SellingPrice == null)
            {
                Debug.WriteLine($"Không thể thêm sản phẩm '{product.Name}' vì không có giá bán.");
                return;
            }

            var existingProduct = OrderProducts.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct != null)
            {
                return;
            }
            else
            {
                OrderProducts.Add(new Product
                {
                    Id = product.Id,
                    Name = product.Name,
                    SellingPrice = product.SellingPrice ?? 0,
                    Quantity = 1
                });
            }

            RaisePropertyChanged(nameof(Total));
            // Ghi ra giá trị hiện tại của Total
            Debug.WriteLine($"Total: {Total}");
        }

        // Phương thức hỗ trợ Fody để thông báo thay đổi
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public Customer? SelectedCustomer { get; set; }

        public void SetCustomer(Customer customer)
        {
            SelectedCustomer = customer;
            RaisePropertyChanged(nameof(SelectedCustomer));
        }
        
        public Order CreateOrder()

        {
            if (OrderProducts.Count == 0)
            {
                throw new InvalidOperationException("Không thể tạo đơn hàng trống.");
            }

            // Tạo đối tượng Order
            var newOrder = new Order
            {
                CustomerId = SelectedCustomer?.Id,
                OrderTime = DateTime.Now,
                Discount = 0,
                TotalPrice = Total,
                IsPaid = false,
            };

            // Lưu Order vào cơ sở dữ liệu
            var dao = Services.Services.GetKeyedSingleton<IDao>();
            dao.Orders.Create(newOrder);
            if (newOrder.Id <= 0)
            {
                throw new Exception("Lỗi: không lấy được OrderId sau khi tạo Order.");
            }

            //Tạo các OrderDetail tương ứng
            foreach (var product in OrderProducts)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = newOrder.Id,
                    ProductId = product.Id,
                    Quantity = product.Quantity ?? 0,
                };

                // Lưu OrderDetail vào cơ sở dữ liệu
                dao.OrderDetails.Create(orderDetail);
            }

            // Xóa danh sách sản phẩm trong đơn hàng sau khi thanh toán
            OrderProducts.Clear();

            // Cập nhật lại Total
            RaisePropertyChanged(nameof(Total));

            return newOrder;
        }
    }
}
