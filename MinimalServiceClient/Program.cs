using System.ServiceModel;

if (args.Length == 0)
{
    Console.WriteLine("Provide the endpoint address to talk to.");
    return;
}

var uri = args[0];
var binding = new BasicHttpBinding();
var endpointAddress = new EndpointAddress(uri);
var client = new RandomNumberClient(binding, endpointAddress);

Console.WriteLine("Here are your random numbers:");

for (int i = 0; i < 10; i++)
{
    var randomNumber = await client.NextAsync();
    Console.WriteLine(randomNumber);
}
