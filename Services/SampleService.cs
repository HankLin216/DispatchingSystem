using Grpc.Core;
using Google.Protobuf.WellKnownTypes;

namespace DispatchingSystem.Services;

public class SampleService : SampleController.SampleControllerBase
{
    private readonly ILogger<GreeterService> _logger;
    public SampleService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override Task<Empty> RefreshSampleInformation(RefreshSampleInformationRequest request, ServerCallContext context)
    {
        string? clientIP = context.GetHttpContext().Connection.RemoteIpAddress?.ToString();
        
        return base.RefreshSampleInformation(request, context);
    }

    public override string? ToString()
    {
        return base.ToString();
    }
}
