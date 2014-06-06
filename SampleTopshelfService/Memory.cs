using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleTopshelfService
{
    public class Memory
    {
        public Memory()
        {
            Container = new Dictionary<string, DateTime>();
        }

        public Dictionary<string, DateTime> Container { get; set; }  //记忆功能
    }
}
