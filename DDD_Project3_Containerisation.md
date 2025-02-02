Here’s a step-by-step guide to containerizing and running the project on **.NET 8.0** in the **cloud**.

---

### **Step 1: Clone the Repository**
First, SSH into your cloud server and clone the project from GitHub:

```sh
git clone https://github.com/trumanng10/MicroLevel2Project2.git
cd MicroLevel2Project2
```

---

### **Step 2: Set Up the .NET Development Certificate**
Navigate to the API project directory and trust the development certificate:

```sh
cd src/Services/YourProject/YourProject.API/
dotnet dev-certs https --trust
```

---

### **Step 3: Run the Application Locally (Optional)**
To verify everything is working before containerization:

```sh
dotnet run
```

---

### **Step 4: Create a `Dockerfile`**
Go back to the project root and create a `Dockerfile`:

```sh
cd ../../../..
nano Dockerfile
```

Paste the following content:

```dockerfile
# Use the .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files
COPY ["src/YourProject.sln", "./"]
COPY ["src/Services/YourProject", "src/Services/YourProject"]

# Restore dependencies
WORKDIR /app/src/Services/YourProject
RUN dotnet restore

# Build and publish
RUN dotnet publish -c Release -o /publish

# Use the .NET 8 runtime for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /publish .

# Expose the necessary port
EXPOSE 5000
EXPOSE 5001

# Set entry point
ENTRYPOINT ["dotnet", "YourProject.API.dll"]
```
Save the file (`CTRL + X`, then `Y`, then `Enter`).

---

### **Step 5: Build and Run the Docker Container**
1. **Build the Docker image**:

   ```sh
   docker build -t my-dotnet8-app .
   ```

2. **Run the container**:

   ```sh
   docker run -p 5000:5000 -p 5001:5001 my-dotnet8-app
   ```

---

### **Step 6: Access the Application**
- Open your cloud server's public IP in a browser:  
  ```
  http://your-cloud-ip:5000
  ```
- If HTTPS is enabled, use:
  ```
  https://your-cloud-ip:5001
  ```

---

### **Step 7: (Optional) Use Docker Compose**
If your project has multiple services (e.g., database, frontend, backend), create a `docker-compose.yml`:

```sh
nano docker-compose.yml
```

Paste this:

```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "5000:5000"
      - "5001:5001"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
```

Save and run:

```sh
docker-compose up -d
```

---

Now your .NET 8 project is containerized and running on the cloud. Let me know if you need further assistance! 🚀
