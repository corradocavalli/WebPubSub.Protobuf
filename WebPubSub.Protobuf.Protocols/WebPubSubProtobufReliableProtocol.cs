using Azure.Messaging.WebPubSub.Clients;
using System.Buffers;

namespace WebPubSub.Client.Protobuf
{
    public class WebPubSubProtobufReliableProtocol : WebPubSubProtocol
    {
        private readonly WebPubSubProtobufProtocolBase _processor = new WebPubSubProtobufProtocolBase();

        public override string Name => "protobuf.reliable.webpubsub.azure.v1";

        public override WebPubSubProtocolMessageType WebSocketMessageType => WebPubSubProtocolMessageType.Binary;

        public override bool IsReliable => true;

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