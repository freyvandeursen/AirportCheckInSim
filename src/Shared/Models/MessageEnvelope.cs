public class MessageEnvelope
{
    public string MessageType { get; set; }
    public string CorrelationId { get; set; }
    public JsonElement Payload { get; set; }
}

