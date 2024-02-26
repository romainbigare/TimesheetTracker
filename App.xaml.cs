using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;

namespace TimesheetTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon _taskbarIcon;
        private MainWindow _mainWindow;
        private DispatcherTimer _timer;

        // TODO:
        // 

        protected override void OnStartup(StartupEventArgs e)
        {
            /*/     INITIAL DATA LOAD //    NO LONGER NEEDED .  MAYBE FOR NEXT USERS
            string csvFilePath = @"C:\Users\rbigare\Desktop\timesheet2.csv";

            // Open the CSV file for reading
            using (StreamReader reader = new StreamReader(csvFilePath))
            {
                string line;

                // Read the file line by line
                while ((line = reader.ReadLine()) != null)
                {
                    // Split the line into fields using comma as the delimiter
                    string[] fields = line.Split(',');

                    var t = new TaskObject();
                    t.ProjectNumber = fields[0];
                    t.ProjectName = fields[1];
                    t.Description = fields[2];
                    t.ExtractionTime = DateTime.Now;
                    t.Hours = 0;
                    t.Save();
                }
            }
            return;
            /*/

            base.OnStartup(e);

            // Create and setup the main window
            _mainWindow = new MainWindow();

            // Create and setup the taskbar icon
            _taskbarIcon = new TaskbarIcon();

            String filename = this.GetType().Namespace + ".Resources.timesheet.ico";
            //or String.Join if you like
            var path = "pack://application:,,,/TimesheetTracker;component/Resources/timesheet.ico";


            _taskbarIcon.IconSource = new System.Windows.Media.Imaging.BitmapImage(new Uri(path));
            _taskbarIcon.ToolTipText = "Timesheet helper";
            _taskbarIcon.TrayMouseDoubleClick += (sender, args) => ShowMainWindow();
            
            // add context menu to taskbar icon to exit application
            _taskbarIcon.ContextMenu = new ContextMenu();
            var menuItem = new MenuItem();
            menuItem.Header = "Exit";
            menuItem.Click += (sender, args) => Application.Current.Shutdown();
            _taskbarIcon.ContextMenu.Items.Add(menuItem);

            // one for show main window
            var menuItem2 = new MenuItem();
            menuItem2.Header = "Show";
            menuItem2.Click += (sender, args) => ShowMainWindow();
            _taskbarIcon.ContextMenu.Items.Add(menuItem2);


            // Create and setup timer to show window automatically every hour on weekdays
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromHours(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void ShowMainWindow()
        {
            _mainWindow.Show();
            _mainWindow.Activate();
            _mainWindow.Focus();
            _mainWindow.Topmost = true;
            _mainWindow.AddNewTask();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Check if it's a weekday and time is between 11am and 6pm
            if (DateTime.Now.DayOfWeek != DayOfWeek.Saturday && DateTime.Now.DayOfWeek != DayOfWeek.Sunday &&
                DateTime.Now.Hour >= 11 && DateTime.Now.Hour <= 21)
            {
                ShowMainWindow();
            }
        }

    }
}
