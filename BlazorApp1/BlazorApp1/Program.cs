using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Extensions;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using practice4;
using practice4.Client.Service;
using practice4.Components;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrapProviders()
    .AddFontAwesomeIcons();
builder.Services.AddScoped(sp => new HttpClient { });
builder.Services.AddSingleton<IAccount, Account>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var db = new DatabaseContext())
{
    Console.WriteLine(db.Model.ToDebugString());
}
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(practice4.Client._Imports).Assembly);

app.MapPost("/register", async (HttpContext context) =>
{
    var form = context.Request.Form;
    
    using (var db = new DatabaseContext())
    {
        if (db.users.Any(user => user.FIO == (string)form["FIO"] || user.Login == (string)form["Login"]))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        db.users.Add(new User() { FIO = (string)form["FIO"], Password = (string)form["Password"], Login = (string)form["Login"] });
        await db.SaveChangesAsync();
    }
    context.Response.StatusCode = StatusCodes.Status200OK;
});

app.MapPost("/login", async (HttpContext context) =>
{
    var form = context.Request.Form;
    using (var db = new DatabaseContext())
    {
        if (db.users.Count(user => user.Login == (string)form["Login"]) != 1)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }
        if (db.users.Single(user => user.Login == (string)form["Login"]).Password != form["Password"])
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        User foundUser = db.users.Single(user => user.Login == (string)form["Login"]);
        foundUser.Password = "";
        await context.Response.WriteAsJsonAsync(foundUser);
    }
});

app.MapPost("/message", async (HttpContext context) =>
{
    var form = context.Request.Form;
    string recieverLogin = form["recieverLogin"];
    string title = form["title"];
    string content = form["content"];
    string senderLogin = form["senderLogin"];
    using (var db = new DatabaseContext())
    {
        if (title.IsNullOrEmpty() || content.IsNullOrEmpty() || recieverLogin.IsNullOrEmpty())
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }
        if (db.users.Count(user => user.Login == recieverLogin) != 1) 
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }
        if (db.users.Count(user => user.Login == senderLogin) != 1)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }
        int recieverID = db.users.Single(user => user.Login == recieverLogin).Id;
        int senderID = db.users.Single(user => user.Login == senderLogin).Id;

        db.messages.Add(new() { ReceiverId = recieverID, SenderId = senderID, Title = title, Content = content, Timestamp = DateTime.Now.ToUniversalTime()});
        await db.SaveChangesAsync();
        context.Response.StatusCode = StatusCodes.Status200OK;
        return;
    }
});

app.MapGet("/message", async (HttpContext context) =>
{
    string? login = context.Request.Query["login"];
    if (login.IsNullOrEmpty())
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
    }
    using (var db = new DatabaseContext())
    {
        if (db.users.Count(user => user.Login == login) != 1)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        List<Message> messages =
        (from message in db.messages
        join user in db.users on message.ReceiverId equals user.Id
        where user.Login == login
        select message).ToList();
        await context.Response.WriteAsJsonAsync(messages);
        return;
    }
});
app.MapPost("/read_message", async (HttpContext context) =>
{
    int? id = int.Parse(context.Request.Query["id"]);
    using (var db = new DatabaseContext())
    {
        Message message = db.messages.Single(message => message.Id == id);
        message.isRead = true;
        await db.SaveChangesAsync();
    }
});
app.MapGet("/fio_by_id", async (HttpContext context) =>
{
    int? id = int.Parse(context.Request.Query["id"]);

    using (var db = new DatabaseContext())
    {
        if (db.users.Count(user => user.Id == id) != 1)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }
        var fio = db.users.Single(user => user.Id == id).FIO;
        await context.Response.WriteAsync(fio);
        return;
    }
});
app.MapGet("/login_by_id", async (HttpContext context) =>
{
    int? id = int.Parse(context.Request.Query["id"]);

    using (var db = new DatabaseContext())
    {
        if (db.users.Count(user => user.Id == id) != 1)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }
        var login = db.users.Single(user => user.Id == id).Login;
        await context.Response.WriteAsync(login);
        return;
    }
});
app.Run();
