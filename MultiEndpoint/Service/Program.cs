using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using System.Runtime.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddServiceModelServices()
    .AddServiceModelMetadata()
    .AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    var binding = new BasicHttpBinding();

    serviceBuilder
        .AddService<RandomNumberService>()
        .AddServiceEndpoint<RandomNumberService, IRandomNumber>(binding, "/RandomNumber.svc")
        .AddServiceEndpoint<RandomNumberService, IRandomNumber>(binding, "/Alternate/RandomNumber.svc");

    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;
});

app.MapGet("/", () => "Hello! Please post a SOAP message to me!");

app.Run();

[MessageContract]
public class RandomNumberRequest
{
    [MessageHeader]
    public int Max { get; set; }
}

[MessageContract]
public class RandomNumberResponse
{
    [MessageBodyMember]
    public int Result { get; set; }
}

[ServiceContract]
public interface IRandomNumber
{
    [OperationContract]
    RandomNumberResponse Next(RandomNumberRequest request);
}

public class RandomNumberService : IRandomNumber
{
    readonly Random _random = new();

    public RandomNumberResponse Next(RandomNumberRequest request)
    {
        var num = _random.Next(request.Max);
        return new RandomNumberResponse { Result = num };
    }
}
