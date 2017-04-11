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
            Synchronize();
        }

        private void Synchronize()
        {
            var synch = new ServiceSynchronizer();
            Task.Run(() => synch.SynchId())
                .ContinueWith((unusedArg) =>
                {
                    synch.DeleteMissing();
                    synch.UploadMissing();
                });
        }
    }
}
