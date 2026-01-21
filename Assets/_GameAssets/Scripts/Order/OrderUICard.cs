using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class OrderUICard : MonoBehaviour
{
    public TMP_Text pickupLocationText;
    public TMP_Text dropOffLocationText;
    public TMP_Text timeLeftText;

    private Order _order;
    private Animator _animationController;
    private Vector2 _desiredPosition;

    void Start(){
        _animationController = GetComponent<Animator>();
    }

    public void Hydrate(Order order)
    {
        _order = order;

        pickupLocationText.text = order.pickupLocation.locationName;
        dropOffLocationText.text = order.dropoffLocation.locationName;

        // Some don't have time limits
        if (order.timeRemaining != 0.0f) {
            timeLeftText.text = FormatTime(order.timeRemaining);
        }
    }

    public void SetDesiredPosition(Vector2 position)
    {
        _desiredPosition = position;
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    public void Update()
    {
        timeLeftText.text = FormatTime(_order.timeRemaining);

        if (_order.pickedUp && pickupLocationText.fontStyle != FontStyles.Strikethrough) {
            pickupLocationText.fontStyle = FontStyles.Strikethrough;
        }

        if (_order.completed && dropOffLocationText.fontStyle != FontStyles.Strikethrough) {
            dropOffLocationText.fontStyle = FontStyles.Strikethrough;
        }

        if (_order.completed) {
            _animationController.SetBool("SlideOut", true);
        }

        // Animate going towards _desiredPosition
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, _desiredPosition, Time.deltaTime * 10.0f);
    }

    public void OnSlideOutComplete()
    {
        if (_order.completed) {
            Destroy(this.gameObject);
        }
    }
}