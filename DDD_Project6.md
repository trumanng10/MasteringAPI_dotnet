To implement external credentials in a JSON file or database for your Reverse Proxy and Auth Service project, you'll modify the existing setup by adding a mechanism to read credentials from an external source (either a JSON file or a database). Here's an updated version of your project structure with the changes and implementations:

### Updated Folder Structure with External Credentials

```
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
│   ├── /wwwroot/                        # Static files (if any)
│   │   └── index.html                   # Placeholder index file (optional)
│   └── /ExternalCredentials/            # Folder for external credential storage
│       └── credentials.json             # External JSON file with credentials
│
├── auth-service/
│   ├── Controllers/
│   │   └── AuthController.cs            # Controller to handle login and JWT generation
│   ├── Models/
│   │   └── LoginModel.cs                # Model for login request data
│   ├── appsettings.json                 # Configuration for Auth Service (JWT settings)
│   ├── auth-service.csproj              # Auth Service project file
│   ├── /ExternalCredentials/            # Folder for external credential storage
│   │   └── credentials.json             # External JSON file with credentials
│   └── Startup.cs                       # Auth Service configuration (optional if using .NET 6 or earlier)
│
├── docker-compose.yml                  # Docker Compose file to run services
├── README.md                           # Project documentation (optional)
└── .gitignore                          # Git ignore file (optional)
```

### Key Changes

1. **External Credentials Folder**:
   - Each service (`reverse-proxy` and `auth-service`) now includes an **ExternalCredentials** folder where credentials are stored in a JSON file. This allows external management of the credentials.
   - **`credentials.json`** file would look like this:
   
   ```json
   {
     "admin": {
       "username": "admin",
       "password": "admin123"
     },
     "user": {
       "username": "user",
       "password": "user123"
     }
   }
   ```

2. **External Credentials in `Program.cs`** (for `auth-service`):

   - You would modify your **`AuthController.cs`** to read from this JSON file instead of hardcoding credentials. Here’s how the modified **`AuthController.cs`** would look:

   ```csharp
   using Microsoft.AspNetCore.Mvc;
   using System.IO;
   using Newtonsoft.Json;
   using YourProjectName.Models;
   using System.Linq;

   namespace AuthService.Controllers
   {
       [Route("api/[controller]")]
       [ApiController]
       public class AuthController : ControllerBase
       {
           private readonly string credentialsFilePath = "ExternalCredentials/credentials.json";

           [HttpPost("login")]
           public IActionResult Login([FromBody] LoginModel login)
           {
               var credentials = GetCredentialsFromFile();

               var user = credentials.FirstOrDefault(c => c.Username == login.Username && c.Password == login.Password);

               if (user != null)
               {
                   var token = GenerateJwtToken(user);  // Call method to generate JWT token
                   return Ok(new { Token = token });
               }
               else
               {
                   return Unauthorized(new { Message = "Invalid credentials" });
               }
           }

           private List<Credential> GetCredentialsFromFile()
           {
               var json = System.IO.File.ReadAllText(credentialsFilePath);
               var credentials = JsonConvert.DeserializeObject<Dictionary<string, Credential>>(json);
               return credentials.Values.ToList();
           }

           private string GenerateJwtToken(Credential user)
           {
               // JWT generation logic
               return "jwt_token";  // This is just a placeholder
           }
       }

       public class Credential
       {
           public string Username { get; set; }
           public string Password { get; set; }
       }
   }
   ```

   - This controller method reads the **`credentials.json`** file, checks the user credentials against the file, and returns a JWT if valid. 

3. **External Credentials in `reverse-proxy`** (optional for Reverse Proxy service):
   - Similarly, you can implement the same approach to handle external credentials for any form of authentication in the **`reverse-proxy`** service (if needed). This can be useful if you need authentication to control access to the reverse proxy or make authentication decisions.

### Docker Configuration

For both services, the **`Dockerfile`** and **`docker-compose.yml`** can remain mostly the same. You need to ensure that the `ExternalCredentials` folder is included in your Docker image and volume bindings.

#### Example `docker-compose.yml`

```yaml
version: '3.4'

services:
  reverse-proxy:
    build:
      context: ./reverse-proxy
      dockerfile: ../Docker/Dockerfile-reverse-proxy
    ports:
      - "5000:80"
    volumes:
      - ./reverse-proxy/ExternalCredentials:/app/ExternalCredentials  # Mount external credentials folder
    environment:
      - ASPNETCORE_ENVIRONMENT=Development

  auth-service:
    build:
      context: ./auth-service
      dockerfile: ../Docker/Dockerfile-auth-service
    ports:
      - "5001:80"
    volumes:
      - ./auth-service/ExternalCredentials:/app/ExternalCredentials  # Mount external credentials folder
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    depends_on:
      - reverse-proxy
```

### `Dockerfile-reverse-proxy`

```dockerfile
# Use an official image as the base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["reverse-proxy/reverse-proxy.csproj", "reverse-proxy/"]
RUN dotnet restore "reverse-proxy/reverse-proxy.csproj"
COPY . .
WORKDIR "/src/reverse-proxy"
RUN dotnet build "reverse-proxy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "reverse-proxy.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "reverse-proxy.dll"]
```

### JWT Integration and Credential Storage

- By storing credentials externally in the JSON file, you can modify the service at runtime without needing to recompile or redeploy.
- The JWT Token is generated using a secret key, stored in **`appsettings.json`**, and used for authenticating API calls.

This setup is modular, and you can later migrate from JSON storage to a database (like SQL Server, MongoDB, or any other database) when needed.
