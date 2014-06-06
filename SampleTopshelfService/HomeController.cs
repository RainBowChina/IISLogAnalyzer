

using System;
using System.Collections.Generic;

namespace SampleTopshelfService
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using RazorEngine;

    public class HomeController:ApiController
    {

        [HttpGet]
        public HttpResponseMessage Index()
        {
            var url = Request.RequestUri.AbsoluteUri.Split('?');
            var startdate = string.Empty;
            var ip = string.Empty;
            var domain = string.Empty;

            if (Request.RequestUri.AbsoluteUri.Split('&').Length==3)
            {
                var parm=url[1].Split('&');
                startdate = parm[0].Split('=')[1];
                ip = parm[1].Split('=')[1];
                domain = parm[2].Split('=')[1];
            }
            else
            {
                startdate = DateTime.Now.ToString("yyyy-MM-dd");
            }             
            var setting = JsonOperater.GetAppSetting();
            var model = Repository.GetIisData(startdate, ip, domain);
            model.StartDate = startdate;
            model.SelectedIp = ip;
            model.SelectedDomain = domain;
            //获取IP列表
            model.Ips=new List<string>();
            model.Domains=new List<string>();
            foreach (var key in setting.IpMapping.Keys)
            {
                model.Domains.Add(key);
                model.Ips.Add(setting.IpMapping[key]);
            }           
            
            string content = new RazorView("Index.cshtml", model).Run();            
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(content, System.Text.Encoding.UTF8, "text/html");            
            return response;
        }
    }
}
