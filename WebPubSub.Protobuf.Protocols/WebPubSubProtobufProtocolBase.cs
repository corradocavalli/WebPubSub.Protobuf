using Messaging.WebPubSub.Client.Protobuf;
using Azure.Messaging.WebPubSub.Clients;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.Buffers;

namespace Azure.WebPubSub.Protobuf.Protocols
{
    internal class WebPubSubProtobufProtocolBase
    {
        public ReadOnlyMemory<byte> GetMessageBytes(WebPubSubMessage message)
        {
            var upstreamMessage = new UpstreamMessage();

            switch (message)
            {
                case JoinGroupMessage joinGroupMessage:
                    upstreamMessage.JoinGroupMessage = new UpstreamMessage.Types.JoinGroupMessage()
                    {
                        Group = joinGroupMessage.Group,
                        AckId = joinGroupMessage.AckId.HasValue ? (ulong)joinGroupMessage.AckId : 0
                    };
                    break;

                case LeaveGroupMessage leaveGroupMessage:
                    upstreamMessage.LeaveGroupMessage = new UpstreamMessage.Types.LeaveGroupMessage()
                    {
                        Group = leaveGroupMessage.Group,
                        AckId = leaveGroupMessage.AckId.HasValue ? (ulong)leaveGroupMessage.AckId : 0
                    };
                    break;

                case SendToGroupMessage sendToGroupMessage:
                    upstreamMessage.SendToGroupMessage = new UpstreamMessage.Types.SendToGroupMessage()
                    {
                        Group = sendToGroupMessage.Group,
                        AckId = sendToGroupMessage.AckId.HasValue ? (ulong)sendToGroupMessage.AckId : 0,
                        Data = new MessageData()
                    };

                    switch (sendToGroupMessage.DataType)
                    {
                        case WebPubSubDataType.Text:                        
                            upstreamMessage.SendToGroupMessage.Data.TextData = sendToGroupMessage.Data.ToString();
                            break;
                        case WebPubSubDataType.Json:
                            upstreamMessage.SendToGroupMessage.Data.JsonData = sendToGroupMessage.Data.ToString();
                            break;
                        case WebPubSubDataType.Binary:
                            upstreamMessage.SendToGroupMessage.Data.BinaryData = ByteString.FromStream(sendToGroupMessage.Data.ToStream());
                            break;

                        case WebPubSubDataType.Protobuf:
                            upstreamMessage.SendToGroupMessage.Data.ProtobufData = Any.Parser.ParseFrom(sendToGroupMessage.Data.ToStream());
                            break;

                        default:
                            throw new InvalidDataException($"Unknown data type: {sendToGroupMessage.DataType}");
                    }
                    break;

                case SendEventMessage sendEventMessage:
                    upstreamMessage.EventMessage = new UpstreamMessage.Types.EventMessage()
                    {
                        Event = sendEventMessage.EventName,
                        AckId = sendEventMessage.AckId.HasValue ? (ulong)sendEventMessage.AckId : 0,
                        Data = new MessageData(),
                    };
                    switch (sendEventMessage.DataType)
                    {
                        case WebPubSubDataType.Text:
                            upstreamMessage.EventMessage.Data.TextData = sendEventMessage.Data.ToString();
                            break;
                        case WebPubSubDataType.Json:
                            upstreamMessage.EventMessage.Data.JsonData = sendEventMessage.Data.ToString();
                            break;

                        case WebPubSubDataType.Binary:
                            upstreamMessage.EventMessage.Data.BinaryData = ByteString.FromStream(sendEventMessage.Data.ToStream());
                            break;

                        case WebPubSubDataType.Protobuf:
                            upstreamMessage.EventMessage.Data.ProtobufData = Any.Parser.ParseFrom(sendEventMessage.Data.ToStream());
                            break;

                        default:
                            throw new InvalidDataException($"Unknown data type: {sendEventMessage.DataType}");
                    }
                    break;

                case SequenceAckMessage sequenceAckMessage:
                    upstreamMessage.SequenceAckMessage = new UpstreamMessage.Types.SequenceAckMessage()
                    {
                        SequenceId = sequenceAckMessage.SequenceId
                    };
                    break;

                default:
                    throw new InvalidDataException($"Unknown message type: {message.GetType().Name}");
            };

            return new ReadOnlyMemory<byte>(upstreamMessage.ToByteArray());
        }

