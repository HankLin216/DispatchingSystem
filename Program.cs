using DispatchingSystem.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using DispatchingSystem.DB;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(kestrelOptions =>
{
    // Setup a HTTP/2 endpoint without TLS.
    kestrelOptions.ListenAnyIP(5000, o =>
    {
        o.Protocols = HttpProtocols.Http2;
    });

    kestrelOptions.ListenAnyIP(5001, o =>
    {
        o.Protocols = HttpProtocols.Http2;

        // get the crt
        string rootPath = builder.Environment.ContentRootPath;
        string pfxPath = Path.Combine(rootPath, "certification", "server.pfx");
        o.UseHttps(pfxPath, "0409");
    });
});

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddSingleton<RedisService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<SampleService>();
// app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
