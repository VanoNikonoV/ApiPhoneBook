using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using PhoneBook.Data;
using PhoneBook.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PhoneBookContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PhoneBookContext")
    ?? throw new InvalidOperationException("Connection string 'PhoneBookContext' not found.")));

//builder.Services.AddTransient<IContactData, ContactDataApi>();

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

}).AddSwaggerGenNewtonsoftSupport();          

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
