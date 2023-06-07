using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Planner.Data;
using Planner.Data.Abstractions;
using Planner.Logic;
using Planner.UI.Views;

namespace Planner.UI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IItemModelManager itemModelManager;
        private readonly IUserModelManager userModelManager;

        private static readonly int _maxItemsPerPage = 10;
        private AddTask addTaskWindow;
        private UserModel User { get; set; }
        public int CurrentPage { get; private set; }
        public int LastPage { get; private set; }
        public IEnumerable<ItemModel> Items { get; set; }
        public bool StateSelected { get; private set; }

        public MainWindow(UserModel user)
        {
            InitializeComponent();

            this.User = user;
            string userName = user.UserName;

            this.userModelManager = new SqlUserModelManager(new PlannerDbContext());
            this.itemModelManager = new SqlItemModelManager(new PlannerDbContext(), user);

            UserName.Content = userName;
            DateTextBlock.Text = DateTime.Now.ToString("dddd, d MMMM");


            PriorityListBox.ItemsSource = new ItemModel().Priorities;
            StateComboBox.ItemsSource = new ItemModel().States;

            CurrentPage = 1;

            SaveItemCommand = new RelayCommand(obj => SaveItem(), obj =>
                !string.IsNullOrEmpty(NameTextBox.Text) &&
                PriorityListBox.SelectedItem != null
            );
            SaveButton.Command = SaveItemCommand;

            DeleteCommand = new RelayCommand(obj => DeleteItem(), obj =>
                ToDoItemsDataGrid.SelectedItem != null &&
                !StateSelected
            );
            DeleteButton.Command = DeleteCommand;

            MarkCompleteCommand = new RelayCommand(obj => MarkComplete(), obj =>
                ToDoItemsDataGrid.SelectedItem != null &&
                !StateSelected
            );
            MarkAsCompletedButton.Command = MarkCompleteCommand;

            PreviousPageCommand = new RelayCommand(obj => PreviousPage(), obj =>
                CurrentPage - 1 > 0
            );
            PreviousButton.Command = PreviousPageCommand;

            NextPageCommand = new RelayCommand(obj => NextPage(), obj =>
                CurrentPage < LastPage
            );
            NextButton.Command = NextPageCommand;

            EditTaskCommand = new RelayCommand(obj => EditTask(), obj =>
                ToDoItemsDataGrid.SelectedItem != null &&
                !StateSelected
            );
            EditTaskButton.Command = EditTaskCommand;

            NewTaskCommand = new RelayCommand(obj => NewTask());
            NewTaskButton.Command = NewTaskCommand;

            RestoreActiveCommand = new RelayCommand(obj => RestoreActive(), obj =>
            ToDoItemsDataGrid.SelectedItem != null &&
            StateSelected
            );
            RestoreAsActiveButton.Command = RestoreActiveCommand;
            User = user;
        }

        RelayCommand DeleteCommand;
        private async void DeleteItem()
        {
            if (ToDoItemsDataGrid.SelectedItem != null)
            {
                var selectedItem = (ItemModel)ToDoItemsDataGrid.SelectedItem;

                // Выполнить удаление выбранного элемента из базы данных или списка задач
                await Task.Run(() => itemModelManager.Delete(selectedItem));

                // Обновить отображение списка задач
                TasksViewRefresh();
            }
        }

        RelayCommand SaveItemCommand;
        private async void SaveItem()
        {
            if (ToDoItemsDataGrid.SelectedItem != null)
            {
                var editedItem = (ItemModel)ToDoItemsDataGrid.SelectedItem;

                editedItem.ItemName = NameTextBox.Text;
                editedItem.Priority = PriorityListBox.SelectedItem.ToString();
                editedItem.DueDate = DueDatePicker.SelectedDate;

                await Task.Run(() => itemModelManager.Update(editedItem));
            }
            else
            {
                var newItem = new ItemModel
                {
                    ItemName = NameTextBox.Text,
                    Priority = PriorityListBox.SelectedItem.ToString(),
                    IsCompleted = false,
                    DueDate = DueDatePicker.SelectedDate,
                    User = User
                };

                itemModelManager.Add(newItem);
            }

            ClearInputValues();
            TasksViewRefresh();
        }

        private void ClearInputValues()
        {
            NameTextBox.Text = string.Empty;
            PriorityListBox.SelectedItem = null;
            DueDatePicker.SelectedDate = null;
        }

        RelayCommand MarkCompleteCommand;
        private async void MarkComplete()
        {
            var itemToComplete = (ItemModel)ToDoItemsDataGrid.SelectedItem;
            itemToComplete.IsCompleted = true;

            await Task.Run(() => itemModelManager.Update(itemToComplete));
            TasksViewRefresh();
        }

        RelayCommand PreviousPageCommand;
        private void PreviousPage()
        {
            CurrentPage--;
            TasksViewRefresh();
        }

        RelayCommand NextPageCommand;
        private void NextPage()
        {
            CurrentPage++;
            TasksViewRefresh();
        }

        private int CalculateLastPage()
        {
            return (int)Math.Ceiling(itemModelManager.GetCountOfItems(state: StateSelected) / (double)_maxItemsPerPage);
        }

        private async void TasksViewRefresh()
        {
            LastPage = await Task.Run(() => CalculateLastPage());

            if (CurrentPage > LastPage &&
                CurrentPage > 1)
            {
                CurrentPage--;
            }

            Items = await Task.Run(() => itemModelManager.GetItemsByUserAndState(User, StateSelected)
            .Where(item => item.CreatedDate == DateTime.Parse(DateTextBlock.Text))
            .Skip((CurrentPage - 1) * _maxItemsPerPage)
            .Take(_maxItemsPerPage));

            RefreshPagecount();
            ToDoItemsDataGrid.ItemsSource = Items;
        }

        private void RefreshPagecount()
        {
            if (LastPage == 0)
            {
                PagecountTextBlock.Text = "No Tasks";
            }
            else
            {
                PagecountTextBlock.Text = $"{CurrentPage} of {LastPage}";
            }
        }

        private void StateComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            // Select active items by default
            StateComboBox.SelectedIndex = 0;
        }

        private void StateComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (StateComboBox.SelectedItem.ToString() == "Show Completed")
            {
                StateSelected = true;
            }
            else
            {
                StateSelected = false;
            }

            CurrentPage = 1;
            TasksViewRefresh();
        }

        RelayCommand EditTaskCommand;
        private void EditTask()
        {
            var editedItem = (ItemModel)ToDoItemsDataGrid.SelectedItem;

            addTaskWindow = new AddTask(editedItem.ItemName, editedItem.Priority, editedItem.DueDate, editedItem.CreatedDate.ToString());

            addTaskWindow.SaveButtonClicked += (itemName, priority, dueDate, createdDate) =>
            {
                editedItem.ItemName = itemName;
                editedItem.Priority = priority;
                editedItem.DueDate = dueDate;

                itemModelManager.Update(editedItem);

                TasksViewRefresh();
            };

            addTaskWindow.ShowDialog();
        }


        RelayCommand NewTaskCommand;
        private void NewTask()
        {
            ToDoItemsDataGrid.SelectedItem = null;

            var addTaskWindow = new AddTask("", "", null, DateTextBlock.Text);
            addTaskWindow.SaveButtonClicked += (itemName, priority, dueDate, createdDate) =>
            {
                var newItem = new ItemModel
                {
                    ItemName = itemName,
                    Priority = priority,
                    IsCompleted = false,
                    DueDate = dueDate,
                    CreatedDate = createdDate
                };

                itemModelManager.Add(newItem);
                TasksViewRefresh();
            };
            addTaskWindow.ShowDialog();
        }


        private void ToDoItemsDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ClearInputValues();
        }

        RelayCommand RestoreActiveCommand;
        private async void RestoreActive()
        {
            var itemToRestore = (ItemModel)ToDoItemsDataGrid.SelectedItem;
            itemToRestore.IsCompleted = false;

            await Task.Run(() => itemModelManager.Update(itemToRestore));
            TasksViewRefresh();
        }

        private void MoreInformation_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime currentDate = DateTime.Parse(DateTextBlock.Text);
            DateTime previousDate = currentDate.AddDays(-1);
            DateTextBlock.Text = previousDate.ToString("dddd, d MMMM");
            TasksViewRefresh();
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime currentDate = DateTime.Parse(DateTextBlock.Text);
            DateTime nextDate = currentDate.AddDays(1);
            DateTextBlock.Text = nextDate.ToString("dddd, d MMMM");
            TasksViewRefresh();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            this.Close();
            login.Show();
        }

        private void OnWeek_Click(object sender, RoutedEventArgs e)
        {
            WeekWindow weekWindow = new WeekWindow(Items, User);
            this.Close();
            weekWindow.Show();
        }
    }
}
