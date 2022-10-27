## MinimalService and MinimalServiceClient
This is a minimal WCF service that supports a simple web method to return a random integer value.

To run the service using localhost:

```ps
pushd ./MinimalService
dotnet run
```

To run the client against the local service:

```ps
pushd ./MinimalServiceClient
dotnet run "http://localhost:5125/RandomNumber.svc"
```

To run cUrl against the local service:
```ps
curl --silent --location --request POST "http://localhost:5125/RandomNumber.svc" --header "SOAPAction: http://tempuri.org/IRandomNumber/Next" --header "Content-Type: text/xml; charset=utf-8" --data-raw "<s:Envelope xmlns:s='http://schemas.xmlsoap.org/soap/envelope/'><s:Body><Next xmlns='http://tempuri.org/'/></s:Body></s:Envelope>" | Format-Xml
```