# Integrating Okta SSO with ASP.NET Core

## 1. Set Up Okta Account

1. Sign up for an Okta Developer account if you haven't already.
2. Create a new application in Okta:
   - Choose "Web" as the platform.
   - Select "OpenID Connect" as the sign-on method.
   - Set the login redirect URI (e.g., https://your-app-domain/authorization-code/callback).

## 2. Install Required NuGet Packages

Add these packages to your project:

```
Okta.AspNetCore
Microsoft.AspNetCore.Authentication.OpenIdConnect
```

## 3. Configure Okta in appsettings.json

Add Okta configuration to your `appsettings.json`:

```json
{
  "Okta": {
    "OktaDomain": "https://{yourOktaDomain}",
    "ClientId": "{clientId}",
    "ClientSecret": "{clientSecret}"
  }
}
```

## 4. Configure Authentication in Program.cs

Update your `Program.cs` to use Okta:

```csharp
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Okta.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOktaMvc(new OktaMvcOptions
{
    OktaDomain = builder.Configuration["Okta:OktaDomain"],
    ClientId = builder.Configuration["Okta:ClientId"],
    ClientSecret = builder.Configuration["Okta:ClientSecret"],
    Scope = new List<string> { "openid", "profile", "email" }
});

// ... other service configurations ...

var app = builder.Build();

// ... other app configurations ...

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

## 5. Protect Controllers and Actions

Use the `[Authorize]` attribute to protect controllers or actions:

```csharp
[Authorize]
public class SecureController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
```

## 6. Implement Login and Logout

Create actions for login and logout:

```csharp
public class AccountController : Controller
{
    public IActionResult Login()
    {
        if (!HttpContext.User.Identity.IsAuthenticated)
        {
            return Challenge(OpenIdConnectDefaults.AuthenticationScheme);
        }
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public IActionResult Logout()
    {
        return new SignOutResult(new[] { OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme });
    }
}
```

## 7. Access User Information

You can access the authenticated user's information in your controllers:

```csharp
[Authorize]
public class UserInfoController : Controller
{
    public IActionResult Index()
    {
        var username = User.Identity.Name;
        var claims = User.Claims;
        // ... use the user information as needed
        return View();
    }
}
```

## 8. Configure Claim Mapping (Optional)

If you need specific claims, configure claim mapping:

```csharp
.AddOktaMvc(new OktaMvcOptions
{
    // ... other options ...
    ClaimActions = new List<ClaimAction>
    {
        new MapAllClaimsAction()
    }
})
```

## 9. Implement Authorization Policies (Optional)

For fine-grained access control:

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireClaim("groups", "Admin"));
});
```

Then use it in your controllers:

```csharp
[Authorize(Policy = "RequireAdminRole")]
public class AdminController : Controller
{
    // ... admin actions ...
}
```

## 10. Test the Integration

- Verify the login flow redirects to Okta and back.
- Check that protected resources require authentication.
- Ensure user information is correctly populated after login.

Remember to follow Okta's security best practices and keep your client secrets secure.
