using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace TimesheetTracker
{
    public class TaskObject
    {
        public string Guid { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateTime ExtractionTime { get; set; }
        public double Hours { get; set; }

        public TaskObject()
        {
            ProjectNumber = "";
            ProjectName = "";
            Description = "";
            ExtractionTime = DateTime.Now;
            Hours = 1;
            Guid = System.Guid.NewGuid().ToString();
        }

        public void Save()
        {
            Database.SaveTask(this);
        }

        public void Update()
        {
            Database.UpdateTask(this);
        }

        internal void Delete()
        {
            Database.DeleteTask(this);
        }
    }
}
