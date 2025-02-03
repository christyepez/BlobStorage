using BlobStorage.WebAPI.Contexts;
using BlobStorage.WebAPI.Repositories;
using BlobStorage.WebAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BlobStorageContext>(x => x.UseSqlServer(connectionString));
builder.Services.AddScoped<IBlobsRepository, CarsRepository>();
builder.Services.AddScoped<IAzureStorageService, AzureStorageService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddCors();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors();
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(name: MyAllowSpecificOrigins,
//                      policy =>
//                      {
//                          policy
//                            .WithOrigins(builder.Configuration["Host"])
//                             .AllowAnyHeader()
//                            .AllowAnyMethod(); ;

//                      });


//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(options =>
{
    options.AllowAnyOrigin();
    options.AllowAnyMethod();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
