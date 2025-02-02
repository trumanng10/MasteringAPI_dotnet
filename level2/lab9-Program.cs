// Login
app.MapPost("/login", async (HttpContext context) =>
{
    var form = await context.Request.ReadFormAsync();
    var username = form["username"];
    var password = form["password"];

    if (username == "admin" && password == "admin")
    {
        context.Response.Redirect("/authenticated");
    }
    else
    {
        context.Response.Redirect("/loginfailed");
    }
});

app.MapGet("/authenticated", () => Results.Json(new { Message = "You are authenticated!" }));

app.MapGet("/loginfailed", () => Results.Json(new { Message = "Invalid credentials. Try again." }));
