using Planner.Data;
using Planner.Logic;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Planner.UI.Views
{
    /// <summary>
    /// Логика взаимодействия для WeekWindow.xaml
    /// </summary>
    public partial class WeekWindow : Window
    {
        private IEnumerable<ItemModel> weekItems;
        private UserModel userModel;
        public WeekWindow(IEnumerable<ItemModel> items, UserModel user)
        {
            InitializeComponent();
            SetWeekDates();
            this.weekItems = items;
            this.userModel = user;
        }

        private void SetWeekDates()
        {
            DateTime currentDate = DateTime.Today;
            int currentDayOfWeek = (int)currentDate.DayOfWeek;

            // Start the week on Monday (1) and end on Sunday (7)
            int startDayOfWeek = 1;
            int endDayOfWeek = 7;

            // Calculate the date for the start of the current week
            DateTime startDate = currentDate.AddDays(-currentDayOfWeek + startDayOfWeek);

            for (int i = startDayOfWeek; i <= endDayOfWeek; i++)
            {
                Label dateLabel = FindName("DateLabel" + i) as Label;
                if (dateLabel != null)
                {
                    dateLabel.Content = startDate.AddDays(i - startDayOfWeek).ToString("dd MMMM");
                }
            }
        }

        private void OnDay_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(userModel);
            this.Close();
            mainWindow.Show();
        }
    }
}
