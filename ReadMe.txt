
# Secure AppSettings in Web.config
-------------------------------------
C:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_regiis -pe "aVScanAppSettings" -site "WebSite.CS" -app "/" -prov "DataProtectionConfigurationProvider"
C:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_regiis -pe "aVScanAppSettings" -site "WebSite.VB" -app "/" -prov "DataProtectionConfigurationProvider"

Web.config
-------------------------------------
  <configSections>
    <section name="aVScanAppSettings" type="System.Configuration.NameValueSectionHandler, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
  </configSections>
  <appSettings>
    <add key="webpages:Version" value="3.0.0.0" />
    <add key="webpages:Enabled" value="false" />
    <add key="PreserveLoginUrl" value="true" />
    <add key="ClientValidationEnabled" value="true" />
    <add key="UnobtrusiveJavaScriptEnabled" value="true" />
  </appSettings>
  <aVScanAppSettings>
    <add key="AwsAccessKeyId" value="manh.nguyen@thirdsight.net" />
    <add key="AwsSecretKey" value="password@8" />
  </aVScanAppSettings>
  <connectionStrings>
    <clear />
    <add name="DBConnectionString"
         connectionString="xmaildatabase=MsSql;Data Source=.;Integrated Security=True;Initial Catalog=mssql;Connect Timeout=30;Pooling=True" />
    <add name="FSBackend.RepoMgr.Properties.Settings.DBConnStr"
       connectionString="Data Source=.;Integrated Security=True;Initial Catalog=mssql;Connect Timeout=30;Pooling=True" />
  </connectionStrings>