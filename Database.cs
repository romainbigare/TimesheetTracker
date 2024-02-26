using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TimesheetTracker
{
    internal static class Database
    {
        private static List<string> _projectNames;
        public static List<string> ProjectNames
        {
            get
            {
                if (_projectNames == null || _projectNames.Count<1)
                {
                    _projectNames = Database.GetUniqueProjectNames().Where(s=>!String.IsNullOrEmpty(s)).ToList();
                }
                return _projectNames;
            }
        }
        private static List<string> _projectNumbers;
        public static List<string> ProjectNumbers
        {
            get
            {
                if (_projectNumbers == null || _projectNumbers.Count < 1)
                {
                    _projectNumbers = Database.GetUniqueProjectNumbers().Where(s => !String.IsNullOrEmpty(s)).ToList();
                }
                return _projectNumbers;
            }
        }
        private static List<string> _descriptions;
        public static List<string> Descriptions
        {
            get
            {
                if (_descriptions == null || _descriptions.Count < 1)
                {
                    _descriptions = Database.GetUniqueDescriptions().Where(s => !String.IsNullOrEmpty(s)).ToList();
                }
                return _descriptions;
            }
        }

        public static string databasePath { get; internal set; } =
            @"C:\Users\rbigare\Desktop\timesheet.db";

        public static string User
        {
            get
            {
                return Environment.UserName;
            }
        }

        public static string Time
        {
            get
            {
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public static string connectionString()
        {
            return $"Data Source={databasePath};Version=3;";
        }

        internal static void LogProblem(string message)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString()))
            {
                try
                {
                    connection.Open();
                    string sql = $"INSERT INTO ErrorLog (User, Date, Message) VALUES ('{User}', '{Time}', '{message}')";
                    connection.Execute(sql);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not connect to database, the x:drive might be down");
                    return;
                }

            }
        }

        internal static List<TaskObject> LoadLatest(int count)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString()))
            {
                try
                {
                    connection.Open();
                    string sql = $"SELECT * FROM Tasks ORDER BY ExtractionTime DESC LIMIT {count}";
                    return connection.Query<TaskObject>(sql).ToList();
                }
                catch (Exception ex)
                {
                    return new List<TaskObject>();
                }
            }
        }

        internal static void SaveTask(TaskObject taskObject)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString()))
            {
                try
                {
                    connection.Open();
                    string sql = $"INSERT INTO Tasks (Guid, ProjectNumber, ProjectName, Description, ExtractionTime, Hours) VALUES ('{taskObject.Guid}', '{taskObject.ProjectNumber}', '{taskObject.ProjectName}', '{taskObject.Description}', '{taskObject.ExtractionTime.ToString("yyyy-MM-dd HH:mm:ss")}', '{taskObject.Hours}')";
                    connection.Execute(sql);
                }
                catch (Exception ex)
                {
                    LogProblem(ex.Message);
                }
            }
        }

        internal static void UpdateTask(TaskObject taskObject)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString()))
            {
                try
                {
                    connection.Open();
                    string sql = $"UPDATE Tasks SET ProjectNumber = '{taskObject.ProjectNumber}', ProjectName = '{taskObject.ProjectName}', Description = '{taskObject.Description}', ExtractionTime = '{taskObject.ExtractionTime.ToString("yyyy-MM-dd HH:mm:ss")}', Hours = '{taskObject.Hours}' WHERE Guid = '{taskObject.Guid}'";
                    connection.Execute(sql);
                }
                catch (Exception ex)
                {
                    LogProblem(ex.Message);
                }
            }
        }

        internal static List<string> GetUniqueProjectNames()
        {
            // how do i change this to only get the last 10 unique project names
            using (SQLiteConnection connection = new SQLiteConnection(connectionString()))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT DISTINCT ProjectName FROM Tasks ORDER BY ExtractionTime DESC LIMIT 20";
                    return connection.Query<string>(sql).ToList();
                }
                catch (Exception ex)
                {
                    LogProblem(ex.Message);
                    return new List<string>();
                }
            }
        }

        internal static List<string> GetUniqueProjectNumbers()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString()))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT DISTINCT ProjectNumber FROM Tasks ORDER BY ExtractionTime DESC LIMIT 20";
                    return connection.Query<string>(sql).ToList();
                }
                catch (Exception ex)
                {
                    LogProblem(ex.Message);
                    return new List<string>();
                }
            }
        }

        internal static List<string> GetUniqueDescriptions()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString()))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT DISTINCT Description FROM Tasks ORDER BY ExtractionTime DESC LIMIT 20";
                    return connection.Query<string>(sql).ToList();
                }
                catch (Exception ex)
                {
                    LogProblem(ex.Message);
                    return new List<string>();
                }
            }
        }

        internal static string GetProjectName(string projectNumber)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString()))
            {
                try
                {
                    connection.Open();
                    string sql = $"SELECT ProjectName FROM Tasks WHERE ProjectNumber = '{projectNumber}' ORDER BY ExtractionTime DESC LIMIT 1";
                    return connection.Query<string>(sql).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    LogProblem(ex.Message);
                    return "";
                }
            }
        }

        internal static string GetProjectNumber(string projectName)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString()))
            {
                try
                {
                    connection.Open();
                    string sql = $"SELECT ProjectNumber FROM Tasks WHERE ProjectName = '{projectName}' ORDER BY ExtractionTime DESC LIMIT 1";
                    return connection.Query<string>(sql).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    LogProblem(ex.Message);
                    return "";
                }
            }
        }

        internal static (string, string) GetProjectNameAndNumber(string description)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString()))
            {
                try
                {
                    connection.Open();
                    string sql = $"SELECT ProjectNumber, ProjectName FROM Tasks WHERE Description LIKE '%{description}%' ORDER BY ExtractionTime DESC LIMIT 1";
                    return connection.Query<(string, string)>(sql).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    LogProblem(ex.Message);
                    return ("", "");
                }
            }
        }

        internal static void DeleteTask(TaskObject taskObject)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString()))
            {
                try
                {
                    connection.Open();
                    string sql = $"DELETE FROM Tasks WHERE Guid = '{taskObject.Guid}'";
                    connection.Execute(sql);
                }
                catch (Exception ex)
                {
                    LogProblem(ex.Message);
                }
            }
        }
    }
}
