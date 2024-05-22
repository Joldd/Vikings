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
    [SerializeField] private UITroopInfo uiTroopInfo;

    private GameManager gameManager;
    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private Hero hero;
    [SerializeField] private GameObject basePlayer;

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
        gameManager = GameManager.Instance;

        pauseMenu.SetActive(false);
        victoryMenu.SetActive(false);
        defeatMenu.SetActive(false);
    }

    public void Pause()
    {
        if (!gameManager.isPause)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            gameManager.isPause = true;
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
        gameManager.isPause = false;
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

    public void DisplayTroopInfos(Troop troop, bool value = true)
    {
        if (value)
        {
            uiTroopInfo.SetupTroop(troop);
        }
        uiTroopInfo.gameObject.SetActive(value);
    }

    private void Update()
    {
        if (hero.isDie)
        {
            hero.timerRespawn -= Time.deltaTime;
            hero.textTimer.text = Mathf.Round(hero.timerRespawn).ToString();
            if (hero.timerRespawn < 0)
            {
                hero.timerRespawn = hero.timerRespawnMax;
                hero.Respawn();
            }
        }
    }
}
