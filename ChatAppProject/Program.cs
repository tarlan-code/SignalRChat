using ChatAppProject.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(pol =>
    {
        pol.AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed(o => true);
    });
});

builder.Services.AddSignalR();
var app = builder.Build();

app.UseCors();
app.UseRouting();

app.MapHub<ChatHub>("/chathub");
app.Run();
