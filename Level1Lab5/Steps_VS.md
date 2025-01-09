### Step-by-Step Guide: Building a Todo API with ASP.Net Core Minimal APIs

---

#### **Step 1: Project Setup**
1. Open **Visual Studio 2022**.
2. Click **Create a new project**.
3. Search for **ASP.Net Core Empty** and select the **ASP.Net Core Empty Project** template.
4. Click **Next**.
5. Name the project **TodoAPI**.
6. Leave the default configurations as they are and click **Create**.
7. Verify that the project contains only the `Program.cs` file (minimal setup).

---

#### **Step 2: Model Creation**
1. Right-click the **project name** in the Solution Explorer.
2. Select **Add > New Item..**, and name it `TodoItem.cs`.
3. Define the following properties in the `TodoItem` class:
   ```csharp
   namespace TodoAPI;

   public class ToDoItem
   {
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsCompleted { get; set; }
   }
   ```

---

#### **Step 3: Database Setup**
1. **Install Entity Framework Core In-Memory Database**:
   - Right-click the **project name**.
   - Select **Manage NuGet Packages**.
   - Search(Click 'Browse') for `Microsoft.EntityFrameworkCore.InMemory`.
   - Click **Install**, then accept the terms.
2. **Create TodoDbContext**:
   - Double Click the **project name** to verify Packages installed. Right-click the **project name**, select **Add > New Item**, and name it `TodoDbContext.cs`.
   - Define the class as follows:
     ```csharp
     using Microsoft.EntityFrameworkCore;

     namespace TodoAPI;
       
     public class TodoDbContext : DbContext
     {
         public TodoDbContext(DbContextOptions<TodoDbContext> options)
         : base(options) { }
     
         public DbSet<TodoItem> TodoItems { get; set; }
     }
     ```
3. **Register the DbContext**:
   - Open `Program.cs` and register the DbContext in the dependency injection container:
     ```csharp
     builder.Services.AddDbContext<TodoDbContext>(options =>
         options.UseInMemoryDatabase("TodoList"));
     ```

---

#### **Step 4: Configure the HTTP Request Pipeline**
1. Open `Program.cs`.
2. Configure middleware using `app.Use...` methods:
   ```csharp
   var app = builder.Build();

   app.UseHttpsRedirection();

   app.Run();
   ```

---

#### **Step 5: CRUD Endpoints**
1. **Add CRUD Endpoints**:
   - Define the following endpoints in `Program.cs`:
     ```csharp
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

#### **Step 6: Data Handling**
- Use `TodoDbContext` in the above CRUD endpoints to interact with the in-memory database.

---

### **Outcome**
- A functional **Todo Web API** with CRUD operations:
  - `GET` to retrieve all or specific Todo items.
  - `POST` to create new items.
  - `PUT` to update existing items.
  - `DELETE` to remove items.
- Simplified database handling using **Entity Framework Core In-Memory Database**.
