using UnityEngine;

public class PackageVisuals : MonoBehaviour
{
    public GameObject package1;
    public GameObject package2;
    public GameObject package3;
    public GameObject package4;

    private int packageCount = 0;

    private OrderManager _orderManager;

    void Start()
    {
        _orderManager = OrderManager.Instance;
        packageCount = 0;

        UpdatePackageVisuals();

        EventBus.Instance.Register<OrderPickupEvent>(OnOrderPickupEvent);
        EventBus.Instance.Register<OrderCompleteEvent>(OnOrderCompleteEvent);

        package1 = this.transform.Find("Package1").gameObject;
        package2 = this.transform.Find("Package2").gameObject;
        package3 = this.transform.Find("Package3").gameObject;
        package4 = this.transform.Find("Package4").gameObject;
    }

    void OnDestroy()
    {
        EventBus.Instance.Unregister<OrderPickupEvent>(OnOrderPickupEvent);
        EventBus.Instance.Unregister<OrderCompleteEvent>(OnOrderCompleteEvent);
    }

    void OnOrderPickupEvent(OrderPickupEvent evnt)
    {
        packageCount++;
        UpdatePackageVisuals();
    }

    void OnOrderCompleteEvent(OrderCompleteEvent evnt)
    {
        packageCount--;
        UpdatePackageVisuals();
    }

    private void UpdatePackageVisuals()
    {
        package1.SetActive(packageCount >= 1);
        package2.SetActive(packageCount >= 2);
        package3.SetActive(packageCount >= 3);
        package4.SetActive(packageCount >= 4);
    }

}
