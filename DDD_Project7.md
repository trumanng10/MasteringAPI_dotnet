To replace the JSON file with MongoDB for storing credentials, you need to adjust the project to connect to a MongoDB container. We’ll configure MongoDB as an external service in `docker-compose.yml`, and modify the code to interact with MongoDB instead of the JSON file.

Here’s how you can modify your project:

### 1. **Add MongoDB Container to Docker Compose**

Update your `docker-compose.yml` to add MongoDB as a service:

```yaml
version: '3.4'

services:
  reverse-proxy:
    build:
      context: ./reverse-proxy
      dockerfile: ../Docker/Dockerfile-reverse-proxy
    ports:
      - "5000:80"
    depends_on:
      - auth-service
    environment:
      - ASPNETCORE_ENVIRONMENT=Development

  auth-service:
    build:
      context: ./auth-service
      dockerfile: ../Docker/Dockerfile-auth-service
    ports:
      - "5001:80"
    depends_on:
      - mongo-db
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - MONGODB_URI=mongodb://mongo-db:27017
    volumes:
      - ./auth-service/ExternalCredentials:/app/ExternalCredentials  # Optional: If you still want to keep some local data persistence

  mongo-db:
    image: mongo:latest
    container_name: mongo-db
    ports:
      - "27017:27017"
    volumes:
      - mongo-data:/data/db

volumes:
  mongo-data:
```

### 2. **Modify `AuthService` to Use MongoDB for Storing Credentials**

You'll need to install the MongoDB NuGet package to connect your application to MongoDB:

```bash
dotnet add package MongoDB.Driver
```

Now, modify your **`AuthController.cs`** to interact with MongoDB instead of reading from a JSON file:

#### `AuthController.cs` (Updated)

```csharp
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Linq;
using YourProjectName.Models;
using System.Threading.Tasks;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMongoCollection<UserCredential> _credentialsCollection;

        public AuthController(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("authdb");  // Choose database name
            _credentialsCollection = database.GetCollection<UserCredential>("credentials");  // Choose collection name
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            var user = await _credentialsCollection
                .Find(c => c.Username == login.Username && c.Password == login.Password)
                .FirstOrDefaultAsync();

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

        private string GenerateJwtToken(UserCredential user)
        {
            // JWT generation logic (you should implement this method to generate the token)
            return "jwt_token";  // Placeholder for JWT token
        }
    }

    // Model to represent user credentials in MongoDB
    public class UserCredential
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
```

### 3. **Configure MongoDB Connection**

In **`appsettings.json`** (for your **auth-service**), you can specify the MongoDB URI (although it's already passed through `docker-compose.yml` as an environment variable):

```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://mongo-db:27017",
    "DatabaseName": "authdb"
  }
}
```

### 4. **Modify `Startup.cs` or `Program.cs` to Configure MongoDB Dependency Injection**

Make sure you configure MongoDB with Dependency Injection (DI) so that the `AuthController` can use it. You can add this configuration in `Program.cs` (or `Startup.cs` for older versions of .NET):

#### **`Program.cs` (or `Startup.cs`)**

```csharp
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Register MongoDB client with DI
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var mongoUri = builder.Configuration["MONGODB_URI"];
    return new MongoClient(mongoUri);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
```

### 5. **Add Sample Credentials to MongoDB**

To populate MongoDB with some sample credentials, you can either:

- Use a MongoDB client (like **MongoDB Compass** or **Mongo shell**) to manually insert some records.
- Write a migration or seed method to insert the credentials automatically when the app starts.

For example, using **MongoDB Compass**, you can connect to `mongodb://localhost:27017`, and add a collection `credentials` to the `authdb` database with the following sample data:

```json
{
  "Username": "admin",
  "Password": "admin123"
}
```

You can add other users as needed.

### 6. **Testing the Authentication**

- Start your containers using Docker Compose:
  
  ```bash
  docker-compose up --build
  ```

- The `auth-service` will now be able to query the MongoDB container for credentials and authenticate users via JWT.

- You can now use **Postman** or any HTTP client to make a `POST` request to `/api/auth/login` with JSON body:

  ```json
  {
    "username": "admin",
    "password": "admin123"
  }
  ```

If the credentials are valid, the response will return a JWT token. If invalid, it will return a 401 Unauthorized response.

### Updated Folder and File Structure

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
│   └── /ExternalCredentials/            # Folder for external credential storage (optional)
│       └── credentials.json             # External JSON file with credentials (optional)
│
├── auth-service/
│   ├── Controllers/
│   │   └── AuthController.cs            # Controller to handle login and JWT generation
│   ├── Models/
│   │   └── LoginModel.cs                # Model for login request data
│   ├── appsettings.json                 # Configuration for Auth Service (JWT settings)
│   ├── auth-service.csproj              # Auth Service project file
│   ├── /ExternalCredentials/            # Folder for external credential storage (optional)
│   │   └── credentials.json             # External JSON file with credentials (optional)
│   └── Startup.cs                       # Auth Service configuration (optional if using .NET 6 or earlier)
│
├── mongo-db/                            # MongoDB container directory
│   └── /data/db/                        # MongoDB data storage (generated by container)
│
├── docker-compose.yml                  # Docker Compose file to run services
├── README.md                           # Project documentation (optional)
└── .gitignore                          # Git ignore file (optional)
```

### Conclusion

Now, MongoDB is used as the external database for storing user credentials, 
and the auth-service queries this database to authenticate users and generate JWT tokens. This setup is flexible, and you can easily swap MongoDB for another database in the future if needed.
