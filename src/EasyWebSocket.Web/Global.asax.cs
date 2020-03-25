using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace EasyWebSocket.Web
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            EasyWebSocketServer.CreateServer()
            .MapHandle<SampleHandle>("/simple")
            .WithFlashPoliy()
            .Start();
        }
    }
}