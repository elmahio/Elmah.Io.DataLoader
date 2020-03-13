using Elmah.Io.Client;
using Elmah.Io.Client.Models;
using GoCommando;
using System;
using Newtonsoft.Json.Serialization;

namespace Elmah.Io.DataLoader
{
    [Command("load")]
    [Description("Load a configurable number of error messages into an elmah.io log")]
    public class Loader : ICommand
    {
        const string DotNetStackTrace = @"
System.Web.HttpException (0x80004005): The controller for path '/api/test' was not found or does not implement IController.
   at System.Web.Mvc.DefaultControllerFactory.GetControllerInstance(RequestContext requestContext, Type controllerType)
   at System.Web.Mvc.DefaultControllerFactory.CreateController(RequestContext requestContext, String controllerName)
   at System.Web.Mvc.MvcHandler.ProcessRequestInit(HttpContextBase httpContext, IController& controller, IControllerFactory& factory)
   at System.Web.Mvc.MvcHandler.BeginProcessRequest(HttpContextBase httpContext, AsyncCallback callback, Object state)
   at System.Web.Mvc.MvcHandler.BeginProcessRequest(HttpContext httpContext, AsyncCallback callback, Object state)
   at System.Web.Mvc.MvcHandler.System.Web.IHttpAsyncHandler.BeginProcessRequest(HttpContext context, AsyncCallback cb, Object extraData)
   at System.Web.HttpApplication.CallHandlerExecutionStep.System.Web.HttpApplication.IExecutionStep.Execute()
   at System.Web.HttpApplication.CallHandlerExecutionStep.System.Web.HttpApplication.IExecutionStep.Execute()
   at System.Web.HttpApplication.ExecuteStep(IExecutionStep step, Boolean& completedSynchronously)";

        [Parameter("apikey")]
        [Description("Your elmah.io API key as found on the profile page")]
        public string ApiKey { get; set; }

        [Parameter("logid")]
        [Description("The log ID of the log to import messages into")]
        public string LogId { get; set; }

        public void Run()
        {
            var api = new ElmahioAPI(new ApiKeyCredentials(ApiKey));
            var random = new Random();
            var yesterday = DateTime.UtcNow.AddDays(-1);
            for (var i = 0; i < 50; i++)
            {
                var r = random.NextDouble();
                api.Messages.Create(LogId, new CreateMessage
                {
                    //Application = "Elmah.Io.DataLoader",
                    Cookies = new[]
                    {
                       new Item("ASP.NET_SessionId", "lm5lbj35ehweehwha2ggsehh")
                    },
                    DateTime = yesterday.AddMinutes(random.Next(1440)),
                    Detail = DotNetStackTrace,
                    Form = new[]
                    {
                        new Item("Username", "ThomasArdal")
                    },
                    QueryString = new[]
                    {
                        new Item("logid", LogId)
                    },
                    ServerVariables = new[]
                    {
                        new Item("CERT_KEYSIZE", "256"),
                        new Item("CONTENT_LENGTH", "0"),
                        new Item("QUERY_STRING", "logid=" + LogId),
                        new Item("REQUEST_METHOD", "POST"),
                        new Item("HTTP_USER_AGENT", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36")
                    },
                    Hostname = "Web01",
                    Severity = Error(r),
                    Source = "Elmah.Io.DataLoader.exe",
                    StatusCode = StatusCode(r),
                    Title = Title(r),
                    Type = Type(r),
                    Url = Url(r),
                    User = User(r),
                    Version = "1.1.0",
                });
            }
        }

        private string Url(double random)
        {
            if (random > 0.5) return "/api/process";
            if (random > 0.2) return "/api/test";
            return null;
        }

        private string Type(double random)
        {
            if (random > 0.5) return "System.NullReferenceException";
            if (random > 0.2) return "System.Net.HttpException";
            return null;
        }

        private string Title(double random)
        {
            if (random > 0.5) return "Object reference not set to an instance of an object.";
            if (random > 0.2)
                return "The controller for path '/api/test' was not found or does not implement IController.";
            return "Processing request";
        }

        private int? StatusCode(double random)
        {
            if (random > 0.5) return 500;
            if (random > 0.2) return 404;
            return null;
        }

        private string Error(double random)
        {
            if (random > 0.5) return "Error";
            if (random > 0.2) return "Warning";
            return "Information";
        }

        private string User(double random)
        {
            if (random > 0.7) return "thomas@elmah.io";
            if (random > 0.4) return "info@elmah.io";
            return null;
        }
    }
}
