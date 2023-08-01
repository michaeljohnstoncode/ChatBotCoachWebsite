
using ChatBotCoachWebsite.Helpers;
using ChatBotCoachWebsite.Helpers.Services;
using ChatBotCoachWebsite.Data;
using ChatBotCoachWebsite.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);



// database server connection stuff
//var connectionString = builder.Configuration.GetConnectionString("ChatBotCoachWebsiteContextConnection") ?? throw new InvalidOperationException("Connection string 'ChatBotCoachWebsiteContextConnection' not found.");
//azure server connectionstring config key
var connectionString = builder.Configuration["Server:ConnectionString"];

builder.Services.AddDbContext<ChatBotCoachWebsiteContext>(options => options.UseSqlServer(connectionString, providerOptions => providerOptions.EnableRetryOnFailure()));

builder.Services.AddDefaultIdentity<ChatBotCoachWebsiteUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ChatBotCoachWebsiteContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddRazorPages();
builder.Services.AddScoped<User>();
builder.Services.AddScoped<GetPineconeIndex>();
builder.Services.AddScoped<QueryPineconeIndex>();
builder.Services.AddScoped<IKeyProvider, TextFileKeyProvider>();
builder.Services.AddScoped<IOpenAIService, OpenAIService>();
builder.Services.AddScoped<IAiChatService, AiChatService>();
builder.Services.AddScoped<ISummarizeChatService, SummarizeMessageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapHub<ChatHub>("/chatHub");

app.Run();
