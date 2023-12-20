using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Task_Management.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var key = Encoding.ASCII.GetBytes("qKHeRRZENb\r\nKi0oHPg91s\r\nGnk6Gp0xTj\r\nYvY9Xb4j3k\r\nUJOeOdsMK6\r\nl2N1cKT74m\r\n8azfcjVY1c\r\nY4tc1y22yU\r\nZbEE9V4qu3\r\nOUsa9sm668");
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


builder.Services.AddDbContext<AppDbContext>(db => db.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    //c.SwaggerDoc("v1", new OpenApiInfo { Title = ".Net Core 5", Version = "v1" });
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskManage", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
    {
    new OpenApiSecurityScheme
  {
    Reference=new OpenApiReference
    {
      Type=ReferenceType.SecurityScheme,
      Id="Bearer"
    }
  },
  new string[]{}
  }
});

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
