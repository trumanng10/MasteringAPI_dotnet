# Lab 10 Part 3: step-by-step guide to containerizing and running the project on **.NET 8.0** in the **cloud** ***

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

# Copy the solution file and restore dependencies
COPY src/YourProject.sln ./
COPY src/Services ./Services

# Restore dependencies
WORKDIR /app/Services/YourProject
RUN dotnet restore YourProject.API/YourProject.API.csproj

# Build and publish the API project
RUN dotnet publish YourProject.API/YourProject.API.csproj -c Release -o /publish

# Use the .NET 8 runtime for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /publish .

# Expose ports
EXPOSE 5000
EXPOSE 5001

# Set entry point
ENTRYPOINT ["dotnet", "YourProject.API.dll"]
```
Save the file (`CTRL + X`, then `Y`, then `Enter`).

---

### **Step 5: Build and Run the Docker Container**
**IMPORTANT NOTE**:
---

1. **Verify the Password Used for the Certificate**
Try manually checking if your password is correct:

```sh
openssl pkcs12 -info -in ~/.aspnet/https/aspnetapp.pfx
```

It will prompt you for a password. If the password is incorrect, it will fail.

---

2. **Delete and Recreate the Certificate**
If you don't remember the password, delete the existing certificate and generate a new one:

```sh
dotnet dev-certs https --clean
dotnet dev-certs https -ep ~/.aspnet/https/aspnetapp.pfx -p NewSecurePassword
dotnet dev-certs https --trust  # Only needed on local development machines
```

Now, use `NewSecurePassword` when running the container.

---

3. **Run the Docker Container Again**
Modify your command with the updated password:

```sh
docker run -p 8000:5000 -p 8001:5001 \
  -v ~/.aspnet/https:/root/.aspnet/https \
  -e ASPNETCORE_Kestrel__Certificates__Default__Path=/root/.aspnet/https/aspnetapp.pfx \
  -e ASPNETCORE_Kestrel__Certificates__Default__Password=NewSecurePassword \
  my-dotnet8-app
```



4. **Build the Docker image**:

   ```sh
   docker build -t my-dotnet8-app .
   ```

5. **Run the container**:

   ```sh
   docker run -p 5000:5000 -p 5001:5001 my-dotnet8-app
   ```

---

### **Step 6: Access the Application**
- Open your cloud server's public IP in a browser:
  Method: POST
  username: admin
  password: admin
  method: x-www-form-urlencoded
  ```
  http://your-cloud-ip:5000
  ```
- If HTTPS is enabled, use:
  Method: POST
  username: admin
  password: admin
  method: x-www-form-urlencoded
  ```
  https://your-cloud-ip:5001/login
  ```


- TEST USING the following and observe the ERROR, TRY TO FIX IT!
  Method: POST
  username: admin
  password: admin
  method: x-www-form-urlencoded
  ```
  https://your-cloud-ip:5001/proxy/login
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

Now your .NET 8 project is containerized and running on the cloud.Test it using POSTMAN!


