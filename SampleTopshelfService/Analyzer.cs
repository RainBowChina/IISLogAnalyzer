using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Topshelf.Logging;

namespace SampleTopshelfService
{
    public class Analyzer
    {
        static readonly LogWriter _log = HostLogger.Get<SampleService>();
        public static void AnalyzeRecord(string[] needAnalyzes, Setting setting, string domain)
        {
            var dt = SqlBulk.GetTableSchema();
            if(needAnalyzes==null)
                return;
            foreach (var needAnalyze in needAnalyzes)
            {
                int timeoutCount = 0;
                int clienterrorCount = 0;
                int servererrorCount = 0;
                int totolCount = 0;
                try
                {
                    var fs = new System.IO.FileStream(needAnalyze, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    using (var reader = new StreamReader(fs))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            totolCount++;
                            var fields = line.Split(' ');
                            if (fields.Count() != 12) continue;
                            if (int.Parse(fields[setting.FieldIndex[1]]) >= 400 &&
                                int.Parse(fields[setting.FieldIndex[1]]) < 500)
                                clienterrorCount++;
                            if (int.Parse(fields[setting.FieldIndex[1]]) >= 500)
                                servererrorCount++;
                            if (int.Parse(fields[setting.FieldIndex[2]]) >= 2000)
                                timeoutCount++;
                        }
                    }
                    // Console.WriteLine("文件名:{0}超时请求数:{1},错误请求数:{2}",needAnalyze,timeoutCount,errorCount);
                    var r = dt.NewRow();
                    var lastOrDefault = needAnalyze.Split('\\').LastOrDefault();
                    if (lastOrDefault != null)
                        r[1] = lastOrDefault.Split('.').First().Substring(4, 8).ToString();
                    r[2] = setting.IpMapping[domain];
                    r[3] = domain;
                    r[4] = timeoutCount;
                    r[5] = clienterrorCount;
                    r[6] = servererrorCount;
                    r[7] = totolCount;
                    dt.Rows.Add(r);
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message+ex.StackTrace+"filename:"+needAnalyze);
                }
                SqlBulk.BulkToDb(dt);
            }

        }
    }
}
