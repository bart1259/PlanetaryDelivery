using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

public class OrderManager : MonoBehaviour
{
    // Singleton instance
    private static OrderManager _instance;
    private List<OrderLocationSO> _orderLocations;
    private Dictionary<Tuple<OrderLocationSO,OrderLocationSO>, int> _locationDifficulties;

    private List<Order> _orders;
    private List<Tuple<Order, bool>> _orderSuccesses = new List<Tuple<Order, bool>>(); // bool indicates if order was completed (true) or expired (false)

    public delegate void OrderCreated(Order order);
    public event OrderCreated OnOrderCreated;

    public static OrderManager Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        Init();

        EventBus.Instance.Register<PlayerEnteredDropOffLocationEvent>(OnPlayerEnteredDropOffLocation);
        EventBus.Instance.Register<OrderCompleteEvent>(OnOrderComplete);
        EventBus.Instance.Register<OrderExpireEvent>(OnOrderExpire);
    }

    void OnDestroy()
    {
        EventBus.Instance.Unregister<PlayerEnteredDropOffLocationEvent>(OnPlayerEnteredDropOffLocation);
        EventBus.Instance.Unregister<OrderCompleteEvent>(OnOrderComplete);
        EventBus.Instance.Unregister<OrderExpireEvent>(OnOrderExpire);
    }

    void OnPlayerEnteredDropOffLocation(PlayerEnteredDropOffLocationEvent evnt)
    {
        for (int i = 0; i < _orders.Count; i++)
        {
            Order order = _orders[i];
            if (order.pickupLocation == evnt.dropOffLocation.locationSO)
            {
                Debug.Log("Player has picked up order from " + order.pickupLocation.locationName);
                order.PickUp();
            }
            if (order.dropoffLocation == evnt.dropOffLocation.locationSO && order.pickedUp)
            {
                Debug.Log("Player has dropped off order at " + order.dropoffLocation.locationName);
                order.Complete();
            }
        }
    }

    void OnOrderComplete(OrderCompleteEvent evnt)
    {
        _orderSuccesses.Add(new Tuple<Order, bool>(evnt.order, true));
        _orders.Remove(evnt.order);
        CheckEndGame();
    }

    void OnOrderExpire(OrderExpireEvent evnt)
    {
        _orderSuccesses.Add(new Tuple<Order, bool>(evnt.order, false));
        _orders.Remove(evnt.order);
        CheckEndGame();
    }

    void CheckEndGame()
    {
        if (_orders.Count == 0)
        {
            // Check if any orders expired
            bool anyExpired = false;
            int orderSuccessCount = 0;
            float maxTimeToComplete = 0.0f;
            foreach (Tuple<Order, bool> orderTuple in _orderSuccesses)
            {
                Order order = orderTuple.Item1;
                maxTimeToComplete = Mathf.Max(maxTimeToComplete, order.timeToComplete);
                if (orderTuple.Item2 == false)
                {
                    anyExpired = true;
                } else
                {
                    orderSuccessCount++;
                }
            }

            FindObjectsByType<CarPhysics>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)[0].enabled = false;
            FindObjectsByType<MapManager>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)[0].enabled = false;

            bool win = !anyExpired;
            if (win)
            {
                GlobalStateManager.Instance.DayCompleted();
                GameOverUI.Instance.Win(GlobalStateManager.Instance.GetCurrentDay(), orderSuccessCount, _orderSuccesses.Count, maxTimeToComplete);
            } else
            {
                GameOverUI.Instance.Lose(GlobalStateManager.Instance.GetCurrentDay(), orderSuccessCount, _orderSuccesses.Count);
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < _orders.Count; i++)
        {
            _orders[i].Update(Time.deltaTime);
        }
    }

    public void Init()
    {
        _orders = new List<Order>();
        _orderLocations = new List<OrderLocationSO>(Resources.LoadAll<OrderLocationSO>(""));
        LoadLocationLocationDifficulties();
    }

    public Order MakeNewOrder(int difficulty, float timeLimit)
    {
        // Filter location pairs by difficulty
        var possibleLocationPairs = _locationDifficulties.Where(kv => kv.Value == difficulty).Select(kv => kv.Key).ToList();
        if (possibleLocationPairs.Count == 0)
        {
            Debug.LogError("No location pairs found for difficulty " + difficulty);
            return null;
        }

        // Select a random location pair
        var randomIndex = UnityEngine.Random.Range(0, possibleLocationPairs.Count);
        var selectedPair = possibleLocationPairs[randomIndex];

        // Ensure the drop off location is not already used in an active order
        foreach (var existingOrder in _orders)
        {
            if (existingOrder.dropoffLocation.locationName == selectedPair.Item2.locationName)
            {
                return MakeNewOrder(difficulty, timeLimit); // Recursively try again
            }
        }

        // Ensure the pick up location is not already used in an active order
        foreach (var existingOrder in _orders)
        {
            if (existingOrder.pickupLocation.locationName == selectedPair.Item1.locationName)
            {
                return MakeNewOrder(difficulty, timeLimit); // Recursively try again
            }
        }

        // Ensure the pick up and drop off location combo is not already used in an active order
        foreach (var existingOrder in _orders)
        {
            if (existingOrder.pickupLocation.locationName == selectedPair.Item2.locationName && existingOrder.dropoffLocation.locationName == selectedPair.Item1.locationName)
            {
                return MakeNewOrder(difficulty, timeLimit); // Recursively try again
            }
        }

        Order order = new Order();
        order.timeLimit = timeLimit; // FIXME: Make some kind of data structure to handle this gets calculated
        order.timeRemaining = timeLimit;
        order.difficulty = difficulty;
        order.pickupLocation = selectedPair.Item1;
        order.dropoffLocation = selectedPair.Item2;

        _orders.Add(order);

        Debug.Log("Calling event");
        OnOrderCreated?.Invoke(order);

        return order;
    }

    private OrderLocationSO FindLocationByName(string locationName)
    {
        locationName = locationName.Trim();
        foreach (var location in _orderLocations)
        {
            if (location.locationName == locationName)
            {
                return location;
            }
        }
        return null;
    }

    private void LoadLocationLocationDifficulties()
    {
        TextAsset locationDifficultyText = Resources.Load<TextAsset>("LocationDifficulties");

        if (locationDifficultyText != null)
        {
            string csvText = locationDifficultyText.text;
            string[] lines = csvText.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            string[] header = lines[0].Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

            // Assert that header[0] is "x"
            if (header[0].Trim() != "x")
            {
                Debug.LogError("LocationDifficulties.csv first cell must be 'x'");
                return;
            }

            // Ensure all locations exist
            List<OrderLocationSO> locations = new List<OrderLocationSO>();
            for (int i = 1; i < header.Length; i++)
            {
                string locationName = header[i].Trim();
                OrderLocationSO location = FindLocationByName(locationName);
                if (location == null)
                {
                    Debug.LogError("LocationDifficulties.csv references unknown location: " + locationName);
                    return;
                }
                locations.Add(location);
            }

            // Interate over each row
            _locationDifficulties = new Dictionary<Tuple<OrderLocationSO, OrderLocationSO>, int>();
            for (int i = 1; i < lines.Length; i++)
            {
                string[] cells = lines[i].Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                string rowLocationName = cells[0].Trim();
                OrderLocationSO rowLocation = FindLocationByName(rowLocationName);
                if (rowLocation == null)
                {
                    Debug.LogError("LocationDifficulties.csv references unknown location: " + rowLocationName);
                    return;
                }

                for (int j = 1; j < cells.Length; j++)
                {
                    string cellValue = cells[j].Trim();
                    int difficulty;
                    if (int.TryParse(cellValue, out difficulty))
                    {
                        // Debug.Log("Parsed difficulty " + difficulty + " for locations " + rowLocation.locationName + " to " + locations[j - 1].locationName);
                        OrderLocationSO colLocation = locations[j - 1];
                        Tuple<OrderLocationSO, OrderLocationSO> locationPair = new Tuple<OrderLocationSO, OrderLocationSO>(rowLocation, colLocation);
                        _locationDifficulties[locationPair] = difficulty;
                    }
                    else
                    {
                        Debug.LogError("LocationDifficulties.csv has invalid integer value at row " + i + ", column " + j);
                        return;
                    }
                }
            }
        } else {
            Debug.LogError("Could not find LocationDifficulties.csv in Resources.");
        }
    }

    public bool LocationIsInActiveOrder(OrderLocationSO location)
    {
        foreach (var order in _orders)
        {
            if ((order.pickupLocation == location && !order.pickedUp) || (order.pickedUp && !order.completed && order.dropoffLocation == location))
            {
                return true;
            }
        }
        return false;
    }
}