using Grpc.Core;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using DispatchingSystem.DB;
using System.Net;
using System.Text.Json;
using StackExchange.Redis;

namespace DispatchingSystem.Services;

public class SampleService : SampleController.SampleControllerBase
{
    private RedisService _redisService;
    private readonly ILogger<GreeterService> _logger;
    public SampleService(ILogger<GreeterService> logger, RedisService redisService)
    {
        _logger = logger;
        _redisService = redisService;
    }
    public override async Task<Empty> RefreshSampleInformation(RefreshSampleInformationRequest request, ServerCallContext context)
    {
        // ipe as key
        var connInfo = context.GetHttpContext().Connection;
        IPAddress clientIP = connInfo.RemoteIpAddress ?? IPAddress.Any;
        var port = connInfo.RemotePort;
        IPEndPoint ipe = new IPEndPoint(clientIP.MapToIPv4(), port);
        string ip = $"{ipe.Address}:{ipe.Port}";

        var jFormatter = new JsonFormatter(new JsonFormatter.Settings(true));
        foreach (var sampleInfo in request.SampleInfos)
        {
            var jsonStr = jFormatter.Format(sampleInfo);
            //
            var document = JsonDocument.Parse(jsonStr);
            var root = document.RootElement;
            List<HashEntry> heList = new List<HashEntry>();
            string redisHashKey = string.Empty;
            foreach (var jele in root.EnumerateObject())
            {
                if (jele.Name.ToUpper() == "ID")
                {
                    redisHashKey = $"{jele.Value.GetRawText()}@{ip}";
                }

                var he = new HashEntry(jele.Name, jele.Value.GetRawText().Replace("\"", ""));
                heList.Add(he);
            }
            // add update time
            heList.Add(new HashEntry("UpdateTime", DateTime.Now.ToString("yyyy/MM/dd_HH:mm:ss")));

            await _redisService.HashSetAsync(redisHashKey, heList.ToArray());
        }

        return new Empty();
    }

    public override string? ToString()
    {
        return base.ToString();
    }
}
