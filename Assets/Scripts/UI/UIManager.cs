using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject victoryMenu;
    [SerializeField] private GameObject defeatMenu;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        pauseMenu.SetActive(false);
        victoryMenu.SetActive(false);
        defeatMenu.SetActive(false);
    }

    public void Pause()
    {
        if (!GameManager.Instance.isPause)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            GameManager.Instance.isPause = true;
        }
        else
        {
            Continue();
        }
    }

    public void Continue()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameManager.Instance.isPause = false;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Again()
    {
        SceneManager.LoadScene(0);
        Continue();
    }

    public void UpdateTimer(float timer)
    {
        float minutes = Mathf.FloorToInt(timer / 60);
        float seconds = Mathf.FloorToInt(timer % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

    }

    public void Victory()
    {
        victoryMenu.SetActive(true);
        Pause();
    }

    public void Defeat()
    {
        defeatMenu.SetActive(true);
        Pause();
    }
}
