using System;
using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Runtime.InteropServices;

namespace SPAjax
{
    public class GetList : IHttpHandler
    {
        [Guid("F7D5B5C3-94E7-455A-B25D-076F31483C4C")]
        #region IHttpHandler成员


        public void ProcessRequest(HttpContent context)
        {
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
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
}
