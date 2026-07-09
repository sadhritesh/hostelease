
using HostelEase.UI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddIdentityServices();
builder.Services.AddApplicationServices();
builder.Services.AddMvcServices();

var app = builder.Build();

await DatabaseInitializer.InitilizeAsync(app);

app.UseApplicationPipeline();

app.Run();



