

public class OrderCompleteEvent : IEvent
{
    public Order order;

    public OrderCompleteEvent(Order order)
    {
        this.order = order;
    }
}