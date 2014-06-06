using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SampleTopshelfService
{
    public class SqlBulk
    {
        public static void BulkToDb(DataTable dt)
        {
            var setting = JsonOperater.GetAppSetting();
            var sqlConn = new SqlConnection(setting.Connection);
            var bulkCopy = new SqlBulkCopy(sqlConn) { DestinationTableName = "IISLogger", BatchSize = dt.Rows.Count };

            try
            {
                sqlConn.Open();
                if (dt.Rows.Count != 0)
                    bulkCopy.WriteToServer(dt);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                sqlConn.Close();
                bulkCopy.Close();
            }
        }

        public static DataTable GetTableSchema()
        {
            var dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[]{ 
            new DataColumn("id",typeof(int)), 
            new DataColumn("Hour",typeof(string)),
	        new DataColumn("IP",typeof(string)),
            new DataColumn("Domain",typeof(string)),
            new DataColumn("TimeoutCount",typeof(int)),
            new DataColumn("ClinetErrorCount",typeof(int)),
            new DataColumn("ServerErrorCount",typeof(int)), 
            new DataColumn("TotalCount",typeof(int)) 
            });

            return dt;
        }
    }
}
