### **Monolithic Code Example**

#### **1. Project Structure (Monolithic)**
```
MyMonolithicApp/
├── Controllers/
│   ├── AppController.cs
├── Models/
│   ├── User.cs
│   ├── Order.cs
├── Services/
│   ├── AppService.cs
├── Program.cs
├── Startup.cs
```

---

#### **2. Models**

- **User.cs**
```csharp
namespace MyMonolithicApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
```

- **Order.cs**
```csharp
namespace MyMonolithicApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Product { get; set; }
        public decimal Amount { get; set; }
    }
}
```

---

#### **3. AppController**

This controller handles **both users and orders**, violating the **single responsibility principle**.

```csharp
using Microsoft.AspNetCore.Mvc;
using MyMonolithicApp.Models;
using MyMonolithicApp.Services;

namespace MyMonolithicApp.Controllers
{
    [ApiController]
    [Route("api/app")]
    public class AppController : ControllerBase
    {
        private readonly AppService _appService;

        public AppController()
        {
            _appService = new AppService(); // Direct instantiation (anti-pattern)
        }

        [HttpGet("users/{id}")]
        public IActionResult GetUser(int id)
        {
            var user = _appService.GetUserById(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPost("users")]
        public IActionResult CreateUser(User user)
        {
            var createdUser = _appService.CreateUser(user);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }

        [HttpGet("orders/{id}")]
        public IActionResult GetOrder(int id)
        {
            var order = _appService.GetOrderById(id);
            if (order == null)
                return NotFound();
            return Ok(order);
        }

        [HttpPost("orders")]
        public IActionResult CreateOrder(Order order)
        {
            var createdOrder = _appService.CreateOrder(order);
            return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.Id }, createdOrder);
        }
    }
}
```

---

#### **4. AppService**

A single service handles both **users** and **orders**, leading to bloated and difficult-to-maintain logic.

```csharp
using System.Collections.Generic;
using System.Linq;
using MyMonolithicApp.Models;

namespace MyMonolithicApp.Services
{
    public class AppService
    {
        private readonly List<User> _users = new();
        private readonly List<Order> _orders = new();

        // User Methods
        public User GetUserById(int id) => _users.FirstOrDefault(u => u.Id == id);

        public User CreateUser(User user)
        {
            user.Id = _users.Count + 1;
            _users.Add(user);
            return user;
        }

        // Order Methods
        public Order GetOrderById(int id) => _orders.FirstOrDefault(o => o.Id == id);

        public Order CreateOrder(Order order)
        {
            order.Id = _orders.Count + 1;
            _orders.Add(order);
            return order;
        }
    }
}
```

---

### **Issues with Monolithic Design**
1. **Tightly Coupled Components:**
   - All functionalities are packed into a single service and controller, making it hard to maintain or test specific features.

2. **No Clear Separation of Concerns:**
   - The `AppController` and `AppService` handle unrelated concerns (e.g., both users and orders).

3. **Difficult to Scale:**
   - Scaling specific functionalities (e.g., orders) requires scaling the entire monolithic application.

4. **Lack of Modularization:**
   - Adding new features or making changes can introduce bugs across unrelated functionalities.

