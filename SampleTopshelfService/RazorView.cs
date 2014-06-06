using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleTopshelfService
{
    using RazorEngine;
    using RazorEngine.Templating;

    public class RazorView
    {
        private ExecuteContext context = new ExecuteContext();

        public string TemplateName { get; set; }
        public string Template { get; set; }
        public object Model { get; set; }
        public dynamic ViewBag
        {
            get
            {
                return context.ViewBag;
            }
        }

        public RazorView()
        {
        }

        public RazorView(string templateName)
        {
            this.TemplateName = templateName;
        }

        public RazorView(string templateName, object model)
            : this(templateName)
        {
            this.Model = model;
        }

        public string Run()
        {
            string content = null;
            if (!string.IsNullOrEmpty(this.TemplateName))
            {
                content =
                    Razor.Resolve(this.TemplateName, this.Model).Run(this.context);
            }
            else if (!string.IsNullOrEmpty(this.Template))
            {
                content =
                    Razor.Parse(this.Template, this.Model);
            }

            return content;
        }
    }
}
