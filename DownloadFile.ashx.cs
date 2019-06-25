using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace statistika_net4.statistika
{
    /// <summary>
    /// Summary description for DownloadFile
    /// </summary>
    public class DownloadFile : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            System.Web.HttpRequest request = System.Web.HttpContext.Current.Request;
            string fileName = request.QueryString["fileName"];
            string fileDest = request.QueryString["fileDest"];
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.ContentType = "text/plain";
            response.AddHeader("Content-Disposition",
                               "attachment; filename=" + fileName + ";");
            response.TransmitFile(fileDest);
            response.Flush();
            response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}