using System;
using System.IO.Pipes;
using System.Windows;
using Planner.Logic;

namespace Planner.UI
{
    /// <summary>
    /// Логика взаимодействия для AddTask.xaml
    /// </summary>
    public partial class AddTask : Window
    {
        public event Action<string, string, DateTime?, DateTime> SaveButtonClicked;
        public event EventHandler<ItemEventArgs> ItemAdded;
        public ItemModel AddedItem { get; private set; }
        private string createdDate;

        public AddTask(string itemName, string priority, DateTime? dueDate, string createdDate)
        {
            InitializeComponent();
            PriorityListBox.ItemsSource = new ItemModel().Priorities;

            // Установите переданные данные таски в элементы формы
            NameTextBox.Text = itemName;
            PriorityListBox.SelectedItem = priority;
            DueDatePicker.SelectedDate = dueDate;
            this.createdDate = createdDate;
        }

        protected virtual void OnItemAdded(ItemModel item)
        {
            ItemAdded?.Invoke(this, new ItemEventArgs(item));
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string itemName = NameTextBox.Text;
            string priority = PriorityListBox.SelectedItem.ToString();
            DateTime? dueDate = DueDatePicker.SelectedDate;

            SaveButtonClicked?.Invoke(itemName, priority, dueDate, DateTime.Parse(createdDate));
            AddedItem = new ItemModel
            {
                ItemName = itemName,
                Priority = priority,
                IsCompleted = false,
                DueDate = dueDate,
                CreatedDate = DateTime.Parse(createdDate)
            };

            OnItemAdded(AddedItem);

            Close();
        }

    }
}
