using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace statistika_net4.statistika
{
    public partial class downloadFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String fileName = Request.QueryString["fileName"];
            String excellFilePath = Request.QueryString["excellFilePath"];
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.ContentType = "text/plain";
            response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ";");
            response.TransmitFile(excellFilePath.Replace('*','\\'));
            response.Flush();
            response.End();
        }
    }
}