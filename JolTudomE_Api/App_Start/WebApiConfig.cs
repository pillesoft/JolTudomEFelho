using JolTudomE_Api.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace JolTudomE_Api {
  public static class WebApiConfig {
    public static void Register(HttpConfiguration config) {
      // Web API configuration and services

      config.EnableCors();

      // Web API routes
      config.MapHttpAttributeRoutes();

      config.Routes.MapHttpRoute(
        name: "Login",
        routeTemplate: "api/Account/Login"
      );

      config.Routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );

      config.MessageHandlers.Add(new TokenMessageHandler());
      config.MessageHandlers.Add(new LoginMessageHandler());
    }
  }
}
