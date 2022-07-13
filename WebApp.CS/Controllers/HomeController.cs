using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Web.Mvc;

namespace WebApp.CS.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            NameValueCollection loc = (NameValueCollection)ConfigurationManager.GetSection("aVScanAppSettings");
            if (loc["AwsAccessKeyId"] != null)
            {
                Debug.Print($@"AwsAccessKeyId: { loc["AwsAccessKeyId"] }");
                ViewBag.AwsAccessKeyId = loc["AwsAccessKeyId"];
            }
            if (loc["AwsSecretKey"] != null)
            {
                Debug.Print($@"AwsSecretKey: { loc["AwsSecretKey"] }");
                ViewBag.AwsSecretKey = loc["AwsSecretKey"];
            }
            return View();
        }
    }
}