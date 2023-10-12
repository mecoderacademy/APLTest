using APLTest.Data;
using APLTest.Services;
using APLTest.Services.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
SqliteConnection sqliteConnection;
sqliteConnection = new SqliteConnection("Filename=:memory:");
sqliteConnection.Open();

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped(typeof(IFileService), typeof(AzureFileService));
builder.Services.AddScoped(typeof(IFileService), typeof(LocalFileService));
builder.Services.AddScoped<DbContext,FileStorageContext>();
builder.Services.AddDbContext<FileStorageContext>(options => options.UseSqlite(sqliteConnection));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .AllowAnyOrigin() // Allow requests from any origin
            .AllowAnyHeader()
            .AllowAnyMethod();
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
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
