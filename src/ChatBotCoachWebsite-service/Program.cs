
using ChatBotCoachWebsite.Helpers;
using ChatBotCoachWebsite.Helpers.Services;
using ChatBotCoachWebsite.Data;
using ChatBotCoachWebsite.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;


// database server connection stuff
var connectionString = builder.Configuration["Server:ConnectionString"];

services.AddDbContext<ChatBotCoachWebsiteContext>(options => options.UseSqlServer(connectionString, providerOptions => providerOptions.EnableRetryOnFailure()));

services.AddDefaultIdentity<ChatBotCoachWebsiteUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ChatBotCoachWebsiteContext>();

// Add services to the container.
services.AddControllersWithViews();
services.AddSignalR();
services.AddRazorPages();
services.AddScoped<User>();
services.AddScoped<GetPineconeIndex>();
services.AddScoped<QueryPineconeIndex>();
services.AddScoped<IKeyProvider, TextFileKeyProvider>();
services.AddScoped<IOpenAIService, OpenAIService>();
services.AddScoped<IAiChatService, AiChatService>();
services.AddScoped<ISummarizeChatService, SummarizeMessageService>();
services.AddTransient<IEmailService, EmailService>();


//external login authentication services
var config = builder.Configuration;

services.AddAuthentication()
   .AddGoogle(options =>
   {
       options.ClientId = config["Authentication:Google:ClientId"];
       options.ClientSecret = config["Authentication:Google:ClientSecret"];
   })
   .AddMicrosoftAccount(microsoftOptions =>
   {
       microsoftOptions.ClientId = config["Authentication:Microsoft:ClientId"];
       microsoftOptions.ClientSecret = config["Authentication:Microsoft:ClientSecret"];
   })
   .AddTwitter(twitterOptions =>
   {
       twitterOptions.ConsumerKey = config["Authentication:Twitter:ConsumerAPIKey"];
       twitterOptions.ConsumerSecret = config["Authentication:Twitter:ConsumerSecret"];
       twitterOptions.RetrieveUserDetails = true;
   });

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
