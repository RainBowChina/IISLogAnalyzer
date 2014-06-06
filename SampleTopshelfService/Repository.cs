using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace SampleTopshelfService
{
    public static class Repository
    {
 
        public static IISTable GetIisData(string date,string ip,string domain)
        {
            var param = int.Parse(date.Replace("-", string.Empty).Substring(2,6));
            var connectionStr = JsonOperater.GetAppSetting().Connection;
            var connection = new SqlConnection(connectionStr);
            var sql = string.Format("select * from IISLogger with(nolock) where hour like '{0}%' order by hour", param);
            var list = connection.Query<HourModel>(sql).Where(s => s.Domain == domain && s.IP == ip).OrderByDescending(s => s.Hour).ToList();
            var item = new IISTable();
            item.List = list;
            item.Count = 1;
            return item;
        }

        public static chartData GetChartData(string date,string ip,string domain)
        {
            var param = int.Parse(date.Replace("-", string.Empty).Substring(2, 6));
             var chartData = new chartData();
            var connectionStr = JsonOperater.GetAppSetting().Connection;
            var connection = new SqlConnection(connectionStr);
            var sql = string.Format("select * from IISLogger with(nolock) where hour like '{0}%' order by hour", param);
            var list = connection.Query<HourModel>(sql).Where(s => s.Domain == domain && s.IP == ip).ToList();           
            chartData.ErrorRequstArray=new int[list.Count];
            chartData.TimeoutRequestArray=new int[list.Count];
            chartData.ErrorInnerArray=new int[list.Count];
            for (var i=0;i<list.Count;i++)
            {
                chartData.ErrorRequstArray[i] = list[i].ClinetErrorCount;
                chartData.ErrorInnerArray[i] = list[i].ServerErrorCount;
                chartData.TimeoutRequestArray[i] = list[i].TimeoutCount;
            }
            return chartData;
        }
    }
}
