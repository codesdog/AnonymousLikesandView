using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Web;

namespace SPAjax
{
    /// <summary>
    /// GetDocumentLibraryList 的摘要说明
    /// </summary>
    public class GetDocumentLibraryList : IHttpHandler
    {
        #region IHttpHandler成员


        public void ProcessRequest(HttpContent context)
        {
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            System.Web.Script.Serialization.JavaScriptSerializer jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            context.Response.ContentType = "application/json";
            var jsonResult = jsonSerializer.Serialize(new JosnResult() { Name = "Abraham Cheng", Age = 29 });// return what you want
            context.Response.Write(jsonResult);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        #endregion
    }
    class JosnResult
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}