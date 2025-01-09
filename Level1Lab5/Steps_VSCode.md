Here’s how you can create the **Todo API** in **Visual Studio Code** using ASP.Net Core Minimal APIs:

---

### **Step 1: Project Setup**
1. **Install .NET SDK**:
   - Make sure you have the latest [.NET SDK](https://dotnet.microsoft.com/download) installed on your machine.

2. **Create a New Project**:
   - Open a terminal in VS Code or use an external terminal.
   - Run the following commands:
     ```bash
     mkdir TodoAPI
     cd TodoAPI
     dotnet new web -o TodoAPI --minimal
     ```
   - This will create a minimal API project with just `Program.cs`.

3. **Open Project in VS Code**:
   - Run:
     ```bash
     code .
     ```
   - This will open the TodoAPI project in VS Code.

---

### **Step 2: Install Dependencies**
1. **Install Entity Framework Core In-Memory Database**:
   - Run the following command in the terminal:
     ```bash
     dotnet add package Microsoft.EntityFrameworkCore.InMemory
     ```
   - This will add the required package for the in-memory database.

---

### **Step 3: Create the Model**
1. **Add the TodoItem Model**:
   - In the VS Code Explorer, create a new file named `TodoItem.cs`.
   - Add the following code:
     ```csharp
     public class TodoItem
     {
         public int Id { get; set; }
         public string Name { get; set; }
         public bool IsCompleted { get; set; }
     }
     ```

---

### **Step 4: Create the Database Context**
1. **Add TodoDbContext**:
   - Create a new file named `TodoDbContext.cs`.
   - Add the following code:
     ```csharp
     using Microsoft.EntityFrameworkCore;

     public class TodoDbContext : DbContext
     {
         public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }
         public DbSet<TodoItem> TodoItems { get; set; }
     }
     ```

---

### **Step 5: Register Services and Configure the Pipeline**
1. Open `Program.cs` and modify it as follows:
   ```csharp
   using Microsoft.EntityFrameworkCore;

   var builder = WebApplication.CreateBuilder(args);

   // Register DbContext
   builder.Services.AddDbContext<TodoDbContext>(options =>
       options.UseInMemoryDatabase("TodoList"));

   var app = builder.Build();

   // Configure Middleware
   app.UseHttpsRedirection();

   // CRUD Endpoints
   app.MapGet("/todoitems", async (TodoDbContext db) =>
       await db.TodoItems.ToListAsync());

   app.MapGet("/todoitems/{id}", async (int id, TodoDbContext db) =>
       await db.TodoItems.FindAsync(id) is TodoItem todo
           ? Results.Ok(todo)
           : Results.NotFound());

   app.MapPost("/todoitems", async (TodoItem todo, TodoDbContext db) =>
   {
       db.TodoItems.Add(todo);
       await db.SaveChangesAsync();
       return Results.Created($"/todoitems/{todo.Id}", todo);
   });

   app.MapPut("/todoitems/{id}", async (int id, TodoItem updatedTodo, TodoDbContext db) =>
   {
       var todo = await db.TodoItems.FindAsync(id);
       if (todo is null) return Results.NotFound();

       todo.Name = updatedTodo.Name;
       todo.IsCompleted = updatedTodo.IsCompleted;
       await db.SaveChangesAsync();
       return Results.NoContent();
   });

   app.MapDelete("/todoitems/{id}", async (int id, TodoDbContext db) =>
   {
       var todo = await db.TodoItems.FindAsync(id);
       if (todo is null) return Results.NotFound();

       db.TodoItems.Remove(todo);
       await db.SaveChangesAsync();
       return Results.NoContent();
   });

   app.Run();
   ```

---

### **Step 6: Run the Application**
1. Open the terminal and run:
   ```bash
   dotnet run
   ```
2. The application will start, and you'll see output indicating the API is running on `https://localhost:5001` (or similar).

---

### **Step 7: Test the API**
1. Use a tool like **Postman** or **cURL** to test the endpoints:
   - `GET /todoitems` – Retrieve all items.
   - `GET /todoitems/{id}` – Retrieve a specific item.
   - `POST /todoitems` – Create a new item (send JSON payload).
   - `PUT /todoitems/{id}` – Update an existing item.
   - `DELETE /todoitems/{id}` – Delete an item.

Example JSON payload for `POST`:
```json
{
    "id": 1,
    "name": "Buy groceries",
    "isCompleted": false
}
```

---

### **Tips for VS Code**:
1. **Enable IntelliSense**:
   - Ensure the C# extension is installed in VS Code for syntax highlighting and IntelliSense.
2. **Debugging**:
   - Press `F5` to start debugging. Configure `launch.json` if needed (VS Code will prompt you).
3. **Hot Reload**:
   - Use `dotnet watch run` to enable hot reload during development.

With these steps, your Todo API will be functional and ready to use!
