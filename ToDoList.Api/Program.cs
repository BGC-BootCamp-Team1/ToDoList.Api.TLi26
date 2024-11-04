using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using ToDoList.Api;
using ToDoList.Api.Services;
using ToDoList.Core;
using ToDoList.Core.DueDateSettingStrategy;
using ToDoList.Infrastructure;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDoList API", Version = "v1",Description="A set of API to manage todo items.", 
    Contact = new OpenApiContact { Name = "Xiangyi Kong", Email = "xkong4@slb.com"}
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. (Do NOT input Bearer)",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters.IssuerSigningKey = TokenUtil.AesKey;
    options.TokenValidationParameters.ValidateAudience = false;
    options.TokenValidationParameters.ValidateIssuer = false;

});
builder.Services.AddAuthorization();

builder.Services.AddSingleton<IToDoItemService, ToDoItemService>();
builder.Services.AddSingleton<INewTodoItemService, NewTodoItemService>();
builder.Services.AddSingleton<ITodoItemsRepository, TodoItemMongoRepository>();

// Register the strategy classes
//builder.Services.AddSingleton<IDueDateSettingStrategy, FirstAvailableDayStrategy>();
//builder.Services.AddSingleton<FewestTodoItemsDayStrategy>();

builder.Services.Configure<TodoStoreDatabaseSettings>(builder.Configuration.GetSection("ToDoItemDatabase"));
var app = builder.Build();



// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();


//app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.Run();

public partial class Program
{

}
