import sys
from suds.client import Client

uri = "http://127.0.0.1:5125/RandomNumber.svc?singleWsdl"
if (len(sys.argv) > 1):
    uri = sys.argv[1]

client = Client(uri)

headers = {
    'Max': 100
 }

client.set_options(soapheaders=headers)

print("Here are your random numbers:")
for i in range(10):
    print(client.service.Next())

