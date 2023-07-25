# Azure Web PubSub protobuf protocol client library for .NET

This library adds the protobuf subprotocols `protobuf.reliable.webpubsub.azure.v1` and `protobuf.webpubsub.azure.v1` to the Azure.Messaging.WebPubSub.Client [library](https://github.com/Azure/azure-sdk-for-net/tree/Azure.Messaging.WebPubSub_1.3.0/sdk/webpubsub/Azure.Messaging.WebPubSub.Client).

### Prerequisites

- An [Azure subscription](https://azure.microsoft.com/free/dotnet/).
- An existing Web PubSub instance. [Create Web PubSub instance](https://learn.microsoft.com/azure/azure-web-pubsub/howto-develop-create-instance)

## Getting started

- Install the Azure.Messaging.WebPubSub.Client package from [NuGet](https://www.nuget.org/): 
```dotnetcli
dotnet add package Azure.Messaging.WebPubSub.Client --prerelease
```
- Install this package from [NuGet](https://www.nuget.org/): 
```dotnetcli
dotnet add package WebPubSub.Protobuf.Protocols
```
You can find more info about the Web PubSub client library [here](https://github.com/Azure/azure-sdk-for-net/tree/Azure.Messaging.WebPubSub_1.3.0/sdk/webpubsub/Azure.Messaging.WebPubSub.Client).  

- Import the `WebPubSub.Client.Protobuf` namespace 
```C#
using WebPubSub.Client.Protobuf
```

## Examples

### Specify protobuf subprotocol

You can specify the protobuf subprotocol to be used by the client choosing between `protobuf.reliable.webpubsub.azure.v1` or `protobuf.webpubsub.azure.v1` after referencing the library.

```C#
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
### Send to groups
```C# Snippet:WebPubSubClient_SendToGroup
// Send message to group "testGroup"
await client.SendToGroupAsync("testGroup", BinaryData.FromString("hello world"), WebPubSubDataType.Text);
```
### Send events to event handler
```C#
// Send custom event to server
await client.SendEventAsync("testEvent", BinaryData.FromString("hello world"), WebPubSubDataType.Text);
```
### Other examples
You can see other examples at the [project repository](https://github.com/corradocavalli/WebPubSub.Protobuf).