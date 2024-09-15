using ApiStore.Data;
using ApiStore.Data.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ������� ������ DbContext � ������������� PostgreSQL
builder.Services.AddDbContext<ApiStoreDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ������� ������ ����������
builder.Services.AddControllers();

// ������� Swagger/OpenAPI ��� ������������ API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ������� CORS ��� ������� ������ � ������ ����������
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // �������� ������ � ����� React-�������
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ������������ HTTP ������
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ������� CORS � ��������
app.UseCors("AllowReactApp");

// ������� Middleware ��� ����������� (������'������, ���� �� ������������� �����������)
app.UseAuthorization();

// ����� ����������
app.MapControllers();

// ̳������ ���� ����� � ��������� ���������� �����
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApiStoreDbContext>();
    dbContext.Database.Migrate();

    if (dbContext.Categories.Count() == 0)
    {
        var cat = new CategoryEntity
        {
            Name = "������",
            Description = "������ �����",
            Image = "dog.jpg"
        };
        dbContext.Categories.Add(cat);
        dbContext.SaveChanges();
    }
}

app.Run();
