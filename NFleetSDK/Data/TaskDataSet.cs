using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFleet.Data
{
    public class TaskDataSet : IResponseData, IVersioned
    {
        public int VersionNumber { get; set; }
        public List<TaskData> Items { get; set; }
        public List<Link> Meta { get; set; }

        public TaskDataSet()
        {
            Items = new List<TaskData>();
            Meta = new List<Link>();
        }
    }
}
