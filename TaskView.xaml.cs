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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TimesheetTracker
{
    /// <summary>
    /// Interaction logic for TaskView.xaml
    /// </summary>
    public partial class TaskView : UserControl
    {
        private StackPanel _parent;
        private TaskObject _task;
        private TextBox NameBox { get; set; }
        private TextBox NumberBox { get; set; }
        private TextBox DescriptionBox { get; set;}
        private Debouncer Debouncer { get; set; } = new Debouncer(800);
        private Debouncer DebouncerUpdate { get; set; } = new Debouncer(1600);

        public TaskView(StackPanel parent)
        {
            var t = new TaskObject();
            Init(parent, t);

            Task.Run(() => t.Save());
        }

        public TaskView(StackPanel parent, TaskObject task)
        {
            Init(parent, task);
        }

        private void Init(StackPanel parent, TaskObject task)
        {
            InitializeComponent();
            Task.Run(() => PopulateUniqueProjects());
            CbDescription.Text = task.Description;
            var t = task.ExtractionTime.ToString("HH:mm");
            var d = task.ExtractionTime.ToString("MM/dd/yyyy");
            var dt = t + "  " + d;
            TbDate.Text = dt;
            TbHours.Text = task.Hours.ToString();
            CbProjectName.Text = task.ProjectName;
            CbProjectNumber.Text = task.ProjectNumber;
            _parent = parent;

            _task = task;

            CbProjectName.Loaded += (s, e) => CbLoaded(CbProjectName);
            CbProjectNumber.Loaded += (s, e) => CbLoaded(CbProjectNumber);
            CbDescription.Loaded += (s, e) => CbLoaded(CbDescription);
        }

        private void CbLoaded(ComboBox cb)
        {
            TextBox editableTextBox = FindVisualChild<TextBox>(cb);
            if (editableTextBox != null)
            {
                if(cb.Name == "CbProjectName")
                {
                    editableTextBox.TextChanged += Name_TextChanged;
                    NameBox = editableTextBox;
                }

                if (cb.Name == "CbProjectNumber")
                {
                    editableTextBox.TextChanged += Number_TextChanged;
                    NumberBox = editableTextBox;
                }

                if (cb.Name == "CbDescription")
                {
                    editableTextBox.TextChanged += Description_TextChanged;
                    DescriptionBox = editableTextBox;
                }
            }
        }

        private void Description_TextChanged(object sender, TextChangedEventArgs e)
        {
            DebouncerUpdate.Debounce(UpdateThisTask);

            var text = DescriptionBox.Text;
            if (NumberBox != null)
            {
                if (NumberBox.Text == "")
                {
                    Debouncer.Debounce(() => PopulateLikelyText(NumberBox, "description", text));
                }
            }
        }

        private void Number_TextChanged(object sender, TextChangedEventArgs e)
        {
            DebouncerUpdate.Debounce(UpdateThisTask);

            var text = NumberBox.Text;
            if(NameBox != null)
            {
                if(NameBox.Text == "")
                {
                    Debouncer.Debounce(() => PopulateLikelyText(NameBox, "name", text));
                }
            }
        }

        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            DebouncerUpdate.Debounce(UpdateThisTask);
            string text = NameBox.Text;
            if(NumberBox != null)
            {
                if(NumberBox.Text == "")
                {
                    Debouncer.Debounce(() => PopulateLikelyText(NumberBox, "number", text));
                }
            }
        }

        private void UpdateThisTask()
        {
            _task.Description =  Dispatcher.Invoke(() => CbDescription.Text);
            _task.Hours = Dispatcher.Invoke(() => double.Parse(TbHours.Text));
            _task.ProjectName = Dispatcher.Invoke(() => CbProjectName.Text);
            _task.ProjectNumber = Dispatcher.Invoke(() => CbProjectNumber.Text);
            _task.Update();
        }

        private void PopulateLikelyText(TextBox textBox, string searchColumn, string parentValue)
        {
            Task.Run(() =>
            {
                string likely = "";
                if (searchColumn == "name")
                    likely = Database.GetProjectName(parentValue);

                else if (searchColumn == "number")
                     likely = Database.GetProjectNumber(parentValue);

                else if (searchColumn == "description")
                    (likely,_) = Database.GetProjectNameAndNumber(parentValue);

                if (String.IsNullOrEmpty(likely))
                    return;

                Dispatcher.Invoke(() =>
                {
                    textBox.Text = likely;
                });
            });
            
        }


        private void PopulateUniqueProjects()
        {
            var projects = Database.ProjectNames;
            var numbers = Database.ProjectNumbers;
            var descriptions = Database.Descriptions;

            Dispatcher.Invoke(() =>
            {
                CbProjectNumber.ItemsSource = numbers;
                CbProjectName.ItemsSource = projects;
                CbDescription.ItemsSource = descriptions;
            });
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var nt = new TaskObject
            {
                Description = CbDescription.Text,
                Hours = double.Parse(TbHours.Text),
                ProjectName = CbProjectName.Text,
                ProjectNumber = CbProjectNumber.Text
            };

            nt.Save();

            var TaskView = new TaskView(_parent, nt);
            _parent.Children.Insert(0,TaskView);
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            _parent.Children.Remove(this);
            _task.Delete();
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }
    }
}
