using ApiPhoneBook.Data;
using ApiPhoneBook.Interfaces;
using Microsoft.EntityFrameworkCore;
using PhoneBook.Data;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PhoneBookContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PhoneBookContext")
    ?? throw new InvalidOperationException("Connection string 'PhoneBookContext' not found.")));

//builder.Services.AddTransient<IContactData, ContactDataApi>();

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "PhoneBook",
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

}).AddSwaggerGenNewtonsoftSupport();          

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c=>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PhoneBook v1");
        c.DisplayOperationId();
    });
}
else {  app.UseExceptionHandler("/error"); }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();