using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using todoclient.Infrastructure;

namespace todoclient
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            Synchronize();
        }

        private void Synchronize()
        {
            var synch = new ServiceSynchronizer();
            Task.Run(() => synch.DeleteMissing())
                .ContinueWith((unusedArg) => synch.UploadMissing());
        }
    }
}
