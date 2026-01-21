

public class OrderExpireEvent : IEvent
{
    public Order order;

    public OrderExpireEvent(Order order)
    {
        this.order = order;
    }
}