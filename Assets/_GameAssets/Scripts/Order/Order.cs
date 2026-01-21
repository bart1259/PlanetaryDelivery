
public class Order
{
    public float timeLimit;
    public float timeRemaining;
    public int difficulty;
    public OrderLocationSO pickupLocation;
    public OrderLocationSO dropoffLocation;
    public bool pickedUp = false;
    public bool completed = false;
    public float timeToComplete;

    public void Update(float deltaTime)
    {
        if (completed) return;
        timeRemaining -= deltaTime;

        if (timeRemaining <= 0) {
            timeRemaining = 0;
            EventBus.Instance.Publish(new OrderExpireEvent(this));
        }
    }

    public void PickUp()
    {
        if (pickedUp) return;
        pickedUp = true;
        EventBus.Instance.Publish(new OrderPickupEvent(this));
    }

    public void Complete()
    {
        if (completed) return;
        timeToComplete = timeLimit - timeRemaining;
        completed = true;
        EventBus.Instance.Publish(new OrderCompleteEvent(this));
    }
}