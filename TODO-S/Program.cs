using TODO_S.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options => 
{
    options.AddPolicy("AllowAnyOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddSingleton<IListItemDatabase, ListItemDatabase>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors("AllowAnyOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();
