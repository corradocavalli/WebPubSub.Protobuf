using Azure.Messaging.WebPubSub.Clients;
using Azure.WebPubSub.Client;
using Google.Protobuf;
using PubSubCore;
using System.Text;
using WebPubSub.Client.Protobuf;

namespace Azure.WebPubSub.Protobuf
{
    internal class Program
    {
        //Set to true to use the reliable protocol
        private const bool Reliable = false;

        private const string Group = "protobuf-client-group";
        private const string Uri = "<web-pubsub-client-uri>";
        private static string? SenderId = "protobuf-client";

        private enum MessageType
        {
            Text,
            Json,
            Binary,
            Protobuf
        }

        // Select the content type of message to send: Json, Text, Binary or Protobuf.
        private static MessageType _messageType = MessageType.Text;

        private static async Task Main(string[] args)
        {
            //Select the protocol
            WebPubSubProtocol protocol = Reliable ? new WebPubSubProtobufReliableProtocol() : new WebPubSubProtobufProtocol();

            //Create the client
            var serviceClient = new WebPubSubClient(new Uri(Uri), new WebPubSubClientOptions() { Protocol = protocol });

            //Subscribe to events
            serviceClient.Connected += OnConnected;
            serviceClient.Disconnected += OnDisconnected;
            serviceClient.GroupMessageReceived += OnGroupMessageReceived;
            serviceClient.ServerMessageReceived += OnServerMessageReceived;

            //Connects and join the group
            await serviceClient.StartAsync();
            await serviceClient.JoinGroupAsync(Group);

            //Send messages
            while (true)
            {
                Prompt("Enter (Message+Enter) for message or just Enter to stop");
                var message = Console.ReadLine();
                if (string.IsNullOrEmpty(message)) break;

                WebPubSubResult result;
                switch (_messageType)
                {
                    case MessageType.Text:
                        result = await serviceClient.SendToGroupAsync(Group, BinaryData.FromString(message), WebPubSubDataType.Text);
                        break;

                    case MessageType.Json:
                        var jsonMessage = new Message() { Text = message, SenderId = SenderId };
                        result = await serviceClient.SendToGroupAsync(Group, BinaryData.FromObjectAsJson(jsonMessage), WebPubSubDataType.Json);
                        break;

                    case MessageType.Binary:
                        var binaryMessage = Encoding.UTF8.GetBytes(message);
                        result = await serviceClient.SendToGroupAsync(Group, BinaryData.FromBytes(binaryMessage), WebPubSubDataType.Binary);
                        break;

                    case MessageType.Protobuf:
                        var protobufMessage = new ProtobufMessage()
                        {
                            From = SenderId,
                            TextMessage = new TextMessage() { Text = message }
                        };
                        result = await serviceClient.SendToGroupAsync(Group, BinaryData.FromBytes(protobufMessage.ToByteArray()), WebPubSubDataType.Protobuf);
                        break;

                    default:
                        throw new NotSupportedException();
                }
                Prompt($"Message sent with Id={result.AckId} sent");
            }

            //Leave the group and disconnect
            await serviceClient.LeaveGroupAsync(Group);
            await serviceClient.StopAsync();

            serviceClient.Connected -= OnConnected;
            serviceClient.Disconnected -= OnDisconnected;
            serviceClient.GroupMessageReceived -= OnGroupMessageReceived;
            serviceClient.ServerMessageReceived -= OnServerMessageReceived;

            Prompt("You've been disconnected, press Enter to exit.");
            Console.ReadLine();
        }

        private static Task OnServerMessageReceived(WebPubSubServerMessageEventArgs args)
        {
            Console.WriteLine($"Server Message received: {args.Message.Data}");
            return Task.CompletedTask;
        }

        private static Task OnDisconnected(WebPubSubDisconnectedEventArgs arg)
        {
            SystemMessage($"Client id {arg.ConnectionId} disconnected.");
            return Task.CompletedTask;
        }

        private static Task OnConnected(WebPubSubConnectedEventArgs arg)
        {
            Console.WriteLine($"Connected with connection id: {arg.ConnectionId}");
            return Task.CompletedTask;
        }

        private static Task OnGroupMessageReceived(WebPubSubGroupMessageEventArgs arg)
        {
            switch (_messageType)
            {
                case MessageType.Text:
                    Message($"-> Received text message: {arg.Message.Data}");
                    return Task.CompletedTask;

                case MessageType.Json:
                    Message($"-> Received json message: {arg.Message.Data}");
                    return Task.CompletedTask;

                case MessageType.Binary:
                    Message($"-> Received binary message: {Encoding.UTF8.GetString(arg.Message.Data)}");
                    return Task.CompletedTask;

                case MessageType.Protobuf:
                    var protoMessage = ProtobufMessage.Parser.ParseFrom(arg.Message.Data.ToArray());
                    Message($"-> Received protobuf message from {protoMessage.From}: {protoMessage.TextMessage.Text}");
                    return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        private static void Message(string text)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private static void SystemMessage(string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private static void Prompt(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}