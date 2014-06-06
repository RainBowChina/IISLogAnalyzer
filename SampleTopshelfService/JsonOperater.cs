using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SampleTopshelfService
{
    public class JsonOperater
    {
        public static Setting GetAppSetting()
        {
            string json;
            var directoryPath = FileManager.GetServiceDirecotry();
            using (var reader = new StreamReader(directoryPath + "\\app.json"))
            {
                json = reader.ReadToEnd();
            }

            var settings = JsonConvert.DeserializeObject<Setting>(json);
            return settings;
        }


    }
}
