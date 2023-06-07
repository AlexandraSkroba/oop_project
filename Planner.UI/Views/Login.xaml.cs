using Microsoft.Win32;
using Planner.Data;
using Planner.Data.Abstractions;
using Planner.Logic;
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

namespace Planner.UI.Views
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new Register();
            this.Close();
            registerWindow.Show();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string userName = UserName.Text;
            string password = Password.Password;

            // Проверка наличия пользователя с заданным именем пользователя и паролем в базе данных
            bool isUserValid = CheckUserCredentials(userName, password);

            if (isUserValid)
            {
                using (var dbContext = new PlannerDbContext())
                {
                    // Получите пользователя из базы данных
                    UserModel user = dbContext.UsersModel.FirstOrDefault(u => u.UserName == userName && u.Password == password);

                    // Открытие окна MainWindow
                    MainWindow mainWindow = new MainWindow(user);
                    mainWindow.Show();

                    // Закрытие текущего окна
                    this.Close();
                }
            }
            else
            {
                UserName.Text = "";
                Password.Password = "";
                MessageBox.Show("Неверное имя пользователя или пароль");
            }
        }

        private bool CheckUserCredentials(string userName, string password)
        {
            using (var dbContext = new PlannerDbContext())
            {
                // Проверка наличия пользователя с заданным именем пользователя и паролем в базе данных
                var user = dbContext.UsersModel.FirstOrDefault(u => u.UserName == userName && u.Password == password);

                if (user != null)
                {
                    return true; // Пользователь найден
                }
                else
                {
                    return false; // Пользователь не найден
                }
            }
        }

    }
}
