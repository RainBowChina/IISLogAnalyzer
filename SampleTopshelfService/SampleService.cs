// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.


namespace SampleTopshelfService
{
    using System;
    using System.Timers;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Web.Http;
    using System.Web.Http.SelfHost;
    using RazorEngine;
    using RazorEngine.Configuration;
    using RazorEngine.Templating;
    using Topshelf;
    using Topshelf.Logging;


    class SampleService :
        ServiceControl
    {
        readonly bool _throwOnStart;
        readonly bool _throwOnStop;
        readonly bool _throwUnhandled;
        static readonly LogWriter _log = HostLogger.Get<SampleService>();
        private readonly HttpSelfHostServer _server;
        private readonly HttpSelfHostConfiguration _config;
        private const string EventSource = "HttpApiService";
        Timer _timer = new Timer(); 

        public SampleService(bool throwOnStart, bool throwOnStop, bool throwUnhandled, Uri address)
        {
            _throwOnStart = throwOnStart;
            _throwOnStop = throwOnStop;
            _throwUnhandled = throwUnhandled;
            if (!EventLog.SourceExists(EventSource))
            {
                EventLog.CreateEventSource(EventSource, "Application");
            }
            EventLog.WriteEntry(EventSource,
                String.Format("Creating server at {0}",
                address.ToString()));
            _config = new HttpSelfHostConfiguration(address);
            _config.Routes.MapHttpRoute("DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional }
            );
            _config.Routes.MapHttpRoute(
              "Default", "{controller}/{action}",
               new { controller = "Home", action = "Index", date = RouteParameter.Optional});
            const string viewPathTemplate = "SampleTopshelfService.Views.{0}";
            var templateConfig = new TemplateServiceConfiguration();
            templateConfig.Resolver = new DelegateTemplateResolver(name =>
            {
                string resourcePath = string.Format(viewPathTemplate, name);
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            });
            Razor.SetTemplateService(new TemplateService(templateConfig));


            _server = new HttpSelfHostServer(_config);
        }

        public bool Start(HostControl hostControl)
        {
            _log.Info("SampleService Starting...");

            hostControl.RequestAdditionalTime(TimeSpan.FromSeconds(10));

            _server.OpenAsync();
            var setting = JsonOperater.GetAppSetting();
            _timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            _timer.Interval = 1000 * 60 * setting.Period;
            _timer.Enabled = true;
            _timer.Start();
            //var setting = JsonOperater.GetAppSetting();
            //FileManager.InitStates(setting);

            //foreach (var key in setting.Directory.Keys)
            //{
            //    if (FileShare.ConnectState(setting.Directory[key], setting.UserName, setting.PassWord))
            //    {
            //        var needAnalyzes = FileManager.GetNeedAnalyzeFiles(setting.Directory[key], key);
            //        Analyzer.AnalyzeRecord(needAnalyzes, setting, key);
            //    }
            //}
            _log.Info("SampleService Started");

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _log.Info("SampleService Stopped");
            FileManager.MemoryCatch();
            if(_throwOnStop)
                throw new InvalidOperationException("Throw on Stop Requested!");
            _server.CloseAsync().Wait();
            _server.Dispose();
            return true;
        }

        public bool Pause(HostControl hostControl)
        {
            _log.Info("SampleService Paused");

            return true;
        }

        public bool Continue(HostControl hostControl)
        {
            _log.Info("SampleService Continued");

            return true;
        }

        private void timer_Elapsed(object sender, EventArgs e)
        {
            //#region 开始分析iis日志
            try
            {
                var setting = JsonOperater.GetAppSetting();
                FileManager.InitStates(setting);

                foreach (var key in setting.Directory.Keys)
                {
                  //  if (FileShare.ConnectState(setting.Directory[key]))
                 //   {
                        var needAnalyzes = FileManager.GetNeedAnalyzeFiles(setting.Directory[key], key);
                        Analyzer.AnalyzeRecord(needAnalyzes, setting, key);
                //    }
                }
                _log.Info("this turn completeed,time is:"+DateTime.Now);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);                
            }
           

            //#endregion
        }
    }
}