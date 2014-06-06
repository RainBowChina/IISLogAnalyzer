using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using SampleTopshelfService;

namespace SampleTopshelfService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web.Http;

    public class ProductsController : ApiController
    {

        public chartData GetAllProducts(string date,string ip,string domain)
        {
            var chartData = Repository.GetChartData(date,ip,domain);
            return chartData;
        }

        public IISTable GetProductById(int id)
        {
            var data = new IISTable();
            return data;
        }

       
    }
}
