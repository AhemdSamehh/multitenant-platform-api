using Infrastructure;
using Application;
using StackExchange.Redis;
namespace WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            //builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<IConnectionMultiplexer>(Options =>
            {
                var Conection = builder.Configuration.GetConnectionString("RedisConnection");
                return ConnectionMultiplexer.Connect(Conection);
            });

            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddJwtAuthentication(builder.Services.GetJwtSettings(builder.Configuration));

       

            builder.Services.AddApplicationService();
            var app = builder.Build();


            // Upgrade UseSwagger & UseSwaggerUI
            await app.Services.AddDatabaseInitializerAsync();
            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.UseInfrastructure();

            app.MapControllers();

            app.Run();
        }
    }
}
