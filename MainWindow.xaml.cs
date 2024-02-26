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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // subscribe to the on open or on show event
            Loaded += Window_Loaded;
            ShowInTaskbar = false;
        }

        // override the on open or on show event
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(TaskStack.Children.Count == 0)
            {
                Task.Run(() =>
                {
                    var latestTasks = Database.LoadLatest(30);
                    // group tasks by ExtractionTime
                    if (latestTasks.Count > 0)
                    {
                        foreach (var task in latestTasks)
                        {
                            if (task.ExtractionTime.Date == DateTime.Now.Date)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    var t = new TaskView(TaskStack, task);
                                    TaskStack.Children.Add(t);
                                });
                            }
                        }
                    }
                });
            }
        }

        public void AddNewTask()
        {
            // add an hour view to the task stack
            Dispatcher.Invoke(() =>
            {
                TaskStack.Children.Insert(0, new TaskView(TaskStack));
            });
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            // Cancel the closing operation
            e.Cancel = true;

            // Hide the window instead of closing
            this.Hide();
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // hide the window
            this.Hide();
        }
    }
}
