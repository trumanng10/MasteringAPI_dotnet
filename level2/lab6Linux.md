### Step-by-Step Guide: Setting Up a Simple .NET Web API and Exposing it via NGROK  

---

### **Lab Overview**  
This lab introduces you to building and deploying a simple .NET Web API and exposing it securely over the internet using NGROK. Participants will gain hands-on experience with .NET Core, Web API creation, and NGROK for tunneling.  

---

### **Learning Objectives**  
By the end of this lab, participants will be able to:  
1. Set up and run a basic .NET Web API project.  
2. Configure NGROK to expose local web servers securely.  
3. Understand how to integrate NGROK with APIs for public access.  
4. Test API endpoints using tools like Postman or cURL.  

---

### **Prerequisites**  
- Visual Studio or Visual Studio Code installed.  
- .NET SDK (latest version).  
- NGROK installed on your machine.  
- Basic knowledge of C# and RESTful APIs.  

---

### **Step 1: Create a Simple .NET Web API**  

1. **Open Terminal or Command Prompt**  
   Navigate to the directory where you want to create the project.  

2. **Create a New Web API Project**  
   Run the following command:  
   ```bash
   dotnet new webapi -n SimpleDotNetAPI
   cd SimpleDotNetAPI
   ```

3. **Run the Application**  
   Start the application using:  
   ```bash
   dotnet run
   ```
   Note the default URL (e.g., `http://localhost:5000` or `http://localhost:5001` for HTTPS).  

4. **Test the API Locally**  
   Open a browser or a tool like Postman and navigate to:  
   ```bash
   http://localhost:5000/weatherforecast
   ```
   You should see a sample JSON response.

---

### **Step 2: Install and Configure NGROK**  

1. **Download NGROK**  
   If NGROK is not already installed, download it from [NGROK's official website](https://ngrok.com/download) and follow the installation instructions.  

2. **Authenticate NGROK**  
   Sign up for a free NGROK account and get your auth token.  
   Run the following command to authenticate:  
   ```bash
   ngrok authtoken YOUR_AUTH_TOKEN
   ```

3. **Expose the API Using NGROK**  
   Run NGROK with the following command, pointing to your API’s port (e.g., 5000):  
   ```bash
   ngrok http 5000
   ```
   NGROK will provide a public URL like `http://abc123.ngrok.io`.  

4. **Verify NGROK URL**  
   Open the public URL in a browser or use Postman to test the `/weatherforecast` endpoint:  
   ```bash
   http://abc123.ngrok.io/weatherforecast
   ```

---

### **Step 3: Test and Verify the API**  

1. **Using Postman or cURL**  
   - Open Postman, create a new GET request, and paste the NGROK URL.  
   - Alternatively, use cURL:  
     ```bash
     curl http://abc123.ngrok.io/weatherforecast
     ```

2. **Check Logs in NGROK Console**  
   The NGROK terminal will display logs of incoming requests, allowing you to monitor traffic in real time.

---

### **Step 4: Stop and Clean Up**  

1. Stop the NGROK process by pressing `Ctrl + C`.  
2. Stop the .NET API by pressing `Ctrl + C` in the terminal running the `dotnet run` command.  

---

### **Conclusion**  
This guide provides a practical introduction to creating and deploying a simple .NET Web API and securely exposing it using NGROK. You can now experiment with more advanced API features and configurations using these tools.  


