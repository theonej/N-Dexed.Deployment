using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Dependencies;
using N_Dexed.Deployment.Configuration;
using N_Dexed.Deployment.RestAPI.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace N_Dexed.Deployment.RestAPI
{
    public static class WebApiConfig
    {
        private static IDependencyResolver m_DependencyResolver;

        internal static IDependencyResolver GetDependencyContainer()
        {
            if (m_DependencyResolver == null)
            {
            m_DependencyResolver =  Bootstrapper.CreateDependencyController();
            }
            return m_DependencyResolver;
        }

        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "CustomerResources",
                routeTemplate: "api/Customers/{customerId}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "CustomerSystemResources",
                routeTemplate: "api/Customers/{customerId}/Systems/{systemId}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "CustomerSystemApplications",
                routeTemplate: "api/Customers/{customerId}/Systems/{systemId}/Applications/{applicationName}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            //CORS
            var corsConfig = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(corsConfig);

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            config.EnableSystemDiagnosticsTracing();

            config.DependencyResolver = GetDependencyContainer();

            JsonMediaTypeFormatter jsonFormatter = config.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonFormatter.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Insert(0, new PlainTextFormatter());
        }
    }
}
