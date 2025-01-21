
# **Lab 6: Setting Up a Simple .NET 8 Web API with YARP(Window)**

---

## **Lab Overview**  
In this lab, you'll learn how to set up a simple **.NET 8 Web API** and configure **YARP** to act as a reverse proxy. This setup will help you understand routing, proxying API requests, and handling multiple backend services using YARP.

---

## **Learning Objectives**  

By the end of this lab, you will be able to:  

1. Create a .NET 8 Web API using Visual Studio.  
2. Install and configure YARP to act as a reverse proxy.  
3. Implement routing for API requests using YARP.  
4. Test the proxy by forwarding requests to backend services.  

---

## **Prerequisites**  

Ensure the following software is installed on your Windows machine:  

1. **Visual Studio 2022+** with the **ASP.NET and Web Development** workload.  
2. **.NET 8 SDK**, available from [https://dotnet.microsoft.com/en-us/download](https://dotnet.microsoft.com/en-us/download).  
3. Basic understanding of REST APIs and HTTP requests.  

---

## **Step 1: Install Required Software**  

1. **Install Visual Studio 2022+**  
   - Download and install it from [https://visualstudio.microsoft.com/](https://visualstudio.microsoft.com/).  
   - Ensure **.NET 8 SDK** is selected during installation.  

2. **Verify .NET Installation**  
   - Open PowerShell or Command Prompt and run:  
     ```powershell
     dotnet --version
     ```
   - Ensure it returns version `8.x.x` or later.  

---

## **Step 2: Create a .NET 8 Web API Project**  

1. **Open Visual Studio** and select **"Create a new project."**  
2. Choose **"ASP.NET Core Web API"**, then click **Next**.  
3. Name the project (e.g., `SimpleYarpApi`) and click **Next**.  
4. Choose **.NET 8 (Long Term Support)** and click **Create**.  
5. Run the application by pressing **F5** to verify it starts correctly.  
   - The default URL should be something like:  
     ```
     https://localhost:5001/weatherforecast
     ```

---

## **Step 3: Install and Configure YARP**  

1. **Install YARP via NuGet Package Manager**  

   - Open the **Package Manager Console** in Visual Studio (Tools > NuGet Package Manager > Package Manager Console).  
   - Install YARP by running the following command:  

     ```powershell
     dotnet add package Yarp.ReverseProxy
     ```

2. **Modify `Program.cs` to Configure YARP**  

   Open `Program.cs` and replace the contents with the following YARP configuration:

   ```csharp
   using Microsoft.AspNetCore.Builder;
   using Microsoft.Extensions.DependencyInjection;
   using Yarp.ReverseProxy.Configuration;

   var builder = WebApplication.CreateBuilder(args);

   // Add YARP services to the app
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
                   { "destination1", new DestinationConfig { Address = "https://localhost:5001" } }
               }
           }
       });

   var app = builder.Build();

   // Enable reverse proxy middleware
   app.MapReverseProxy();
   app.Run();
   ```

---

## **Step 4: Run the Reverse Proxy Application**  

1. **Set the YARP proxy project as the startup project:**  
   - In Visual Studio, right-click the project and choose **"Set as Startup Project."**  

2. **Run the Application:**  
   - Press **F5** or click **Start Debugging.**  

3. **Test the Proxy by Visiting:**  
   - Open a web browser and navigate to:  
     ```
     https://localhost:5000/proxy/weatherforecast
     ```
   - You should see the JSON response from the backend API, proving that YARP is routing traffic correctly.

---

## **Step 5: Testing and Validation**  

1. **Using Postman:**  
   - Send a **GET** request to:  
     ```
     https://localhost:5000/proxy/weatherforecast
     ```
   - Ensure the response matches the original API output.

2. **Using Command Prompt (cURL):**  
   ```powershell
   curl https://localhost:5000/proxy/weatherforecast
   ```

3. **View Logs in Visual Studio:**  
   - Check the Output window to verify incoming proxy requests.

---

## **Step 6: Add Load Balancing (Optional)**  

Enhance YARP by adding load balancing to distribute traffic across multiple API instances. Modify the cluster configuration in `Program.cs`:  

```csharp
new ClusterConfig()
{
    ClusterId = "api_cluster",
    LoadBalancingPolicy = "RoundRobin",
    Destinations = new Dictionary<string, DestinationConfig>
    {
        { "destination1", new DestinationConfig { Address = "https://localhost:5001" } },
        { "destination2", new DestinationConfig { Address = "https://localhost:5002" } }
    }
}
```

---

## **Step 7: Securing the Reverse Proxy**  

1. **Enable HTTPS Forwarding:**  
   - Add this middleware in `Program.cs`:  
     ```csharp
     app.UseHttpsRedirection();
     ```

2. **Enable Authentication (Optional):**  
   - Implement authentication middleware to protect endpoints.  

---

## **Step 8: Stop and Cleanup**  

1. **Stop the Running Applications:**  
   - Click **Stop** in Visual Studio or close the running terminal.  

2. **Delete the Project (Optional):**  
   - If no longer needed, remove the project folder.  

---

## **Step 9: Troubleshooting Tips**  

1. **Proxy Not Forwarding Requests?**  
   - Ensure the backend API is running on the expected port (`5001`).  
   - Check for typos in the YARP configuration.  

2. **Access Denied Errors?**  
   - Ensure firewall settings allow local requests.  

3. **Port Conflicts?**  
   - Change the listening port in `launchSettings.json` under the `Properties` folder.

---

## **Conclusion**  

Congratulations! You have successfully set up a simple **.NET 8 Web API** and configured **YARP** to act as a reverse proxy. You learned how to:  

- Set up a .NET 8 Web API.  
- Install and configure YARP.  
- Test API routing and proxying.  
- Enhance the setup with load balancing.  

---

## **Additional Resources**  

- [Microsoft YARP Documentation](https://microsoft.github.io/reverse-proxy/)  
- [.NET 8 API Documentation](https://learn.microsoft.com/en-us/aspnet/core/web-api/)  

