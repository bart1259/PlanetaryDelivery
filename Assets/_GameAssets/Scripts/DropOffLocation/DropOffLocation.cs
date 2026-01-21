using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropOffLocation : MonoBehaviour
{
    public OrderLocationSO locationSO;

    public static List<DropOffLocation> allDropOffLocations = new List<DropOffLocation>();
    public static DropOffLocation GetDropOffLocationByName(string locationName)
    {
        if (allDropOffLocations.Count == 0){
            allDropOffLocations = new List<DropOffLocation>(FindObjectsOfType<DropOffLocation>());
        }

        foreach (DropOffLocation dropOffLocation in allDropOffLocations)
        {
            if (dropOffLocation.locationSO.locationName == locationName)
            {
                return dropOffLocation;
            }
        }
        return null;
    }

    private bool _isTriggerEnabled = true;
    private OrderManager _orderManager;

    void Start()
    {
        MapManager.Instance.AddMarker(this.transform.position.normalized, locationSO.locationName);
        _orderManager = OrderManager.Instance;
    }

    public void EnableTrigger()
    {
        _isTriggerEnabled = true;
        GetComponent<MeshRenderer>().enabled = true;
    }

    public void DisableTrigger()
    {
        _isTriggerEnabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }

    // On trigger
    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && _isTriggerEnabled)
        {
            EventBus.Instance.Publish(new PlayerEnteredDropOffLocationEvent(this));
        }
    }

    void Update()
    {
        if (_isTriggerEnabled && !_orderManager.LocationIsInActiveOrder(locationSO)) {
            DisableTrigger();
        }

        if (!_isTriggerEnabled && _orderManager.LocationIsInActiveOrder(locationSO)) {
            EnableTrigger();
        }
    }
}
