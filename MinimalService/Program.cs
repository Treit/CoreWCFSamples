using CoreWCF;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

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
