### **🔐 Adding JWT Authentication to Your YARP Reverse Proxy Setup**  

Now that we've set up **YARP** as a reverse proxy, let's **add JWT authentication** to secure the `/authenticated` endpoint.  

---

## **📌 Steps to Add JWT Authentication**
1️⃣ Install JWT authentication package  
2️⃣ Configure authentication in `Program.cs`  
3️⃣ Modify the `AuthService` to generate JWT tokens  
4️⃣ Secure the `/authenticated` endpoint  

---

### **1️⃣ Install Required Packages**  
Run the following command to install the JWT authentication package:
```sh
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

---

### **2️⃣ Configure Authentication in `Program.cs`**
Modify `Program.cs` to **enable JWT authentication**:

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Yarp.ReverseProxy;
using YourProject.Application.Services;
using YourProject.Domain.Services;
using YourProject.Infrastructure.Controllers;

var builder = WebApplication.CreateBuilder(args);

// ✅ JWT Secret Key (Make sure to store it securely in appsettings.json or env variables)
var key = Encoding.UTF8.GetBytes("YourSuperSecretKeyHere");

// ✅ Add Authentication & JWT Bearer Services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

// ✅ Add Reverse Proxy
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// ✅ Register Services
builder.Services.AddScoped<IAuthenticator, Authenticator>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();

// ✅ Enable Authentication & Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapReverseProxy();
});

app.Run();
```
---

### **3️⃣ Modify `AuthService.cs` to Generate JWT Tokens**
Create a new service `AuthService.cs` to handle login & JWT token generation.

```csharp
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace YourProject.Application.Services;

public class AuthService
{
    private const string SecretKey = "YourSuperSecretKeyHere"; // ⚠️ Store securely!

    public string? Login(string username, string password)
    {
        if (username == "admin" && password == "admin")
        {
            return GenerateJwtToken(username);
        }
        return null;
    }

    private string GenerateJwtToken(string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(SecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
```
---
### **4️⃣ Modify `LoginController.cs` to Return JWT Token**
Modify the **LoginController** to return a **JWT token** instead of just redirecting.

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

        var token = _authService.Login(username, password);

        if (token == null)
        {
            return Unauthorized(new { Message = "Invalid credentials" });
        }

        return Ok(new { Token = token });
    }
}
```
---
### **5️⃣ Secure `/authenticated` Endpoint**
Modify `/authenticated` so only **users with valid JWT tokens** can access it.

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourProject.Infrastructure.Controllers;

[ApiController]
public class AuthenticatedController : ControllerBase
{
    [Authorize]
    [HttpGet("authenticated")]
    public IActionResult Get()
    {
        return Ok(new { Message = "You are authenticated!" });
    }
}
```
---
## **🎯 How It Works**
1️⃣ **POST** to `/login` with `username` and `password` → Returns a **JWT token**  
2️⃣ **Include JWT in Authorization Header** when calling `/authenticated`  
3️⃣ If JWT is valid → ✅ Access granted  
4️⃣ If JWT is missing/invalid → ❌ Access denied  

---

## **🚀 Testing with Postman**
### **🔹 1. Login Request**
**POST** `http://localhost:5000/login`  
📌 **Body (form-data)**:
```plaintext
username = admin
password = admin
```
✅ **Response**:
```json
{
  "Token": "your.jwt.token.here"
}
```
---
### **🔹 2. Access Secure Endpoint**
**GET** `http://localhost:5000/authenticated`  
📌 **Headers**:
```
Authorization: Bearer your.jwt.token.here
```
✅ **Response**:
```json
{
  "Message": "You are authenticated!"
}
```
---
## **🎯 Summary**
✔️ **YARP Reverse Proxy** forwards `/login`  
✔️ **JWT Authentication** secures `/authenticated`  
✔️ **Postman Testing** works as expected  
