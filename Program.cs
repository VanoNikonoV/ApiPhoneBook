using ApiPhoneBook.Data;
using ApiPhoneBook.Middlewares;
using ApiPhoneBook.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PhoneBook.Data;
using PhoneBook.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

string conetcion = builder.Configuration.GetConnectionString("PhoneBookContext");

builder.Services.AddDbContext<PhoneBookContext>(options =>
    options.UseSqlServer(conetcion
    ?? throw new InvalidOperationException("Connection string 'PhoneBookContext' not found.")));

builder.Services.AddDbContext<UsersContext>(options =>
    options.UseSqlite("Data Source=userApp.db")); 


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
        Description = "������ �� web-Api",
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
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // ��������, ����� �� �������������� �������� ��� ��������� ������
        ValidateIssuer = true,
        // ������, �������������� ��������
        ValidIssuer = AuthOptions.ISSUER,

        // ����� �� �������������� ����������� ������
        ValidateAudience = true,
        // ��������� ����������� ������
        ValidAudience = AuthOptions.AUDIENCE,
        // ����� �� �������������� ����� �������������
        ValidateLifetime = true,

        // ��������� ����� ������������
        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
        // ��������� ����� ������������
        ValidateIssuerSigningKey = true,


        //ValidateIssuerSigningKey = true,
        //ValidateAudience = false,
        //ValidateIssuer = false,
        //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
        //        builder.Configuration.GetSection("AppSettings:Token").Value!))
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

    app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
