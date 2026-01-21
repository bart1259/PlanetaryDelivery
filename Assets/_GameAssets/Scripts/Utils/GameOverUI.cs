using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set;}

    public GameObject gameOverPanel;
    public TMP_Text gameOverText;
    public TMP_Text packagesDeliveredText;
    public TMP_Text timeTakenText;
    public TMP_Text thankYouText;

    public Button MainMenuButton;
    public Button ReplayDayButton;
    public Button NextDayButton;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        
        MainMenuButton.onClick.AddListener(() => {
            SceneManager.LoadScene("GameMenu");
        });

        ReplayDayButton.onClick.AddListener(() => {
            GlobalStateManager.Instance.LoadDay(GlobalStateManager.Instance.GetCurrentDay());
        });

        NextDayButton.onClick.AddListener(() => {
            GlobalStateManager.Instance.LoadNextDay();
        });
    }

    public void Win(int dayCompleted, int packagesDelivered, int totalPackages, float timeTaken)
    {
        gameOverPanel.SetActive(true);
        gameOverText.text = dayCompleted == 5 ? "Game Complete!" : "Day " + dayCompleted + " Complete";
        packagesDeliveredText.text = "Packages Delivered: " + packagesDelivered + " / " + totalPackages;
        timeTakenText.text = "Time Taken: " + ((int)timeTaken) + " s";
        thankYouText.gameObject.SetActive(dayCompleted == 5);

        if (dayCompleted == 5)
        {
            NextDayButton.interactable = false;
        }
    }

    public void Lose(int dayCompleted, int packagesDelivered, int totalPackages)
    {
        gameOverPanel.SetActive(true);
        gameOverText.text = "Time's Up!";
        packagesDeliveredText.text = "Packages Delivered: " + packagesDelivered + " / " + totalPackages;
        timeTakenText.text = "";
        NextDayButton.interactable = false;
    }
}
