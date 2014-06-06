using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Dynamic;
using System.Linq;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace SampleTopshelfService
{
  
    public class IISDataDetail
    {
        public int Id { get; set; }
      
    }

    public class IISTable
    {
        public IISTable()
        {
            DayList=new List<DayModel>();
        }
        public List<HourModel> List { get; set; }

        public List<DayModel> DayList { get; set; } 
 
        public int Count { get; set; }

        public string StartDate { get; set; } 

        public string EndDate { get; set; }

        public string SelectedIp { get; set; }

        public string SelectedDomain { get; set; }

        public List<string> Ips { get; set; }

        public List<string> Domains { get; set; }
    }

    public class chartData
    {
        
      public  int[] ErrorRequstArray { get; set; }
      public int[] ErrorInnerArray { get; set; }
      public  int[] TimeoutRequestArray { get; set; }
     }

    public class DayModel
    {
        public string Day { get; set; } 

        public string Domain { get; set; }

        public string IP { get; set; }

        public int TimeoutCount { get; set; }

        public int ClinetErrorCount { get; set; }

        public int ServerErrorCount { get; set; }

        public int TotalCount { get; set; }
    }

    public class HourModel
    {
        public int Id { get; set; }

        public string Hour { get; set; }

        public string Domain { get; set; }

        public string IP { get; set; }

        public int TimeoutCount { get; set; }

        public int ClinetErrorCount { get; set; }

        public int ServerErrorCount { get; set; }

        public int TotalCount { get; set; }
    }

}
