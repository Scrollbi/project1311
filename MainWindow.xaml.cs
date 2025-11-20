using Microsoft.Win32;
using project2.Models;
using project2.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace project2
{
    public partial class MainWindow : Window
    {
        private User _currentUser;
        private ProductManager productManager;
        private ImageHandler imageHandler;
        private ProductEditWindow _currentEditWindow;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            productManager = new ProductManager(user);
            imageHandler = new ImageHandler();
            txtUser.Text = "Пользователь: " + user.FullName;
            LoadProducts();
        }

        private void LoadProducts()
        {
            var products = productManager.LoadProducts(imageHandler);
            productsGrid.ItemsSource = products;

            var manufacturers = productManager.GetManufacturers();
            cmbManufacturer.ItemsSource = manufacturers;
            cmbManufacturer.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            string searchText = txtSearch.Text;
            string selectedManufacturer = cmbManufacturer.SelectedItem?.ToString();
            var filtered = productManager.ApplyFilters(searchText, selectedManufacturer);
            productsGrid.ItemsSource = filtered;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void cmbManufacturer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void btnSort_Click(object sender, RoutedEventArgs e)
        {
            var products = productsGrid.ItemsSource as List<ProductDisplay>;
            if (products != null)
            {
                productsGrid.ItemsSource = productManager.SortProducts(products);
                productManager.ToggleSortDirection();
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }

        private void BtnChangeImage_Click(object sender, RoutedEventArgs e)
        {
            if (productsGrid.SelectedItem is ProductDisplay selectedProduct)
            {
                ChangeProductImage(selectedProduct);
            }
            else
            {
                MessageBox.Show("Выберите товар для изменения фото");
            }
        }

        private void productsGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (productsGrid.SelectedItem is ProductDisplay selectedProduct)
            {
                ChangeProductImage(selectedProduct);
            }
        }

        private void ChangeProductImage(ProductDisplay product)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Картинки|*.jpg;*.jpeg;*.png";

            if (dialog.ShowDialog() == true)
            {
                productManager.UpdateProductImage(product.Id, dialog.FileName);
                LoadProducts();
                MessageBox.Show("Фото обновлено");
            }
        }

       

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            OpenProductEditWindow(null);
        }

        private void BtnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (productsGrid.SelectedItem is ProductDisplay selectedProduct)
            {
                var product = productManager.GetProductById(selectedProduct.Id);
                if (product != null)
                {
                    OpenProductEditWindow(product);
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для редактирования", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (productsGrid.SelectedItem is ProductDisplay selectedProduct)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить товар \"{selectedProduct.Name}\"?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var (success, message) = productManager.DeleteProduct(selectedProduct.Id);
                    if (success)
                    {
                        MessageBox.Show(message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadProducts(); 
                    }
                    else
                    {
                        MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OpenProductEditWindow(Product product)
        {
           
            if (_currentEditWindow != null && _currentEditWindow.IsLoaded)
            {
                _currentEditWindow.Focus();
                return;
            }

            _currentEditWindow = new ProductEditWindow(product);
            _currentEditWindow.Owner = this;

            if (_currentEditWindow.ShowDialog() == true)
            {
               
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    LoadProducts();
                }), System.Windows.Threading.DispatcherPriority.Background);
            }

            _currentEditWindow = null;
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}