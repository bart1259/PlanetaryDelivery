using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class StartScreenManager : MonoBehaviour
{
    public Button night1Button;
    public Button night2Button;
    public Button night3Button;
    public Button night4Button;
    public Button night5Button;
    public Button quitButton;

    public TMP_Text loadingText;

    public Image fadeImage;

    private bool day1AnimationPlaying = false;

    public TMP_Text day1AnimationText1;
    public TMP_Text day1AnimationText2;
    public TMP_Text day1AnimationText3;

    void Start()
    {
        night1Button.onClick.AddListener(() => OnStartDay(1));
        night2Button.onClick.AddListener(() => OnStartDay(2));
        night3Button.onClick.AddListener(() => OnStartDay(3));
        night4Button.onClick.AddListener(() => OnStartDay(4));
        night5Button.onClick.AddListener(() => OnStartDay(5));
        quitButton.onClick.AddListener(() => Application.Quit());
    
        int maxCompletedDay = GlobalStateManager.Instance.GetMaxCompletedDay();
        night1Button.interactable = true;
        night2Button.interactable = maxCompletedDay >= 1;
        night3Button.interactable = maxCompletedDay >= 2;
        night4Button.interactable = maxCompletedDay >= 3;
        night5Button.interactable = maxCompletedDay >= 4;

        loadingText.text = "";

        day1AnimationText1.gameObject.SetActive(false);
        day1AnimationText2.gameObject.SetActive(false);
        day1AnimationText3.gameObject.SetActive(false);
    }

    void OnStartDay(int day)
    {
        if (day == 1)
        {
            StartCoroutine(StartDay1Animation(day));
        } else
        {
            StartCoroutine(LoadNewDay(day));
        }
    }

    IEnumerator StartDay1Animation(int day)
    {
        // Fade to black
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            fadeImage.color = new Color(0f, 0f, 0f, (i + 1) / 100f);
        }

        // Show Text
        day1AnimationText1.gameObject.SetActive(true);
        for (int i = 0; i < 100; i++)
        {
            day1AnimationText1.color = new Color(1.0f, 1.0f, 1.0f, (i + 1) / 100f);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(3.0f);

        day1AnimationText2.gameObject.SetActive(true);
        for (int i = 0; i < 100; i++)
        {
            day1AnimationText2.color = new Color(1.0f, 1.0f, 1.0f, (i + 1) / 100f);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(3.0f);

        day1AnimationText3.gameObject.SetActive(true);
        for (int i = 0; i < 100; i++)
        {
            day1AnimationText3.color = new Color(1.0f, 1.0f, 1.0f, (i + 1) / 100f);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(3.0f);

        // Fade out
        for (int i = 0; i < 100; i++)
        {
            day1AnimationText1.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - (i + 1) / 100f);
            day1AnimationText2.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - (i + 1) / 100f);
            day1AnimationText3.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - (i + 1) / 100f);
            yield return new WaitForSeconds(0.01f);
        }
        day1AnimationText1.gameObject.SetActive(false);
        day1AnimationText2.gameObject.SetActive(false);
        day1AnimationText3.gameObject.SetActive(false);

        loadingText.gameObject.SetActive(true);
        loadingText.text = "Loading...";
        yield return new WaitForSeconds(0.1f);
        
        GlobalStateManager.Instance.LoadDay(day);

    }

    IEnumerator LoadNewDay(int day)
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
    
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            fadeImage.color = new Color(0f, 0f, 0f, (i + 1) / 100f);
        }

        loadingText.gameObject.SetActive(true);
        loadingText.text = "Loading...";
        yield return new WaitForSeconds(0.1f);
        
        GlobalStateManager.Instance.LoadDay(day);
    }
}
