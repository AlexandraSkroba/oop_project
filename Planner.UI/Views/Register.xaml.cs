using Planner.Data;
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
    /// Логика взаимодействия для Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        private PlannerDbContext dbContext;
        public Register()
        {
            InitializeComponent();
            dbContext = new PlannerDbContext();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            UserModel user = new UserModel
            {
                UserName = NewUserTextBox.Text,
                Password = NewPasswordBox.Password
            };

            dbContext.UsersModel.Add(user);
            dbContext.SaveChanges();

            string userName = user.UserName;
            MainWindow mainWindow = new MainWindow(user);
            this.Close();
            mainWindow.Show();
        }

        private void PrevPageButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new Login();
            this.Close();
            loginWindow.Show();
        }
    }
}
