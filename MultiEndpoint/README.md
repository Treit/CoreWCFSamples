# CoreWCF service listening on multiple endpoints
The CoreWCF service here is configured to listen on multiple endpoints for the same service:

```cs
    serviceBuilder
        .AddService<RandomNumberService>()
        .AddServiceEndpoint<RandomNumberService, IRandomNumber>(binding, "/RandomNumber.svc")
        .AddServiceEndpoint<RandomNumberService, IRandomNumber>(binding, "/Alternate/RandomNumber.svc");
```

This causes an issue when generating the WSDL for the service. Header values are duplicated:

```xml
  <wsdl:binding name="BasicHttpBinding_IRandomNumber" type="tns:IRandomNumber">
    <soap:binding transport="http://schemas.xmlsoap.org/soap/http" />
    <wsdl:operation name="Next">
      <soap:operation soapAction="http://tempuri.org/IRandomNumber/Next" style="document" />
      <wsdl:input name="RandomNumberRequest">
        <soap:header message="tns:RandomNumberRequest_Headers" part="Max" use="literal" />
        <soap:body use="literal" />
        <soap:header message="tns:RandomNumberRequest_Headers" part="Max" use="literal" />
      </wsdl:input>
      <wsdl:output name="RandomNumberResponse">
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
```

If we listened on three endpoints the Max SOAP header would show up three times, and so on.

## Python SOAP client problem
While the C# client generated using svcutil has no problem with these duplicated values, using the
Python suds package to make the WCF call causes it to send the header value multiple times.

This Python code:

```py
headers = {
    'Max': 100
 }

client.set_options(soapheaders=headers)
```

results in this payload being sent over the wire:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<SOAP-ENV:Envelope xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:tns="http://tempuri.org/" xmlns:ns0="http://tempuri.org/" xmlns:ns1="http://schemas.xmlsoap.org/soap/envelope/">
  <SOAP-ENV:Header>
    <tns:Max>100</tns:Max>
    <tns:Max>100</tns:Max>
  </SOAP-ENV:Header>
  <ns1:Body>
    <ns0:RandomNumberRequest />
  </ns1:Body>
</SOAP-ENV:Envelope>
```

This causes the WCF service to fail if the service implementation contains code to query for this header:

```
System.Reflection.TargetInvocationException
  HResult=0x80131604
  Message=Exception has been thrown by the target of an invocation.
  Source=System.Private.CoreLib
  StackTrace:
   at System.RuntimeMethodHandle.InvokeMethod(Object target, Span`1& arguments, Signature sig, Boolean constructor, Boolean wrapExceptions)
   at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture) in /_/src/coreclr/System.Private.CoreLib/src/System/Reflection/RuntimeMethodInfo.cs:line 435

  This exception was originally thrown at this call stack:
    CoreWCF.Channels.MessageHeaders.FindNonAddressingHeader(string, string, string[]) in MessageHeaders.cs
    CoreWCF.Channels.MessageHeaders.GetHeader<T>(string, string, System.Runtime.Serialization.XmlObjectSerializer) in MessageHeaders.cs
    RandomNumberService.Next(RandomNumberRequest) in Program.cs

Inner Exception 1:
MessageHeaderException: Multiple headers with name 'Max' and namespace 'http://tempuri.org/' found.
```

## To reproduce
In one terminal window:
```ps
pushd ./Service
dotnet run
```

In another terminal window:
```ps
pip install suds
python ./python_test_client.py
```
