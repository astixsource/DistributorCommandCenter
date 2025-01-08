using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Configuration;
using System.Security.Claims;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Data.SqlClient;
using System.Data;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Net;

/// <summary>
/// Summary description for clsADUserInfo
/// </summary>
public class clsADUserInfo
{

    private string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
    private string appKey = ConfigurationManager.AppSettings["ida:ClientSecret"];
    private string aadInstance = EnsureTrailingSlash(ConfigurationManager.AppSettings["ida:AADInstance"]);
    private string graphResourceID = "https://graph.windows.net";
    public clsADUserInfo()
    {
        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        //
        // TODO: Add constructor logic here
        //
    }

    public async Task<IUser> fnGetUserDetail()
    {
        string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
        string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

        Uri servicePointUri = new Uri(graphResourceID);
        Uri serviceRoot = new Uri(servicePointUri, tenantID);
        ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot,
              async () => await GetTokenForApplication());

        // use the token for querying the graph to get the user details

        var result = await activeDirectoryClient.Users
            .Where(u => u.ObjectId.Equals(userObjectID))
            .ExecuteAsync();
        IUser user = result.CurrentPage.ToList().First();

        return user;
    }

    public string fnGetTokenNoForUser()
    {
        string strResp = "";
        try
        {

            var client = new RestClient("https://login.microsoftonline.com/515445df-f272-4c96-aa97-d670cbdeab20/oauth2/token");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "password");
            request.AddParameter("client_id", clientId);
            request.AddParameter("client_secret", appKey);
            request.AddParameter("scope", "https://graph.microsoft.com/.default");
            request.AddParameter("userName", "anuj@astixsolutions.com");
            request.AddParameter("password", "invalidPWD@12");
            request.AddParameter("scope", "openid");
            request.AddParameter("resource", "https://analysis.windows.net/powerbi/api");
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var JObject1 = JObject.Parse(response.Content);
                string token = JObject1["access_token"].ToString();
                HttpContext.Current.Session["token"] = token;
                HttpContext.Current.Session.Timeout = 50;
                strResp = "1|" + token;
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                strResp = "2|" + JObject.Parse(response.Content).ToString();
            }
            else
            {
                strResp = "2|" + (response.ErrorMessage != "" ? response.ErrorMessage : response.Content);
            }
        }
        catch (Exception ex)
        {
            strResp = "2|" + ex.Message;
        }
        return strResp;
    }
    public string GetApplicationToken()
    {
        string strResp = "";
        try
        {
            var client = new RestClient("https://login.microsoftonline.com/515445df-f272-4c96-aa97-d670cbdeab20/oauth2/token");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("SdkVersion", "postman-graph/v1.0");
            request.AddHeader("Cookie", "fpc=Avbe7LEQkIFOmPJctsLNdPdTzCN4AQAAAP--GtoOAAAAbqtL9AEAAABLwxraDgAAAA; stsservicecookie=estsfd; x-ms-gateway-slice=estsfd");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", clientId);
            request.AddParameter("client_secret", appKey);
            request.AddParameter("scope", "openid");
            request.AddParameter("resource", "https://analysis.windows.net/powerbi/api");
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var JObject1 = JObject.Parse(response.Content);
                string token = JObject1["access_token"].ToString();
                HttpContext.Current.Session["token"] = token;
                HttpContext.Current.Session.Timeout = 50;
                strResp = "1|" + token;
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                strResp = "2|" + JObject.Parse(response.Content).ToString();
            }
            else
            {
                strResp = "2|" + (response.ErrorMessage != "" ? response.ErrorMessage : response.Content);
            }
        }
        catch (Exception ex)
        {
            strResp = "2|" + ex.Message;
        }
        return strResp;
    }

    public string GetEmbedUrl(string reportid, string access_token)
    {
        string strResp = "";
        try
        {
            label:
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                       SecurityProtocolType.Tls11 |
                                       SecurityProtocolType.Tls12;
            var client = new RestClient("https://api.powerbi.com/v1.0/myorg/groups/e64e6f45-222f-4a74-943d-7a419a4bb247/reports/" + reportid);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + access_token);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var JObject1 = JObject.Parse(response.Content);
                string embedUrl = JObject1["embedUrl"].ToString();
                string datasetId = JObject1["datasetId"].ToString();
                strResp = "1|" + embedUrl + "|" + datasetId;
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                strResp = "2|" + JObject.Parse(response.Content).ToString();
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                var data = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<dynamic>(response.Content);
                if(data["error"]["code"]== "TokenExpired")
                {
                   string str= GetApplicationToken();
                    if (str.Split('|')[0] == "1")
                    {
                        access_token = str.Split('|')[1];
                        goto label;
                    }
                }
                strResp = "2|" + JObject.Parse(response.Content).ToString();
            }
            else
            {
                strResp = "2|" + (response.ErrorMessage != "" ? response.ErrorMessage : response.Content);
            }
        }
        catch (Exception ex)
        {
            strResp = "2|" + ex.Message;
        }
        return strResp;
    }

    public string fngeneratetoken(string reportid, string access_token,string UserName,string datasets,bool IsRslapplied)
    {
        string strResp = "";
        try
        {
            label:
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                       SecurityProtocolType.Tls11 |
                                       SecurityProtocolType.Tls12;
            var client = new RestClient("https://api.powerbi.com/v1.0/myorg/groups/e64e6f45-222f-4a74-943d-7a419a4bb247/reports/" + reportid + "/generatetoken");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + access_token);
            string postData = "{\"accessLevel\":\"View\"}";
            if (IsRslapplied)
            {
                postData = "{\"accessLevel\":\"View\",\"identities\":[{\"username\":\"" + UserName + "\",\"roles\":[\"User\"],\"datasets\":[\"" + datasets + "\"]}]}";
            }
            request.AddParameter("application/json", postData, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var JObject1 = JObject.Parse(response.Content);
                string token = JObject1["token"].ToString();
                strResp = "1|" + token;
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                strResp = "2|" + JObject.Parse(response.Content).ToString();
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                var data = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<dynamic>(response.Content);
                if (data["error"]["code"] == "TokenExpired")
                {
                    string str = GetApplicationToken();
                    if (str.Split('|')[0] == "1")
                    {
                        access_token = str.Split('|')[1];
                        goto label;
                    }
                }
                strResp = "2|" + JObject.Parse(response.Content).ToString();
            }
            else
            {
                strResp = "2|" + (response.ErrorMessage != "" ? response.ErrorMessage : response.Content);
            }

        }
        catch (Exception ex)
        {
            strResp = "2|" + ex.Message;
        }
        return strResp;
    }
    public async Task<string> GetTokenForApplication()
    {
        string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
        string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
        string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

        // get a token for the Graph without triggering any user interaction (from the cache, via multi-resource refresh token, etc)
        ClientCredential clientcred = new ClientCredential(clientId, appKey);
        // initialize AuthenticationContext with the token cache of the currently signed in user, as kept in the app's database
        AuthenticationContext authenticationContext = new AuthenticationContext(aadInstance + tenantID);
        AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenSilentAsync(graphResourceID, clientcred, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));
        return authenticationResult.AccessToken;
    }
    private static string EnsureTrailingSlash(string value)
    {
        if (value == null)
        {
            value = string.Empty;
        }

        if (!value.EndsWith("/", StringComparison.Ordinal))
        {
            return value + "/";
        }

        return value;
    }
   
}