
using udi.Device_Controller_SignalR.Hubs;
using udi.Device_Controller_SignalR.Interface.DBConn;
using udi.Device_Controller_SignalR.Interface.DeviceController;
using udi.Device_Controller_SignalR.Service.DBConn;
using udi.Device_Controller_SignalR.Service.DeviceControoler;

namespace udi.Device_Controller_SignalR
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IDBConn, DBConn>();
            builder.Services.AddScoped<IGetDeviceControlService, GetDeviceContrlService>();
            builder.Services.AddScoped<IGetUdiBreathing, GetUdiBreathing>();
            builder.Services.AddScoped<IGetRecycleProgress, GetRecyleProgressService>();

            // Cors 設定
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            builder.Services.AddSignalR();
            var app = builder.Build();
            app.UseCors();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();
            // 設定路由
            app.MapHub<SignalServer>("/SignalServer");
            app.Run();
        }
    }
}
