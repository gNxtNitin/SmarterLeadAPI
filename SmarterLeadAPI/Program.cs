using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SmarterLead.API.DataServices;
using Stripe;
using System.Text;

DotNetEnv.Env.Load();

StripeConfiguration.AppInfo = new AppInfo
{
    Name = "stripe-samples/checkout-single-subscription",
    Url = "https://github.com/stripe-samples/checkout-single-subscription",
    Version = "0.0.1",
};

var builder = WebApplication.CreateBuilder(args);
// Services for payment
builder.Services.Configure<StripeOptions>(options =>
{
    options.PublishableKey = Environment.GetEnvironmentVariable("STRIPE_PUBLISHABLE_KEY");
    options.SecretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
    options.WebhookSecret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET");
    options.BasicPrice = Environment.GetEnvironmentVariable("BASIC_PRICE_ID");
    options.SilverPrice = Environment.GetEnvironmentVariable("SILVER_PRICE_ID");
    options.GoldPrice = Environment.GetEnvironmentVariable("Gold_PRICE_ID");
    options.ProPrice = Environment.GetEnvironmentVariable("PRO_PRICE_ID");
    options.Domain = Environment.GetEnvironmentVariable("DOMAIN");
});

// Add services to the container.
var connStr = builder.Configuration.GetConnectionString("connStr");
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connStr,ServerVersion.AutoDetect(connStr)).EnableDetailedErrors()); // Adjust the version as necessary

var key = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("jwt:key"));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseSerilog();

var app = builder.Build();

Log.Logger = new LoggerConfiguration().
               WriteTo.File($"{ app.Configuration.GetValue<string>("LogPath")}//log.txt", rollingInterval: RollingInterval.Day)
               .CreateLogger();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
