Here’s a full **folder and file structure** for the **Reverse Proxy with JWT Authentication** and **Docker Compose** setup:

### **Project Folder Structure**

```plaintext
ReverseProxyWithJWT/
│
├── Docker/
│   ├── Dockerfile-reverse-proxy          # Dockerfile for Reverse Proxy service
│   ├── Dockerfile-auth-service          # Dockerfile for Auth Service (JWT generation)
│
├── reverse-proxy/
│   ├── Program.cs                       # Reverse Proxy Program.cs (main entry)
│   ├── appsettings.json                 # Configuration for Reverse Proxy
│   ├── reverse-proxy.csproj             # Reverse Proxy project file
│   └── wwwroot/                         # Static files (if any)
│       └── index.html                   # Placeholder index file (optional)
│
├── auth-service/
│   ├── Controllers/
│   │   └── AuthController.cs            # Controller to handle login and JWT generation
│   ├── Models/
│   │   └── LoginModel.cs                # Model for login request data
│   ├── appsettings.json                 # Configuration for Auth Service (JWT settings)
│   ├── auth-service.csproj              # Auth Service project file
│   └── Startup.cs                       # Auth Service configuration (optional if using .NET 6 or earlier)
│
├── docker-compose.yml                  # Docker Compose file to run services
├── README.md                           # Project documentation (optional)
└── .gitignore                          # Git ignore file (optional)
```

### **Detailed Explanation**

#### **1️⃣ `Docker/Dockerfile-reverse-proxy`**
The Dockerfile to build the Reverse Proxy image.

```dockerfile
# Step 1: Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Step 2: Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["reverse-proxy/reverse-proxy.csproj", "reverse-proxy/"]
RUN dotnet restore "reverse-proxy/reverse-proxy.csproj"
COPY . .
WORKDIR "/src/reverse-proxy"
RUN dotnet build "reverse-proxy.csproj" -c Release -o /app/build

# Step 3: Publish the app
FROM build AS publish
RUN dotnet publish "reverse-proxy.csproj" -c Release -o /app/publish

# Step 4: Copy the build to the base image and run it
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "reverse-proxy.dll"]
```

#### **2️⃣ `Docker/Dockerfile-auth-service`**
The Dockerfile to build the Auth Service image.

```dockerfile
# Step 1: Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Step 2: Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["auth-service/auth-service.csproj", "auth-service/"]
RUN dotnet restore "auth-service/auth-service.csproj"
COPY . .
WORKDIR "/src/auth-service"
RUN dotnet build "auth-service.csproj" -c Release -o /app/build

# Step 3: Publish the app
FROM build AS publish
RUN dotnet publish "auth-service.csproj" -c Release -o /app/publish

# Step 4: Copy the build to the base image and run it
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "auth-service.dll"]
```

#### **3️⃣ `docker-compose.yml`**
Docker Compose file to link the services (Reverse Proxy and Auth Service).

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
      - auth-service  # Reverse Proxy depends on the Auth Service to validate JWT
    environment:
      - JwtSettings__SecretKey=your-secret-key
    networks:
      - proxy_network

  auth-service:
    build:
      context: .
      dockerfile: Docker/Dockerfile-auth-service
    ports:
      - "5001:80"  # Expose authentication service on port 5001
    environment:
      - JwtSettings__SecretKey=your-secret-key
    networks:
      - proxy_network

networks:
  proxy_network:
    driver: bridge
```

#### **4️⃣ `reverse-proxy/Program.cs`**
Main program file for the Reverse Proxy that handles JWT authentication and routing.

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Configure JWT Authentication Middleware
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://auth-service:5001";  // URL of the auth service
        options.Audience = "your-api";
        options.RequireHttpsMetadata = false;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Use Authentication and Authorization Middleware
app.UseAuthentication(); 
app.UseAuthorization();

// Define login and authentication routes
app.MapPost("/login", async (HttpContext context) =>
{
    var form = await context.Request.ReadFormAsync();
    var username = form["username"];
    var password = form["password"];

    if (username == "admin" && password == "admin")
    {
        var token = "your-jwt-token"; // Simulate the JWT token
        context.Response.Redirect("/authenticated");
    }
    else
    {
        context.Response.Redirect("/loginfailed");
    }
});

app.MapGet("/authenticated", () => Results.Json(new { Message = "You are authenticated!" }));
app.MapGet("/loginfailed", () => Results.Json(new { Message = "Invalid credentials. Try again." }));

// Define a protected route
app.MapGet("/protected-resource", [Authorize] () => Results.Json(new { Message = "You have access to this protected resource!" }));

app.Run();
```

#### **5️⃣ `auth-service/Controllers/AuthController.cs`**
Auth Service controller to handle JWT token generation.

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            if (loginModel.Username == "admin" && loginModel.Password == "admin")
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(new[] {
                        new System.Security.Claims.Claim("username", loginModel.Username),
                        new System.Security.Claims.Claim("role", "admin")
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new { Token = tokenString });
            }

            return Unauthorized("Invalid credentials");
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
```

#### **6️⃣ `auth-service/appsettings.json`**
JWT secret configuration for the Auth Service.

```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key"
  }
}
```

### **Additional Files**

- **`README.md`**: Include a brief overview of your project, how to build and run it, and any necessary instructions for developers.
- **`.gitignore`**: Standard .NET `.gitignore` file for excluding bin, obj, and other temporary files.

---

### **Conclusion**

This project structure allows you to:

1. **Build and run** a Reverse Proxy that integrates JWT authentication.
2. **Authenticate requests** through the reverse proxy, routing protected requests only if the JWT is valid.
3. **Use Docker Compose** to easily manage and run both the reverse proxy and the authentication service in containers.
