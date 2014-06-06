using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Topshelf.Logging;

namespace SampleTopshelfService
{
    public class FileManager
    {
        private static ConcurrentDictionary<string, DateTime> statesDic = new ConcurrentDictionary<string, DateTime>();
        static readonly LogWriter _log = HostLogger.Get<SampleService>();
        public static void InitStates(Setting setting)
        {
            if (!MemoryExist())
            {
                InitMemoryFile(setting);
            }
            var memory = ReadMemoryFile();
            foreach (var dic in setting.Directory)
            {
                statesDic.TryAdd(dic.Key, memory.Container[dic.Key]);
            }
        }

        public static string[] GetNeedAnalyzeFiles(string path, string key)
        {
            var allFiles = GetFiles(path);
            if (allFiles.Length== 0)
                return null;
            var rememberTime = GetLastAnalyzeFile(path, key); //最后分析的时间
            var files =
                allFiles.Where(s => s.CreationTime > rememberTime&&s.CreationTime<DateTime.Now.AddHours(-1))
                    .Select(s => s.FullName).ToArray();
            return files;
        }

        public static string[] GetSubDirecorys(string path)
        {
            var subDirecorys = Directory.GetDirectories(path);
            return subDirecorys;
        }

        public static List<FileInfo> GetSubFiles(string path)
        {
            try
            {
                var dir = new DirectoryInfo(path);
                return dir.GetFiles().Where(f => f.Name.EndsWith(".log")).ToList();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message+ex.StackTrace+"path is:"+path);            
            }
            return new List<FileInfo>();
        }

        public static FileInfo[] GetFiles(string path)
        {
            var files = new List<FileInfo>();
            var subFiles = GetSubFiles(path.TrimEnd('\\'));
            var subDirs = GetSubDirecorys(path);
            foreach (var subDir in subDirs)
            {
                var dir = new DirectoryInfo(subDir);
                files.AddRange(dir.GetFiles().Where(f => f.Name.EndsWith(".log")).ToList());
            }
            files.AddRange(subFiles);
            return files.ToArray();
        }

        /// <summary>
        /// 找到某个站点最后分析的日志
        /// </summary>
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static DateTime GetLastAnalyzeFile(string path, string key)
        {
            var pathValue = statesDic[key];
            var lastCreateFile = GetFiles(path).Max(s => s.CreationTime);
            if (pathValue < DateTime.Now.AddYears(-10))
            {
                statesDic[key] = lastCreateFile;
                return DateTime.Now.AddYears(-50);
            }
            statesDic[key] = lastCreateFile;
            return pathValue;
        }

        public static string GetServiceDirecotry()
        {
            var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            var directoryPath = Path.GetDirectoryName(location);
            return directoryPath;
        }

        public static void MemoryCatch()
        {
            var currentMemory = new Memory();
            var path = GetServiceDirecotry() + "\\memory.data";

            foreach (var pair in statesDic)
            {
                currentMemory.Container.Add(pair.Key, pair.Value);
            }
            var memoryJson = JsonConvert.SerializeObject(currentMemory);
            File.WriteAllText(path, memoryJson);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="setting"></param>
        private static void InitMemoryFile(Setting setting)
        {
            var lastMemory = new Memory();
            foreach (var dic in setting.Directory)
            {
                lastMemory.Container.Add(dic.Key, DateTime.Now.AddYears(-50));
            }
            var memoryJson = JsonConvert.SerializeObject(lastMemory);
            var path = GetServiceDirecotry();
            using (var file = new System.IO.StreamWriter(path + "\\memory.data"))
            {
                file.Write(memoryJson);
            }
        }

        private static Memory ReadMemoryFile()
        {
            string json;
            var path = GetServiceDirecotry() + "\\memory.data";
            using (var reader = new StreamReader(path))
            {
                json = reader.ReadToEnd();
            }

            var memory = JsonConvert.DeserializeObject<Memory>(json);
            return memory;
        }

        private static bool MemoryExist()
        {
            var path = GetServiceDirecotry() + "\\memory.data";
            return File.Exists(path);
        }
    }
}
