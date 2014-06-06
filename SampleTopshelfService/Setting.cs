using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleTopshelfService
{
    public class Setting
    {
        public string Version { get; set; }

        public string UserName { get; set; }

        public string PassWord { get; set; }

        public string TimeZone { get; set; }

        public int Period { get; set; }

        public int[] FieldIndex { get; set; }

        public string Connection { get; set; }

        public Dictionary<string, string> IpMapping { get; set; }

        public Dictionary<string, string> Directory { get; set; }

    }
  
}
