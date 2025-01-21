### **Step-by-Step Guide: Setting Up a Simple .NET Web API and Exposing it via NGROK (Windows with Visual Studio)**  

---

### **Lab Overview**  
This lab will guide you through creating a simple .NET Web API using Visual Studio on Windows and exposing it securely to the internet using NGROK. Participants will learn how to develop, run, and share their local APIs for external access and testing.  

---

### **Learning Objectives**  
By the end of this lab, participants will be able to:  
1. Install and set up a .NET Web API project using Visual Studio.  
2. Configure and run NGROK to expose the API to the internet securely.  
3. Test the publicly accessible API using Postman or a web browser.  
4. Monitor incoming API traffic using NGROK's interface.  

---

### **Prerequisites**  
- Windows 10/11  
- Visual Studio (latest version) with ASP.NET and web development workload installed  
- .NET SDK (included with Visual Studio installation)  
- NGROK installed – [Download NGROK](https://ngrok.com/download)  
- Basic knowledge of RESTful APIs and Visual Studio usage  

---

### **Step 1: Install Required Software**  

1. **Install Visual Studio**  
   - Download and install Visual Studio from [https://visualstudio.microsoft.com/](https://visualstudio.microsoft.com/).  
   - During installation, ensure the **"ASP.NET and web development"** workload is selected.  
   
2. **Install NGROK**  
   - Download NGROK from [https://ngrok.com/download](https://ngrok.com/download).  
   - Extract the ZIP file and place `ngrok.exe` in `C:\ngrok` or any preferred directory.  
   - Add NGROK to system PATH:  
     1. Search **"Environment Variables"**, open it.  
     2. Under "System Variables," find and edit the `Path` variable.  
     3. Add `C:\ngrok` to the list and save changes.  
   - Open Command Prompt and verify installation:  
     ```powershell
     ngrok --version
     ```

---

### **Step 2: Create a Simple .NET Web API in Visual Studio**  

1. **Open Visual Studio**  
   - Launch **Visual Studio** and select **"Create a new project."**  

2. **Create a New Web API Project**  
   - Select **ASP.NET Core Web API**, click **Next**.  
   - Enter a project name (e.g., `SimpleDotNetAPI`) and location, click **Next**.  
   - Choose **.NET 6.0 or later**, and click **Create**.  

3. **Run the Web API**  
   - Press **F5** or click **Run** to start the application.  
   - By default, the API will run on ports such as:  
     ```
     http://localhost:5000
     https://localhost:5001
     ```
   - Open a browser and visit:  
     ```
     http://localhost:5000/weatherforecast
     ```
   - You should see a JSON response with weather data.  

---

### **Step 3: Expose the API Using NGROK**  

1. **Authenticate NGROK (First-time setup only)**  
   - Sign up for an NGROK account at [https://ngrok.com/](https://ngrok.com/).  
   - Copy your authentication token from the NGROK dashboard.  
   - Open Command Prompt and enter:  
     ```powershell
     ngrok authtoken YOUR_AUTH_TOKEN
     ```

2. **Expose the Local API Using NGROK**  
   - In Visual Studio, check which port your API is running on (e.g., `5000`).  
   - Open Command Prompt and run the following command:  
     ```powershell
     ngrok http 5000
     ```
   - NGROK will generate a public URL such as:  
     ```
     Forwarding  http://abc123.ngrok.io -> http://localhost:5000
     ```
   - Copy the public NGROK URL and use it to access your API from anywhere.  

3. **Test the Public API URL**  
   - Open the generated NGROK URL in a web browser:  
     ```
     http://abc123.ngrok.io/weatherforecast
     ```
   - You should see the same JSON output as before.  

---

### **Step 4: Testing the API with Postman or cURL**  

1. **Using Postman:**  
   - Open Postman and create a **GET** request.  
   - Enter the NGROK public URL (e.g., `http://abc123.ngrok.io/weatherforecast`).  
   - Click **Send** to receive the API response.  

2. **Using cURL:**  
   Open Command Prompt and run:  
   ```powershell
   curl http://abc123.ngrok.io/weatherforecast
   ```

---

### **Step 5: Monitor and Debug Requests**  

1. **Using NGROK's Web Interface**  
   - NGROK provides a local web interface for monitoring requests.  
   - Open your browser and visit:  
     ```
     http://127.0.0.1:4040
     ```
   - This dashboard shows request logs, headers, and response details.  

---

### **Step 6: Stop and Cleanup**  

1. Stop the NGROK session by pressing `Ctrl + C` in the Command Prompt window.  
2. Stop the .NET API by closing Visual Studio or pressing the **Stop** button.  
3. Delete the project folder if necessary.  

---

### **Step 7: Additional Configurations (Optional)**  

- **Change API Port:**  
  To change the default port, open the `launchSettings.json` file in the `Properties` folder and modify the port under `"applicationUrl"`.  
- **Enable HTTPS:**  
  Use the `https` NGROK tunnel for secure API access:  
  ```powershell
  ngrok http https://localhost:5001
  ```

---

### **Conclusion**  
You have successfully created and exposed a .NET Web API using Visual Studio and NGROK. This setup allows you to securely share your API for remote testing and development.  

---

### **Troubleshooting Tips**  

1. **NGROK Tunnel Not Working?**  
   - Ensure your firewall isn't blocking NGROK connections.  
   - Restart NGROK with the correct port.  

2. **API Not Responding?**  
   - Check if the API is running by visiting `http://localhost:5000` in your browser.  
   - Ensure your Visual Studio project is properly set up.  

3. **Access Denied Error?**  
   - Run NGROK as Administrator if necessary.  
   - Ensure that Visual Studio is not blocking external requests.  

---

### **Additional Resources**  
- [Microsoft .NET Web API Documentation](https://learn.microsoft.com/en-us/aspnet/core/web-api/)  
- [NGROK Documentation](https://ngrok.com/docs)  

---

Let me know if you need further assistance!
