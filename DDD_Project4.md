### **Folder and File Structure for the Project with Docker and JWT Authentication**

Here’s the complete folder structure, including Dockerfiles, your application code, and configuration files.

---

### **1️⃣ Folder Structure Overview**

```
/YourProjectRoot
│
├── /Docker
│   ├── Dockerfile-reverse-proxy        # Dockerfile for reverse proxy
│   └── Dockerfile-auth-service        # Dockerfile for authentication service
│
├── /YourProject                       # Main reverse proxy project
│   ├── YourProject.csproj
│   ├── Program.cs                     # Main program entry point
│   ├── appsettings.json               # Configuration file for reverse proxy
│   └── /Controllers
│       ├── AuthenticationController.cs
│       └── ReverseProxyController.cs
│
├── /AuthService                       # Authentication service project
│   ├── AuthService.csproj
│   ├── Program.cs                     # Main program entry point for auth service
│   ├── appsettings.json               # Configuration file for auth service
│   └── /Controllers
│       └── AuthController.cs          # Controller for handling login and JWT
│
├── docker-compose.yml                 # Docker Compose to orchestrate services
└── README.md                          # Documentation and instructions for your project
```

---

### **2️⃣ Details of the Files and Folders**

#### **`/YourProjectRoot`**

- **`docker-compose.yml`**  
  Docker Compose configuration file to link both the reverse proxy and authentication service.

#### **`/YourProject`** (Reverse Proxy)

- **`YourProject.csproj`**  
  The project file for the reverse proxy application.

- **`Program.cs`**  
  This file contains the setup for the reverse proxy, including the YARP and JWT middleware.

- **`appsettings.json`**  
  Configuration file for the reverse proxy service. It could include settings like API base URLs or JWT settings.

- **`/Controllers`**  
  - **`AuthenticationController.cs`**  
    Handles the login API, validates credentials, and redirects or returns a JWT token.
  - **`ReverseProxyController.cs`**  
    Handles other routes or API endpoints in the reverse proxy application.

#### **`/AuthService`** (Authentication Service)

- **`AuthService.csproj`**  
  The project file for the authentication service.

- **`Program.cs`**  
  This file contains the setup for the authentication service, including JWT token generation and validation.

- **`appsettings.json`**  
  Configuration file for the authentication service, possibly including JWT signing keys or secret configurations.

- **`/Controllers`**
  - **`AuthController.cs`**  
    Handles the login process, validates credentials, and issues JWT tokens.

---

### **3️⃣ Example of `docker-compose.yml` File**

```yaml
version: '3.8'

services:
  reverse-proxy:
    build:
      context: .
      dockerfile: Docker/Dockerfile-reverse-proxy
    ports:
      - "5000:80"  # Expose reverse proxy on port 5000
    depends_on:
      - auth-service  # Ensure auth service starts before reverse proxy

  auth-service:
    build:
      context: .
      dockerfile: Docker/Dockerfile-auth-service
    ports:
      - "5001:80"  # Expose authentication service on port 5001
```

---

### **4️⃣ Example of `Program.cs` for Reverse Proxy Service**

Here’s an updated `Program.cs` for the reverse proxy with JWT authentication, including a login route:

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// JWT Authentication Middleware
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://yourauthservice";  // URL of your authentication service
        options.Audience = "your-api";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Apply JWT Authentication and Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/login", async (HttpContext context) =>
{
    var form = await context.Request.ReadFormAsync();
    var username = form["username"];
    var password = form["password"];

    if (username == "admin" && password == "admin")
    {
        var token = "your-jwt-token";  // Generate or get JWT token here
        context.Response.Redirect("/authenticated");
    }
    else
    {
        context.Response.Redirect("/loginfailed");
    }
});

app.MapGet("/authenticated", () => Results.Json(new { Message = "You are authenticated!" }));
app.MapGet("/loginfailed", () => Results.Json(new { Message = "Invalid credentials. Try again." }));

app.Run();
```

---

### **5️⃣ Example of `Program.cs` for Authentication Service**

Here’s an updated `Program.cs` for the authentication service that generates JWT tokens:

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Secret key for JWT signing
var secretKey = "your-secret-key"; 

// Configure JWT Authentication
builder.Services.AddSingleton(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)));
builder.Services.AddSingleton(new SigningCredentials(
    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
    SecurityAlgorithms.HmacSha256
));

var app = builder.Build();

app.MapPost("/login", async (HttpContext context, SigningCredentials signingCredentials) =>
{
    var form = await context.Request.ReadFormAsync();
    var username = form["username"];
    var password = form["password"];

    if (username == "admin" && password == "admin")
    {
        var claims = new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "admin"),
            new System.Security.Claims.Claim("role", "admin")
        };

        var tokenDescriptor = new JwtSecurityToken(
            issuer: "your-issuer",
            audience: "your-api",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: signingCredentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { Token = token });
    }
    else
    {
        context.Response.StatusCode = 401; // Unauthorized
        await context.Response.WriteAsync("Invalid credentials.");
    }
});

app.Run();
```

---

### **6️⃣ Additional Notes**

1. **JWT Secret**:
   - The JWT secret key (`your-secret-key`) must be stored securely and should be a strong, complex string.
   - In production, consider using environment variables or a secure key vault (e.g., AWS Secrets Manager, Azure Key Vault) to store secrets.

2. **JWT Token Creation**:
   - The `AuthController.cs` in the **AuthService** handles creating the JWT token when a successful login is detected.

3. **Authorization**:
   - The reverse proxy uses JWT tokens to authenticate requests. The `AddAuthentication` middleware validates the JWT from incoming requests.

---

With this structure, you have a **modular** DDD-inspired design where both the reverse proxy and authentication services are **decoupled** and can be scaled or modified independently.

