# Lab 10: Part 1 - Create DotNet Project Structure using DDD Principles

**.NET 8** project structured using **Domain-Driven Design (DDD)** principles:  

### **DDD Layers Implemented**  
1. **Domain Layer** → Contains business logic (`User` and `Authenticator`).  
2. **Application Layer** → Handles user authentication (`AuthService`).  
3. **Infrastructure Layer** → Handles HTTP request handling (`LoginController`).  
4. **Presentation Layer** → Exposes API endpoints (`Program.cs`).  

---

### **1. Create Project Structure**
```
YourProject/
│── YourProject.Api/         # Presentation Layer (Entry point)
│   ├── Program.cs
│── YourProject.Application/ # Application Layer (Business logic coordination)
│   ├── Services/
│   │   ├── AuthService.cs
│── YourProject.Domain/      # Domain Layer (Core business logic)
│   ├── Entities/
│   │   ├── User.cs
│   ├── Services/
│   │   ├── IAuthenticator.cs
│   │   ├── Authenticator.cs
│── YourProject.Infrastructure/ # Infrastructure Layer (External dependencies)
│   ├── Controllers/
│   │   ├── LoginController.cs
```

---

## **1. Domain Layer (`YourProject.Domain`)**
Defines business logic and rules.  

### **User Entity (`User.cs`)**
```csharp
namespace YourProject.Domain.Entities;

public record User(string Username, string Password);
```

### **Authenticator Service (`IAuthenticator.cs` & `Authenticator.cs`)**
```csharp
namespace YourProject.Domain.Services;

public interface IAuthenticator
{
    bool Authenticate(User user);
}
```

```csharp
namespace YourProject.Domain.Services;
using YourProject.Domain.Entities;

public class Authenticator : IAuthenticator
{
    public bool Authenticate(User user)
    {
        return user.Username == "admin" && user.Password == "admin";
    }
}
```

---

## **2. Application Layer (`YourProject.Application`)**
This coordinates business logic.  

### **Auth Service (`AuthService.cs`)**
```csharp
namespace YourProject.Application.Services;
using YourProject.Domain.Entities;
using YourProject.Domain.Services;

public class AuthService
{
    private readonly IAuthenticator _authenticator;

    public AuthService(IAuthenticator authenticator)
    {
        _authenticator = authenticator;
    }

    public string Login(string username, string password)
    {
        var user = new User(username, password);
        return _authenticator.Authenticate(user) ? "/authenticated" : "/loginfailed";
    }
}
```

---

## **3. Infrastructure Layer (`YourProject.Infrastructure`)**
Handles HTTP requests and dependency injection.  

### **Login Controller (`LoginController.cs`)**
```csharp
using Microsoft.AspNetCore.Http;
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

    [HttpGet("authenticated")]
    public IActionResult Authenticated() =>
        Ok(new { Message = "You are authenticated!" });

    [HttpGet("loginfailed")]
    public IActionResult LoginFailed() =>
        Ok(new { Message = "Invalid credentials. Try again." });
}
```

---

## **4. Presentation Layer (`YourProject.Api`)**
The main entry point where all services are registered and the app starts.  

### **Program.cs**
```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YourProject.Application.Services;
using YourProject.Domain.Services;
using YourProject.Infrastructure.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Register services using Dependency Injection
builder.Services.AddScoped<IAuthenticator, Authenticator>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
```
## **5. Communications Among Layers and Manage Nuget**
Note 1:
If any of these references are missing, the namespaces won't be found. In Visual Studio, right-click on YourProject.Api > Add > Project Reference and make sure that all the necessary projects are selected.

Note 2:
In Visual Studio:
1.	Right-click on your project (e.g., YourProject.Infrastructure) and select Manage NuGet Packages.
2.	Under the Browse tab, search for Microsoft.AspNetCore.App and install it.
3.	Under the Browse tab, search for Microsoft.AspNetCore.App.MVC and install it.

---

### **Why This Follows DDD?**
✅ **Domain Layer** contains business logic (`User`, `Authenticator`).  
✅ **Application Layer** handles authentication logic (`AuthService`).  
✅ **Infrastructure Layer** manages request handling (`LoginController`).  
✅ **Presentation Layer** exposes the API (`Program.cs`).  

This makes it **scalable**, **testable**, and **maintainable**. 🚀  

Would you like to add JWT authentication or database storage next? 😊
