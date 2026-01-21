

public class OrderPickupEvent : IEvent
{
    public Order order;

    public OrderPickupEvent(Order order)
    {
        this.order = order;
    }
}