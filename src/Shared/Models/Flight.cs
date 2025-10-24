public class Flight
{
    [JsonPropertyName("@attributes")]
    public FlightAttributes Attributes { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
}
