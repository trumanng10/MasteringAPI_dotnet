# **Lab 6: Setting Up a Simple .NET 8 Web API with YARP on Linux (Without NGROK)**

---

## **Lab Overview**  
In this lab, you'll learn how to set up a simple **.NET 8 Web API** and configure **YARP** to act as a reverse proxy. This will help you understand request routing, proxying API requests, and handling multiple backend services using YARP.

---

## **Learning Objectives**  

By the end of this lab, you will be able to:  

1. Install and configure .NET 8 SDK on Linux.  
2. Create and run a simple .NET 8 Web API.  
3. Install and configure YARP as a reverse proxy.  
4. Test and validate proxy functionality using cURL.  

---

## **Prerequisites**  

Ensure the following before starting the lab:  

1. **Ubuntu/Debian/Linux Mint** or **RHEL/CentOS/Fedora** machine.  
2. Installed **.NET 8 SDK** – Download from [https://dotnet.microsoft.com/en-us/download](https://dotnet.microsoft.com/en-us/download).  
3. Basic knowledge of using the Linux terminal.  
4. Installed cURL for testing API requests.  

---

## **Step 1: Install .NET 8 SDK on Linux**  

### **Ubuntu/Debian**  
Open the terminal and run the following commands to install the .NET SDK:  

```bash
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
sudo apt install -y dotnet-sdk-8.0
```

### **RHEL/CentOS/Fedora**  

```bash
sudo dnf install -y dotnet-sdk-8.0
```

### **Verify Installation**  

```bash
dotnet --version
```

Ensure the output shows `8.x.x` indicating successful installation.

---

## **Step 2: Create a .NET 8 Web API Project**  

1. Open the terminal and create a new directory for the project:  

   ```bash
   mkdir SimpleYarpApi && cd SimpleYarpApi
   ```

2. Create a new Web API project:  

   ```bash
   dotnet new webapi -n SimpleYarpApi
   cd SimpleYarpApi
   ```

3. Run the Web API to verify it works:  

   ```bash
   dotnet run
   ```

4. The API should start on a local address (e.g., `http://localhost:5000`). Test it using:  

   ```bash
   curl http://localhost:5000/weatherforecast
   ```

---

## **Step 3: Install YARP Package**  

Install YARP using the .NET CLI:

```bash
dotnet add package Yarp.ReverseProxy
```

---

## **Step 4: Configure YARP in .NET Web API**  

1. Open the `Program.cs` file in your preferred editor (e.g., `nano` or `vim`):  

   ```bash
   nano Program.cs
   ```

2. Modify the file to include YARP configuration:

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

3. Save and exit the file (`CTRL+X`, then `Y` and `Enter` in nano).

---

## **Step 5: Run the Reverse Proxy Application**  

1. Start the proxy server by running:  

   ```bash
   dotnet run
   ```

2. Test the proxy by sending a request:  

   ```bash
   curl http://localhost:5000/proxy/weatherforecast
   ```

   You should see the API response forwarded via the proxy.

---

## **Step 6: Testing and Debugging**  

1. Check the logs in the terminal to verify requests are being proxied correctly.  
2. Run the following command to check running services:  

   ```bash
   lsof -i :5000
   ```

3. Test with different tools like **Postman** or **HTTPie** to ensure the proxy is working.

---

## **Step 7: Adding Load Balancing (Optional)**  

To configure YARP for load balancing, modify the `Program.cs` file:

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

---

## **Step 8: Securing the Proxy with HTTPS**  

1. Modify `Program.cs` to enable HTTPS redirection:  

   ```csharp
   app.UseHttpsRedirection();
   ```

2. Generate an SSL certificate (for testing purposes):  

   ```bash
   dotnet dev-certs https --trust
   ```

3. Test HTTPS with:  

   ```bash
   curl -k https://localhost:5001/proxy/weatherforecast
   ```

---

## **Step 9: Stop and Cleanup**  

1. Stop the running process by pressing `Ctrl + C`.  
2. To remove the project, delete the project folder:  

   ```bash
   rm -rf SimpleYarpApi
   ```

---

## **Step 10: Troubleshooting Tips**  

1. **Common Errors and Fixes:**  
   - **"Port already in use" Error:**  
     - Find and kill the process:  
       ```bash
       sudo lsof -i :5000
       sudo kill -9 <PID>
       ```

   - **"Command not found" Error:**  
     - Ensure `dotnet` is installed and available in `PATH`:  
       ```bash
       export PATH=$PATH:$HOME/dotnet
       ```

2. **Test Different Endpoints:**  
   - Run `curl http://localhost:5000/proxy/weatherforecast` and check response.  

---

## **Conclusion**  

Congratulations! You've successfully set up a **.NET 8 Web API** and configured **YARP** as a reverse proxy on Linux. You learned how to:  

- Install and configure .NET 8 on Linux.  
- Set up a Web API project.  
- Install and configure YARP for request forwarding.  
- Test API proxy functionality using Linux command-line tools.  

---

## **Additional Resources**  

- [Microsoft YARP Documentation](https://microsoft.github.io/reverse-proxy/)  
- [.NET 8 Documentation](https://learn.microsoft.com/en-us/dotnet/core/)  
- [Linux Terminal Cheat Sheet](https://www.gnu.org/software/bash/manual/bash.html)  
