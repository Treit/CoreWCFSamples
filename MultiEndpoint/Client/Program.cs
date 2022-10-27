using System.ServiceModel;

var uri = args.Length == 0 ? "http://127.0.0.1:5125/RandomNumber.svc" : args[0];

var binding = new BasicHttpBinding();
var endpointAddress = new EndpointAddress(uri);
var client = new RandomNumberClient(binding, endpointAddress);

Console.WriteLine("Here are your random numbers:");

for (int i = 0; i < 10; i++)
{
    var response = await client.NextAsync(100);
    Console.WriteLine(response.Result);
}
