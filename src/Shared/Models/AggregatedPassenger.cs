
public class AggregatedPassenger
{
    public required Passenger passenger { get; set; }
    public List<Luggage>? luggage { get; set; }

    public static implicit operator AggregatedPassenger(AggregatedPassenger v)
    {
        throw new NotImplementedException();
    }
}

