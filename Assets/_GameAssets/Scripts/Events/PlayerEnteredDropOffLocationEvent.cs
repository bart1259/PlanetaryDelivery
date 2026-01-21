

public class PlayerEnteredDropOffLocationEvent : IEvent
{
    public DropOffLocation dropOffLocation;

    public PlayerEnteredDropOffLocationEvent(DropOffLocation dropOffLocation)
    {
        this.dropOffLocation = dropOffLocation;
    }
}