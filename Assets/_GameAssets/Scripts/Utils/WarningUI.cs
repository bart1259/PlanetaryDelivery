using UnityEngine;
using TMPro;

public class WarningUI : MonoBehaviour
{
    public static WarningUI Instance { get; private set;}

    public TMP_Text warningText;
    private float timer = 0.0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        warningText.text = "";
        timer = 0.0f;
    }

    public void ShowWarning(string message)
    {
        warningText.text = message;
        timer = 3.0f; // Show for 3 seconds
    }

    public void Update()
    {
        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                warningText.text = "";
            }
        }
    }

}