        public WebPubSubMessage ParseMessage(ReadOnlySequence<byte> input)
        {
            var downstreamMessage = DownstreamMessage.Parser.ParseFrom(input.ToArray());
            switch (downstreamMessage.MessageCase)
            {
                case DownstreamMessage.MessageOneofCase.SystemMessage:
                    DownstreamMessage.Types.SystemMessage systemMessage = downstreamMessage.SystemMessage;
                    switch (systemMessage.MessageCase)
                    {
                        case DownstreamMessage.Types.SystemMessage.MessageOneofCase.ConnectedMessage:
                            var connectedMessage = systemMessage.ConnectedMessage;
                            return new ConnectedMessage(connectedMessage.UserId, connectedMessage.ConnectionId, connectedMessage.ReconnectionToken);

                        case DownstreamMessage.Types.SystemMessage.MessageOneofCase.DisconnectedMessage:
                            var disconnectedMessage = systemMessage.DisconnectedMessage;
                            return new DisconnectedMessage(disconnectedMessage.Reason);

                        default:
                            throw new InvalidDataException($"Unsupported system message case: {systemMessage.MessageCase}");
                    }
                case DownstreamMessage.MessageOneofCase.AckMessage:
                    var ackMessage = downstreamMessage.AckMessage;
                    var error = ackMessage.Error != null ? new AckMessageError(ackMessage.Error?.Name, ackMessage.Error?.Message) : null;
                    return new AckMessage(ackMessage.AckId, ackMessage.Success, error);

                case DownstreamMessage.MessageOneofCase.DataMessage:
                    byte[] binaryData = Array.Empty<byte>();
                    string textData = string.Empty;
                    WebPubSubDataType dataType;
                    var dataMessage = downstreamMessage.DataMessage;
                    switch (dataMessage.Data.DataCase)
                    {
                        case MessageData.DataOneofCase.ProtobufData:
                            binaryData = dataMessage.Data.ProtobufData.ToByteArray();
                            dataType = WebPubSubDataType.Protobuf;
                            break;

                        case MessageData.DataOneofCase.BinaryData:
                            binaryData = dataMessage.Data.BinaryData.ToByteArray();
                            dataType = WebPubSubDataType.Binary;
                            break;

                        case MessageData.DataOneofCase.TextData:
                            textData = dataMessage.Data.TextData.ToString();
                            dataType = WebPubSubDataType.Text;
                            break;

                        case MessageData.DataOneofCase.JsonData:
                            textData = dataMessage.Data.JsonData.ToString();
                            dataType = WebPubSubDataType.Json;
                            break;

                        default:
                            throw new InvalidDataException($"Unsupported data type: {dataMessage.Data.DataCase}");
                    }

                    var data = (dataType == WebPubSubDataType.Text || dataType == WebPubSubDataType.Json) ? BinaryData.FromString(textData) : BinaryData.FromBytes(binaryData);
                    switch (dataMessage.From)
                    {
                        case "group":
                            return new GroupDataMessage(dataMessage.Group, dataType, data, dataMessage.SequenceId == 0 ? null : dataMessage.SequenceId, null);

                        case "server":
                            return new ServerDataMessage(dataType, data, dataMessage.SequenceId);

                        default:
                            throw new InvalidDataException($"Unsupported message sender: {dataMessage.From}");
                    }
                default:
                    throw new InvalidDataException($"Unsupported Downstream Messagecase: {downstreamMessage.MessageCase.GetType().Name}");
            }
        }
    }
}