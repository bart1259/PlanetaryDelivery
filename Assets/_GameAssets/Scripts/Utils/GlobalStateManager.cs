using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalStateManager : MonoBehaviour
{
    public static GlobalStateManager Instance;

    [Header("Progress")]
    public int currentDay = 1;        // The day the player is currently playing (1..5)
    public int maxCompletedDay = 0;   // Highest day the player has completed

    const string SaveKey_MaxCompletedDay = "MaxCompletedDay";

    void Awake()
    {
        // Singleton + persist across scene loads
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadProgress();
    }

    public int GetMaxCompletedDay()
    {
        return maxCompletedDay;
    }

    public int GetCurrentDay()
    {
        return currentDay;
    }

    public void DayCompleted()
    {
        maxCompletedDay = Mathf.Max(maxCompletedDay, currentDay);
        SaveProgress();

    }

    public void LoadNextDay()
    {
        currentDay++;
        LoadDay(currentDay);
    }

    public void LoadDay(int day)
    {
        currentDay = day;
        SceneManager.LoadSceneAsync("GameScene");
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt(SaveKey_MaxCompletedDay, maxCompletedDay);
        PlayerPrefs.Save();
    }

    public void LoadProgress()
    {
        maxCompletedDay = PlayerPrefs.GetInt(SaveKey_MaxCompletedDay, 0);
    }

    public void ResetProgress()
    {
        maxCompletedDay = 0;
        SaveProgress();
    }

}