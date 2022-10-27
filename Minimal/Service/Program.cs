using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddServiceModelServices()
    .AddServiceModelMetadata()
    .AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder
        .AddService<RandomNumberService>()
        .AddServiceEndpoint<RandomNumberService, IRandomNumber>(new BasicHttpBinding(), "/RandomNumber.svc");

    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;
});

app.MapGet("/", () => "Hello! Please post a SOAP message to me!");

app.Run();

[ServiceContract]
public interface IRandomNumber
{
    [OperationContract]
    int Next();
}

public class RandomNumberService : IRandomNumber
{
    readonly Random _random = new();

    public int Next()
    {
        return _random.Next();
    }
}
