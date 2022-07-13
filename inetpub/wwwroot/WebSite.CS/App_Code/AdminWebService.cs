using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Web.Services;

/// <summary>
/// Summary description for AdminWebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
[System.Web.Script.Services.ScriptService]
public class AdminWebService : System.Web.Services.WebService
{
    public AdminWebService()
    {
        //Uncomment the following line if using designed components
        //InitializeComponent();
    }

    [WebMethod]
    public AppSettingDto HelloWorld()
    {
        string awsAccessKeyId = string.Empty;
        NameValueCollection loc = (NameValueCollection)ConfigurationManager.GetSection("aVScanAppSettings");
        if (loc["AwsAccessKeyId"] != null)
        {
            Debug.Print($@"AwsAccessKeyId: { loc["AwsAccessKeyId"] }");
            awsAccessKeyId = loc["AwsAccessKeyId"];
        }
        string awsSecretKey = string.Empty;
        if (loc["AwsSecretKey"] != null)
        {
            Debug.Print($@"AwsSecretKey: { loc["AwsSecretKey"] }");
            awsSecretKey = loc["AwsSecretKey"];
        }

        return new AppSettingDto(awsAccessKeyId, awsSecretKey);
    }
}

public class AppSettingDto
{
    public AppSettingDto()
    {
    }

    public AppSettingDto(string awsAccessKeyId, string awsSecretKey)
    {
        AwsAccessKeyId = awsAccessKeyId;
        AwsSecretKey = awsSecretKey;
    }

    public string AwsAccessKeyId { get; }
    public string AwsSecretKey { get; }
}