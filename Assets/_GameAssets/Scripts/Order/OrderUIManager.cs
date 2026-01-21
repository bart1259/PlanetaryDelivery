using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrderUIManager : MonoBehaviour
{
    public GameObject OrderUICardPrefab;

    public int positionX = -120;
    public int positionY = -30;
    public int spacingY = -65;

    private List<GameObject> _orderUICards = new List<GameObject>();

    void Start()
    {
        OrderManager.Instance.OnOrderCreated += OnNewOrderCreated;
    }

    void CheckDeletedCards()
    {
        _orderUICards.RemoveAll(card => card == null);
    }

    void Update()
    {
        CheckDeletedCards();
        for (int i = 0; i < _orderUICards.Count; i++)
        {
            _orderUICards[i].GetComponent<OrderUICard>().SetDesiredPosition(new Vector2(positionX, positionY + (i * spacingY)));
        }
    }

    void OnNewOrderCreated(Order order)
    {
        Debug.Log("Created new order");
        GameObject orderUICardGO = Instantiate(OrderUICardPrefab, this.transform);
        Vector2 desiredPosition = new Vector2(positionX, positionY + (_orderUICards.Count * spacingY));
        orderUICardGO.GetComponent<RectTransform>().anchoredPosition = desiredPosition;
        
        OrderUICard orderUICard = orderUICardGO.GetComponent<OrderUICard>();
        orderUICard.Hydrate(order);
        orderUICard.SetDesiredPosition(desiredPosition);

        _orderUICards.Add(orderUICardGO);
    }
}