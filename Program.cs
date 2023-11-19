using ApiPhoneBook.Data;
using ApiPhoneBook.Middlewares;
using ApiPhoneBook.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PhoneBook.Data;
using PhoneBook.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

string conetcion = builder.Configuration.GetConnectionString("PhoneBookContext");

builder.Services.AddDbContext<PhoneBookContext>(options =>
    options.UseSqlServer(conetcion
    ?? throw new InvalidOperationException("Connection string 'PhoneBookContext' not found.")));

string conetcion2 = builder.Configuration.GetConnectionString("UserApp");

builder.Services.AddDbContext<UsersContext>(options =>
    options.UseSqlServer(conetcion2)); 



builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Contact>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "PhoneBookApi",
        Description = "Модуль по web-Api",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        { Name = "Ivan Nikonov", Email = "cmn.nia@gmail.com" },
        Version = "v1"
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    c.IncludeXmlComments(xmlPath);

    c.CustomOperationIds(apiDascription =>
    {
        return apiDascription.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
    });

    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    c.OperationFilter<SecurityRequirementsOperationFilter>();

}).AddSwaggerGenNewtonsoftSupport();

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration.GetSection("AppSettings:Token").Value!))
    };
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c=>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PhoneBook v1");
        c.DisplayOperationId();
    });
    app.UseExceptionHandler("/error-development");
}
else {  app.UseExceptionHandler("/error"); }

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
