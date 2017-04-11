using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using todoclient.Infrastructure;

namespace todoclient
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            Task.Run(() => Synchronize());
        }

        private void Synchronize()
        {
            var synch = new ServiceSynchronizer();
            synch.SynchId();
            synch.DeleteMissing();
            synch.UploadMissing();
        }
    }
}
