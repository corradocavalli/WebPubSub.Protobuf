using Azure.Messaging.WebPubSub.Clients;
using Azure.WebPubSub.Protobuf.Protocols;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.Buffers;

namespace Azure.Messaging.WebPubSub.Client.Protobuf
{
    public class WebPubSubProtobufProtocol : WebPubSubProtocol
    {
        private readonly WebPubSubProtobufProtocolBase _processor = new WebPubSubProtobufProtocolBase();

        public override bool IsReliable => false;
        public override string Name => "protobuf.webpubsub.azure.v1";

        public override WebPubSubProtocolMessageType WebSocketMessageType => WebPubSubProtocolMessageType.Binary;

        public override ReadOnlyMemory<byte> GetMessageBytes(WebPubSubMessage message)
        {
            return _processor.GetMessageBytes(message);
        }

        public override WebPubSubMessage ParseMessage(ReadOnlySequence<byte> input)
        {
            return _processor.ParseMessage(input);
        }

        public override void WriteMessage(WebPubSubMessage message, IBufferWriter<byte> output)
        {
            throw new NotImplementedException();
        }
    }
}