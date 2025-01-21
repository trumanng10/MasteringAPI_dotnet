# **Lab 6: Setting Up a Simple .NET 8 Web API with YARP on Windows VS CODE**

---

## **Lab Overview**  
In this lab, you'll learn how to set up a simple **.NET 8 Web API** and configure **YARP (Yet Another Reverse Proxy)**. This setup will help you understand request routing, proxying API requests, and handling multiple backend services using YARP.

---

## **Learning Objectives**  

By the end of this lab, you will be able to:  

1. Install and configure .NET 8 SDK on Windows.  
2. Set up and run a simple .NET 8 Web API using Visual Studio Code.  
3. Install and configure YARP as a reverse proxy.  
4. Test and validate proxy functionality using PowerShell or Postman.  

---

## **Prerequisites**  

Ensure the following before starting the lab:  

1. **Windows 10/11** machine.  
2. Installed **.NET 8 SDK** – Download from [https://dotnet.microsoft.com/en-us/download](https://dotnet.microsoft.com/en-us/download).  
3. Installed **Visual Studio Code** – Download from [https://code.visualstudio.com/](https://code.visualstudio.com/).  
4. Installed **PowerShell** (default on Windows) for testing.  
5. Installed **Postman** (optional) for API testing.  

---

## **Step 1: Install .NET 8 SDK**  

1. Download the .NET 8 SDK from the official website:  
   [https://dotnet.microsoft.com/en-us/download/dotnet/8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
   
2. Run the installer and follow the setup instructions.  
   
3. Verify installation by opening **PowerShell** and running:  

   ```powershell
   dotnet --version
   ```

   Expected output: `8.x.x` (confirming successful installation).

---

## **Step 2: Set Up Visual Studio Code**  

1. Download and install **VS Code** from [https://code.visualstudio.com/](https://code.visualstudio.com/).  
2. Open VS Code and install the following extensions:  
   - **C# Dev Kit**  
   - **C# Extensions**  
   - **REST Client** (optional for API testing inside VS Code)

---

## **Step 3: Create a New .NET 8 Web API Project**  

1. Open **PowerShell** and navigate to your preferred directory:  

   ```powershell
   cd C:\Projects
   ```

2. Create a new Web API project:  

   ```powershell
   dotnet new webapi -n SimpleYarpApi
   ```

3. Navigate to the project folder:  

   ```powershell
   cd SimpleYarpApi
   ```

4. Open the project in **VS Code** by running:  

   ```powershell
   code .
   ```

---

## **Step 4: Run the Web API**  

1. In VS Code, open the **Terminal** (`Ctrl + ~`).  
2. Run the project:  

   ```powershell
   dotnet run
   ```

3. The default API will start on `http://localhost:5000`.  
4. Open a browser or use PowerShell to test it:  

   ```powershell
   Invoke-RestMethod -Uri http://localhost:5000/weatherforecast
   ```

---

## **Step 5: Install YARP Package**  

1. In the VS Code terminal, install the YARP package using:  

   ```powershell
   dotnet add package Yarp.ReverseProxy
   ```

2. Confirm the package is installed by checking `SimpleYarpApi.csproj` for an entry like:

   ```xml
   <PackageReference Include="Yarp.ReverseProxy" Version="1.x.x" />
   ```

---

## **Step 6: Configure YARP in the Web API**  

1. Open the `Program.cs` file in VS Code.  
2. Replace the contents with the following code to configure YARP:  

   ```csharp
   using Microsoft.AspNetCore.Builder;
   using Microsoft.Extensions.DependencyInjection;
   using Yarp.ReverseProxy.Configuration;

   var builder = WebApplication.CreateBuilder(args);

   // Add YARP reverse proxy configuration
   builder.Services.AddReverseProxy()
       .LoadFromMemory(new[]
       {
           new RouteConfig()
           {
               RouteId = "api_route",
               ClusterId = "api_cluster",
               Match = new RouteMatch { Path = "/proxy/{**catch-all}" }
           }
       },
       new[]
       {
           new ClusterConfig()
           {
               ClusterId = "api_cluster",
               Destinations = new Dictionary<string, DestinationConfig>
               {
                   { "destination1", new DestinationConfig { Address = "http://localhost:5000" } }
               }
           }
       });

   var app = builder.Build();

   // Enable reverse proxy middleware
   app.MapReverseProxy();
   app.Run();
   ```

3. Save the file (`Ctrl + S`).

---

## **Step 7: Run the Reverse Proxy Application**  

1. Start the proxy server:  

   ```powershell
   dotnet run
   ```

2. Open a browser and visit:  

   ```
   http://localhost:5000/proxy/weatherforecast
   ```

3. You should see a JSON response forwarded via the proxy.

---

## **Step 8: Testing the Proxy Setup**  

### **Using PowerShell:**  

```powershell
Invoke-RestMethod -Uri http://localhost:5000/proxy/weatherforecast
```

### **Using Postman:**  

1. Open **Postman**.  
2. Enter the URL: `http://localhost:5000/proxy/weatherforecast`.  
3. Click **Send**, and verify the response.

---

## **Step 9: Enhancing the Proxy Configuration**  

You can add additional features to your proxy, such as:  

1. **Load Balancing:** Modify `Program.cs` to add multiple backend services:

   ```csharp
   new ClusterConfig()
   {
       ClusterId = "api_cluster",
       LoadBalancingPolicy = "RoundRobin",
       Destinations = new Dictionary<string, DestinationConfig>
       {
           { "destination1", new DestinationConfig { Address = "http://localhost:5000" } },
           { "destination2", new DestinationConfig { Address = "http://localhost:5001" } }
       }
   }
   ```

2. **SSL Support:** Enable HTTPS by adding the following to `Program.cs`:

   ```csharp
   app.UseHttpsRedirection();
   ```

---

## **Step 10: Stop and Cleanup**  

1. Stop the running process by pressing `Ctrl + C` in the terminal.  
2. To remove the project, delete the project folder:  

   ```powershell
   Remove-Item -Recurse -Force C:\Projects\SimpleYarpApi
   ```

---

## **Step 11: Troubleshooting**  

1. **"Port already in use" error:**  

   - Identify the process using the port:  
     ```powershell
     netstat -ano | findstr :5000
     ```

   - Kill the process using:  
     ```powershell
     taskkill /PID <PID> /F
     ```

2. **Check for project build errors:**  

   ```powershell
   dotnet build
   ```

3. **Test different endpoints using:**  

   ```powershell
   curl http://localhost:5000/proxy/weatherforecast
   ```

---

## **Conclusion**  

Congratulations! You've successfully set up a **.NET 8 Web API** with **YARP** as a reverse proxy using **Visual Studio Code on Windows**. You learned how to:  

- Install and configure .NET 8 on Windows.  
- Set up and run a .NET 8 Web API project.  
- Install and configure YARP for request forwarding.  
- Test the proxy setup using PowerShell and Postman.  

---

## **Additional Resources**  

- [Microsoft YARP Documentation](https://microsoft.github.io/reverse-proxy/)  
- [.NET 8 Documentation](https://learn.microsoft.com/en-us/dotnet/core/)  
- [VS Code .NET Support](https://code.visualstudio.com/docs/languages/dotnet)  
