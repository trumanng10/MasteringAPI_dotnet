We originally wanted to use **YARP (Yet Another Reverse Proxy)**, but we got sidetracked into structuring the project using **DDD**. Let's now add **YARP** into the project while keeping it structured properly.

---

## **📌 Steps to Add YARP (Reverse Proxy)**
1. Install YARP package: `dotnet add package Yarp.ReverseProxy`
2. Configure **YARP** to forward authentication requests
3. Keep DDD structure while making YARP handle `/login` requests

---

### **1️⃣ Install YARP in Your Project**
Run this command in the terminal:
```sh
dotnet add package Yarp.ReverseProxy
```

---

### **2️⃣ Configure YARP in `Program.cs`**
Modify your `Program.cs` to configure **YARP** as a reverse proxy.

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Yarp.ReverseProxy;
using YourProject.Application.Services;
using YourProject.Domain.Services;
using YourProject.Infrastructure.Controllers;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add Reverse Proxy Service
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// ✅ Register Business Logic Services
builder.Services.AddScoped<IAuthenticator, Authenticator>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddControllers();

var app = builder.Build();

// ✅ Enable Routing & Reverse Proxy Middleware
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapReverseProxy();  // 🛑 This Enables YARP!
});

app.Run();
```

---

### **3️⃣ Configure `appsettings.json` for YARP**
Create or modify `appsettings.json` to configure YARP routes:

```json
{
    "Urls": "http://0.0.0.0:5000;https://0.0.0.0:5001",
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft": "Warning",
            "Yarp.ReverseProxy": "Debug", // Logs detailed information about YARP
            "Yarp": "Debug", // General YARP logging
            "Microsoft.AspNetCore": "Information",
            "System.Net.Http": "Information", // Logs HTTP request details
            "Microsoft.AspNetCore.Hosting.Diagnostics": "Information" // ASP.NET Core diagnostics
        }
    },
    "ReverseProxy": {
        "Routes": {
            "minimumroute": {
                "ClusterId": "minimumcluster",
                "Match": {
                    "Path": "{**catch-all}"
                }
            },
            "route2": {
                "ClusterId": "cluster2",
                "Match": {
                    "Path": "/something/{*any}"
                }
            },
            "loginRoute": {
                "ClusterId": "loginCluster",
                "Match": {
                    "Path": "/proxy/login"
                },
                "Transforms": [
                    {
                        "PathRemovePrefix": "/proxy"
                    }
                ]
            },
            "weatherforecastRoute": {
                "ClusterId": "weatherforecastCluster",
                "Match": {
                    "Path": "/proxy/weatherforecast"
                },
                "Transforms": [
                    {
                        "PathRemovePrefix": "/proxy"
                    }
                ]
            }
        },
        "Clusters": {
            "minimumcluster": {
                "Destinations": {
                    "default_destination": {
                        "Address": "https://www.google.com"
                    }
                }
            },
            "cluster2": {
                "Destinations": {
                    "first_destination": {
                        "Address": "https://contoso.com"
                    },
                    "another_destination": {
                        "Address": "https://bing.com"
                    }
                },
                "LoadBalancingPolicy": "RoundRobin"
            },
            "weatherforecastCluster": {
                "Destinations": {
                    "local_destination": {
                        "Address": "https://localhost:5001"
                    }
                }
            },
            "loginCluster": {
                "Destinations": {
                    "login_destination": {
                        "Address": "https://localhost:5001"
                    }
                }
            }

        }
    }
}
```
🔹 This tells YARP:  
- Any request to `/login` will be **forwarded** to another authentication service (e.g., running on `localhost:5001`).

---

### **4️⃣ Modify `LoginController.cs`**
Since the reverse proxy will forward `/login` requests to an actual authentication service, we need a separate API service listening on `http://localhost:5001`.

Example of `LoginController.cs` in the **auth service**:
```csharp
using Microsoft.AspNetCore.Mvc;
using YourProject.Application.Services;

namespace YourProject.Infrastructure.Controllers;

[ApiController]
public class LoginController : ControllerBase
{
    private readonly AuthService _authService;

    public LoginController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login()
    {
        var form = await Request.ReadFormAsync();
        var username = form["username"];
        var password = form["password"];

        var redirectUrl = _authService.Login(username, password);
        return Redirect(redirectUrl);
    }
}
```
💡 Now, when you **POST to `/login`**, YARP **forwards** it to `http://localhost:5001/login`.

---

## **🎯 Summary**
✅ **YARP Reverse Proxy** is added to forward `/login` requests  
✅ **DDD Structure** is still followed  
✅ **Authentication Service** can be deployed separately  
✅ **Flexible & Scalable Architecture**  
