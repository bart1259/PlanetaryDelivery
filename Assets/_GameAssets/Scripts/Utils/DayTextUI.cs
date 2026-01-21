using UnityEngine;
using TMPro;

public class DayTextUI : MonoBehaviour
{
    private TMP_Text dayText;

    void Start()
    {
        dayText = GetComponent<TMP_Text>();
        int currentDay = GlobalStateManager.Instance.GetCurrentDay();
        dayText.text = "Day " + currentDay;
    }
}
