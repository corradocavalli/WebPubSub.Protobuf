# Azure Web PubSub protobuf protocol client library for .NET

[Web PubSub](https://aka.ms/awps/doc) is an Azure-managed service that helps developers easily build web applications with real-time features and publish-subscribe patterns. Any scenario that requires real-time publish-subscribe messaging between server and clients or among clients can use Web PubSub. Traditional real-time features that often require polling from the server or submitting HTTP requests can also use Web PubSub.

You can use this library to add protobuf subprotocols including `protobuf.reliable.webpubsub.azure.v1` and `protobuf.webpubsub.azure.v1` support to the Azure.Messaging.WebPubSub.Client library.

## Getting started


Clone this repository, add the Azure.WebPubSub protocol project to your solution and reference it from your project.

### Prerequisites

- An [Azure subscription](https://azure.microsoft.com/free/dotnet/).
- An existing Web PubSub instance. [Create Web PubSub instance](https://learn.microsoft.com/azure/azure-web-pubsub/howto-develop-create-instance)

## Examples

### Specify protobuf subprotocol

You can specify the subprotocol to be used by the client. You can choose to use `protobuf.reliable.webpubsub.azure.v1` or `protobuf.webpubsub.azure.v1` after referencing the library.

```C# Snippet:WebPubSubClient_JsonReliableProtocol
var client = new WebPubSubClient(new Uri("<client-access-uri>"), new WebPubSubClientOptions
{
    Protocol = new WebPubSubProtobufReliableProtocol()
});
```

```C# Snippet:WebPubSubClient_JsonProtocol
var client = new WebPubSubClient(new Uri("<client-access-uri>"), new WebPubSubClientOptions
{
    Protocol = new WebPubSubProtobufProtocol()
});
```

### Included project sample
The repo includes a sample that shows how to use the protobuf protocol with supported data types to send and receive simple messages.
#### How to run the sample
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

