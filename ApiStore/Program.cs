using ApiStore.Data;
using ApiStore.Data.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Додайте службу DbContext з використанням PostgreSQL
builder.Services.AddDbContext<ApiStoreDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Додайте служби контролерів
builder.Services.AddControllers();

// Додайте Swagger/OpenAPI для документації API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Додайте CORS для дозволу запитів з іншого походження
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Дозволяє запити з порту React-додатка
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Налаштування HTTP запитів
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Додайте CORS у пайплайн
app.UseCors("AllowReactApp");

// Додайте Middleware для авторизації (необов'язково, якщо не використовуєте авторизацію)
app.UseAuthorization();

// Мапінг контролерів
app.MapControllers();

// Міграція бази даних і додавання початкових даних
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApiStoreDbContext>();
    dbContext.Database.Migrate();

    if (dbContext.Categories.Count() == 0)
    {
        var cat = new CategoryEntity
        {
            Name = "Собаки",
            Description = "БоГато Собак",
            Image = "dog.jpg"
        };
        dbContext.Categories.Add(cat);
        dbContext.SaveChanges();
    }
}

app.Run();
