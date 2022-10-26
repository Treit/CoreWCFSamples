# CoreWCFSamples
Sample CoreWCF projects.

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
