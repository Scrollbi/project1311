using project2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace project2
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        readonly Oshkinng207b2Context _context;
        public LoginWindow()
        {
            InitializeComponent();
            _context = new Oshkinng207b2Context();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text, password = txtPassword.Password; 
            var user = _context.Users.FirstOrDefault(u => u.Login == login && u.Password== password);
            if (user != null)
            {
                MainWindow main = new MainWindow(user);
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль");
            }
        }
       
    }
}
