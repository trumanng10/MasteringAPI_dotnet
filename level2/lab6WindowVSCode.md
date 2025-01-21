### **Step-by-Step Guide: Setting Up a Simple .NET Web API and Exposing it via NGROK (Windows Users)**  

---

### **Lab Overview**  
This lab will guide Windows users through creating a simple .NET Web API and exposing it to the internet using NGROK. Participants will learn to develop, run, and securely share their local APIs for testing and integration purposes.  

---

### **Learning Objectives**  
By the end of this lab, participants will be able to:  
1. Install and set up a .NET Web API project on Windows.  
2. Use NGROK to expose the local API to the internet securely.  
3. Test the publicly accessible API endpoint using Postman or a web browser.  
4. Monitor incoming requests via NGROK's interface.  

---

### **Prerequisites**  
- Windows 10/11 installed.  
- .NET SDK (latest version) – [Download .NET SDK](https://dotnet.microsoft.com/download).  
- NGROK installed – [Download NGROK](https://ngrok.com/download).  
- Visual Studio (optional) or Visual Studio Code.  
- Basic knowledge of RESTful APIs and command-line usage.  

---

### **Step 1: Install Required Software**  

1. **Install .NET SDK**  
   - Download the .NET SDK from the official website and install it.  
   - Verify the installation by opening **Command Prompt (Win + R → cmd → Enter)** and running:  
     ```powershell
     dotnet --version
     ```
   - If installed correctly, it will display the installed version number.  

2. **Install NGROK**  
   - Download NGROK for Windows from the [NGROK website](https://ngrok.com/download).  
   - Extract the downloaded ZIP file and place `ngrok.exe` in a preferred directory (e.g., `C:\ngrok`).  
   - Add NGROK to the system PATH for easy access:  
     1. Search **"Environment Variables"**, open it.  
     2. Under "System Variables," find and edit the `Path` variable.  
     3. Add `C:\ngrok` to the list and save.  
   - Verify installation by running in Command Prompt:  
     ```powershell
     ngrok --version
     ```

---

### **Step 2: Create a Simple .NET Web API**  

1. **Open Command Prompt**  
   Press **Win + R**, type `cmd`, and press **Enter** to open the terminal.  

2. **Create a New Web API Project**  
   Navigate to a working directory and run the following command:  
   ```powershell
   dotnet new webapi -n SimpleDotNetAPI
   cd SimpleDotNetAPI
   ```

3. **Run the Application**  
   Start the web API by executing:  
   ```powershell
   dotnet run
   ```
   You will see output indicating the API is running, usually on URLs like:  
   ```
   Now listening on: http://localhost:5000
   Now listening on: https://localhost:5001
   ```

4. **Test the API Locally**  
   - Open your browser and go to:  
     ```
     http://localhost:5000/weatherforecast
     ```
   - You should see a JSON response with weather forecast data.  

---

### **Step 3: Expose the API Using NGROK**  

1. **Authenticate NGROK (First-time setup only)**  
   - Sign up for an NGROK account at [ngrok.com](https://ngrok.com/).  
   - Copy your authentication token from the dashboard.  
   - Run the following command to authenticate:  
     ```powershell
     ngrok authtoken YOUR_AUTH_TOKEN
     ```

2. **Expose the API to the Internet**  
   - Run NGROK to forward traffic from the internet to your local API port (e.g., `5000`):  
     ```powershell
     ngrok http 5000
     ```
   - You will see output like:  
     ```
     Forwarding  http://abc123.ngrok.io -> http://localhost:5000
     Forwarding  https://abc123.ngrok.io -> http://localhost:5000
     ```

3. **Test the Public URL**  
   - Open a browser or Postman and navigate to:  
     ```
     http://abc123.ngrok.io/weatherforecast
     ```
   - You should receive the same JSON response as before, confirming that your API is publicly accessible.  

---

### **Step 4: Testing the API via Postman or cURL**  

1. **Using Postman:**  
   - Open Postman and create a **GET** request with the NGROK URL:  
     ```
     http://abc123.ngrok.io/weatherforecast
     ```
   - Click **Send**, and the response should appear in JSON format.  

2. **Using cURL (Command Prompt):**  
   ```powershell
   curl http://abc123.ngrok.io/weatherforecast
   ```

---

### **Step 5: Monitor Traffic and Debugging**  

1. **Check NGROK Traffic Dashboard**  
   - Open NGROK's web interface in a browser:  
     ```
     http://127.0.0.1:4040
     ```
   - This will display incoming requests, response status, and headers, allowing you to debug requests.  

---

### **Step 6: Stop and Clean Up**  

1. To stop the NGROK tunnel, press `Ctrl + C` in the Command Prompt window running NGROK.  
2. To stop the running .NET API, press `Ctrl + C` in the Command Prompt running `dotnet run`.  
3. Delete the project folder if cleanup is needed.

---

### **Conclusion**  
You have successfully set up and exposed a simple .NET Web API using NGROK on Windows. This process allows you to securely share and test your local APIs with others over the internet without complex deployment setups.  

---

### **Additional Resources**  
- [Microsoft .NET Web API Documentation](https://learn.microsoft.com/en-us/aspnet/core/web-api/)  
- [NGROK Documentation](https://ngrok.com/docs)  

