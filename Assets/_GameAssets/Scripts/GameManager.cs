using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Responsible for just about everything high level in the game
    public int Day = 1 ; // This is what day (level) the player is on (1 indexed)

    public DayDifficulty[] DayDifficulties;

    void Start()
    {
        Day = GlobalStateManager.Instance.GetCurrentDay();
        Invoke("DelayedStart", 1.0f);
    }

    void DelayedStart()
    {
        StartDay(Day);
    }


    public void StartDay(int day)
    {
        Day = day;

        bool difficultyFound = false;
        DayDifficulty currentDifficulty = DayDifficulties[0];
        for (int i = 0; i < DayDifficulties.Length; i++)
        {
            if (DayDifficulties[i].Day == day)
            {
                currentDifficulty = DayDifficulties[i];
                difficultyFound = true;
                break;
            }
        }

        if (difficultyFound)
        {
            foreach (var orderInfo in currentDifficulty.OrderDifficulties)
            {
                for (int i = 0; i < orderInfo.Count; i++)
                {
                    OrderManager.Instance.MakeNewOrder(orderInfo.OrderDifficulty, currentDifficulty.TimeLimit);
                }
            }
        }
        else {
            Debug.LogError("No difficulty found for day " + day);
            return;
        }


    }
}
