using JolTudomE_Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;

namespace JolTudomE_Api.Security {


  public class LoginMessageHandler : DelegatingHandler {
    private const string _AuthType = "basic";
    private const string _TokenName = "JolTudomEToken";
    private string _Token;
    private string _UserName;

    protected override async System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken) {

      if (HttpContext.Current.User.Identity.IsAuthenticated) {
        return await base.SendAsync(request, cancellationToken);
      }
      if (!CanHandleAuthentication(request)) {
        return await base.SendAsync(request, cancellationToken);
      }
      bool isAuthenticated;
      try {
        isAuthenticated = Authenticate(request);
      }
      catch (Exception) {
        return CreateUnauthorizedResponse();
      }
      if (isAuthenticated) {
        var response = await base.SendAsync(request, cancellationToken);

        // add token cookie value
        IEnumerable<CookieHeaderValue> ccoll = new CookieHeaderValue[] { new CookieHeaderValue(_TokenName, _Token) };
        response.Headers.AddCookies(ccoll);
        
        return response.StatusCode == HttpStatusCode.Unauthorized ? CreateUnauthorizedResponse() : response;
      }
      return CreateUnauthorizedResponse();
    }

    private bool Authenticate(HttpRequestMessage request) {
      var authHeader = request.Headers.Authorization;
      if (authHeader == null) {
        return false;
      }
      var credentialParts = GetCredentialParts(authHeader);
      if (credentialParts.Length != 2) {
        return false;
      }
      return SetPrincipal(credentialParts[0], credentialParts[1]);
    }

    private bool SetPrincipal(string username, string password) {
      var user = ValidateUser(username, password);
      IPrincipal principal = null;
      if (user == null || (principal = GetPrincipal(user)) == null) {
        return false;
      }
      Thread.CurrentPrincipal = principal;
      if (HttpContext.Current != null) {
        HttpContext.Current.User = principal;
      }
      return true;
    }

    private IPrincipal GetPrincipal(LoginResponse user) {
      var identity = new CustomIdentity(_UserName,
        _AuthType,
        user.PersonID,
        user.RoleID,
        _Token);

      return new CustomPrincipal(identity);
    }

    private LoginResponse ValidateUser(string username, string password) {
      using (JolTudomEEntities db = new JolTudomEEntities()) {
        usp_Authenticate_Result result = db.usp_Authenticate(username, password).FirstOrDefault();
        if (result != null) {
          var session = SessionManager.NewSession(result.PersonID, result.RoleID).Session;
          _Token = session.Token;
          _UserName = session.Person.UserName;
          return new LoginResponse {
            PersonID = result.PersonID,
            RoleID = result.RoleID
          };
        }
        else
          return null;
      }
    }

    private bool CanHandleAuthentication(HttpRequestMessage request) {
      return (request.Headers != null
      && request.Headers.Authorization != null
      && request.Headers.Authorization.Scheme.ToLowerInvariant() == _AuthType);
    }

    private string[] GetCredentialParts(AuthenticationHeaderValue authHeader) {
      var encodedCredentials = authHeader.Parameter;
      var credentialBytes = Convert.FromBase64String(encodedCredentials);
      var credentials = Encoding.ASCII.GetString(credentialBytes);
      var credentialParts = credentials.Split(':');
      return credentialParts;
    }

    private HttpResponseMessage CreateUnauthorizedResponse() {
      var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
      response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(_AuthType));
      return response;
    }

  }
}