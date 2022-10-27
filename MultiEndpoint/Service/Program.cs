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
        // We could just pull the Max soap header value off of request.Max, but attempting to get
        // the header from IncomingMessageHeaders illustrates a problem when called via a Python
        // SOAP client.
        var max = OperationContext.Current.IncomingMessageHeaders.GetHeader<int>("Max", "http://tempuri.org/");
        var num = _random.Next(max);
        return new RandomNumberResponse { Result = num };
    }
}
