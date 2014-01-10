using System.Web;
using System.Web.Mvc;

namespace N_Dexed.Deployment.RestAPI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}