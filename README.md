# Azure Web PubSub protobuf protocol client library for .NET

[Web PubSub](https://aka.ms/awps/doc) is an Azure-managed service that helps developers easily build web applications with real-time features and publish-subscribe patterns. Any scenario that requires real-time publish-subscribe messaging between server and clients or among clients can use Web PubSub. Traditional real-time features that often require polling from the server or submitting HTTP requests can also use Web PubSub.

You can use this library to add protobuf subprotocols including `protobuf.reliable.webpubsub.azure.v1` and `protobuf.webpubsub.azure.v1` support to the Azure.Messaging.WebPubSub.Client library.

## Getting started

### Install the package
Install the client library from [NuGet](https://www.nuget.org/packages/WebPubSub.Protobuf):

```dotnetcli
dotnet add package WebpubSub.Protobuf --version 1.0.0
```


### Prerequisites

- An [Azure subscription](https://azure.microsoft.com/free/dotnet/).
- An existing Web PubSub instance. [Create Web PubSub instance](https://learn.microsoft.com/azure/azure-web-pubsub/howto-develop-create-instance)

## Select the protobuf subprotocol

Import the `WebPubSub.Client.Protobuf` namespace
```C#
using WebPubSub.Client.Protobuf
```


Indicate the subprotocol used by the client choosing from `protobuf.webpubsub.azure.v1` or `protobuf.reliable.webpubsub.azure.v1`.

```C#
var client = new WebPubSubClient(new Uri("<client-access-uri>"), new WebPubSubClientOptions
{
    Protocol = new WebPubSubProtobufReliableProtocol() // or new WebPubSubProtobufProtocol()
});
```

## Example
The repo includes a sample that shows how to use the protobuf protocol with supported data types to send and receive simple messages.
### How to run the sample
1. Set a valid PubSub connection string.

```C#
 private const string Uri = "wss://...";
```
2. Select the type of data to send between Text, Json, Binary and Protobuf.
```C#
 private static MessageType _messageType = MessageType.Text;
```

3. Select between Reliable and Non-Reliable protocol.
```C#
 private const bool Reliable = true;
```
Enter some text in the console and press enter to send the message, it will be received by the client and printed in the console.
