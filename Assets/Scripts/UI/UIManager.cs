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
    [SerializeField] private GameObject uiBuilding;
    [SerializeField] private Transform buttonsBuildings;
    public Button btnRepair;

    private GameManager gameManager;
    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private Hero hero;
    [SerializeField] private GameObject basePlayer;

    public bool inGamePause;
    [SerializeField] Image btnPauseGame;
    [SerializeField] Sprite spritePause, spritePlay;

    float currentTimeScale = 1f;

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
        Time.timeScale = currentTimeScale;
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

    public void CreateButtonsBuilding(House house)
    {
        foreach (ButtonUnit btn in house.myButtonsToCreate)
        {
            GameObject btnGO = btn.SetupButton(buttonsBuildings, house);
            house.myButtonsToDisplay.Add(btnGO);
        }
    }

    public void DisplayUIBuilding(House house, bool value)
    {
        uiBuilding.SetActive(value);
        foreach(GameObject btn in house.myButtonsToDisplay)
        {
            btn.SetActive(value);
        }
    }

    public void PauseGame()
    {
        if (gameManager.isPause) return;
        if (!inGamePause)
        {
            inGamePause = true;
            Time.timeScale = 0;
            btnPauseGame.sprite = spritePlay;
        }
        else
        {
            inGamePause = false;
            Time.timeScale = currentTimeScale;
            btnPauseGame.sprite = spritePause;
        }
    }

    public void SpeedGame(float speed)
    {
        currentTimeScale = speed;
        if (!inGamePause) Time.timeScale = currentTimeScale;        
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

        ////////////////////////////// PAUSE IN GAME //////////////////////////////////////////
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
    }
}
