Deploying a minimal .NET application on Kubernetes involves several key steps, including creating a lightweight .NET application, containerizing it, and setting up Kubernetes configurations. Here's how it's done:

### **1. Create a Minimal .NET Application**
Use the .NET SDK to create a minimal API using **ASP.NET Core Minimal APIs**:

```sh
dotnet new web -o MinimalApiApp
cd MinimalApiApp
```

Modify `Program.cs` to define a simple API:

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello from Minimal .NET!");

app.Run();
```

### **2. Add Docker Support**
Create a `Dockerfile` to containerize the application:

```dockerfile
# Stage 1: Build and publish the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY MinimalApiApp.csproj ./
RUN dotnet restore

# Copy the remaining source code and publish the app
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 80 and set the entrypoint
EXPOSE 80
ENTRYPOINT ["dotnet", "MinimalApiApp.dll"]
```

### **3. Build and Push Docker Image**
Build and push the image to a container registry like Docker Hub or Azure Container Registry:

```sh
docker build -t <yourdockerhubusername>/minimal-dotnet .
docker login
docker push <yourdockerhubusername>/minimal-dotnet
```

### **4. Create Kubernetes Deployment and Service**
Create a `deployment.yaml` file:

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: minimal-dotnet
spec:
  replicas: 2
  selector:
    matchLabels:
      app: minimal-dotnet
  template:
    metadata:
      labels:
        app: minimal-dotnet
    spec:
      containers:
      - name: minimal-dotnet
        image: yourdockerhubusername/minimal-dotnet:latest
        ports:
        - containerPort: 80
---
apiVersion: v1
kind: Service
metadata:
  name: minimal-dotnet-service
spec:
  selector:
    app: minimal-dotnet
  ports:
    - protocol: TCP
      port: 80
      targetPort: 8080
  type: LoadBalancer
```

### **5. Deploy to Kubernetes**
Apply the deployment:

```sh
kubectl apply -f deployment.yaml
```

Check the status:

```sh
kubectl get pods
kubectl get services
```

### **6. Access the Application**
Once the LoadBalancer is provisioned, get the external IP:

```sh
kubectl get svc minimal-dotnet-service
```

Then, access it via `http://EXTERNAL_IP`.

