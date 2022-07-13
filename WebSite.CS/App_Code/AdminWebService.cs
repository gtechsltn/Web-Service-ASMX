using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
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

    //HTTP: POST
    [WebMethod]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public string Hello(string name)
    {
        string helloMessage = $"Hello {name}. Connection String: {ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString}";
        return helloMessage;
    }

    //HTTP: POST
    [WebMethod]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public string HelloWorld()
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

        AppSettingDto appSettings = new AppSettingDto(awsAccessKeyId, awsSecretKey);

        JavaScriptSerializer js = new JavaScriptSerializer();
        return js.Serialize(appSettings); //{ "MaxJsonLength": 2097152, "RecursionLimit": 100 }
    }

    //HTTP: GET
    [WebMethod]
    [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
    public void SayHello()
    {
        Context.Response.Clear();
        Context.Response.ContentType = "application/json";
        Context.Response.Write("Hello World");
        //return "Hello World";
    }

    //HTTP: GET
    [WebMethod]
    [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
    public void SayHelloWorld()
    {
        JavaScriptSerializer js = new JavaScriptSerializer();
        Context.Response.Clear();
        Context.Response.ContentType = "application/json";
        HelloWorldData data = new HelloWorldData
        {
            Message = "Hello World"
        };
        Context.Response.Write(js.Serialize(data));
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

public class HelloWorldData
{
    public string Message;
}