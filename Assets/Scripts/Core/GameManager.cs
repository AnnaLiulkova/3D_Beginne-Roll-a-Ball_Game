using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; 
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject[] enemiesInLevel;
    public static GameManager Instance { get; private set; }

    [Header("In-Game UI")]
    public TextMeshProUGUI countText;
    public TextMeshProUGUI timerText; 
    
    [Header("Pause UI")]
    public GameObject pausePanel;         
    public Image pauseHUDButtonImage;     
    public Sprite pauseIcon;              
    public Sprite playIcon;               

    [Header("End Game UI")]
    public GameObject endGamePanel; 
    public GameObject winHeader;    
    public GameObject loseHeader;   
    public TextMeshProUGUI endTitleText; 
    
    public TextMeshProUGUI endTotalScoreText; 
    public TextMeshProUGUI endRegularScoreText; 
    public TextMeshProUGUI endSuperScoreText; 
    public TextMeshProUGUI endTimeText;  

    [Header("Level Rules")]
    public int winScore = 15; 
    public bool isTimeAttackMode = false; 
    public float timeLimit = 60f; 

    private int currentScore = 0;
    private int regularBonusCount = 0;
    private int superBonusCount = 0;

    private float currentTime;
    private float timePlayed = 0f; 
    public bool isGameOver = false;
    private bool isPaused = false; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        Time.timeScale = 1f; 
    }

    void Start()
    {
        if (endGamePanel != null) endGamePanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        
        UpdateScoreUI();

        currentTime = timeLimit;
        if (timerText != null)
        {
            timerText.gameObject.SetActive(isTimeAttackMode);
            UpdateTimerUI();
        }
        int savedEnemyCount = PlayerPrefs.GetInt("EnemyCount", 1);

       if (enemiesInLevel != null && enemiesInLevel.Length > 0)
       {
          for (int i = 0; i < enemiesInLevel.Length; i++)
          {
            if (enemiesInLevel[i] != null)
            {
               enemiesInLevel[i].SetActive(i < savedEnemyCount); 
            }
          }
       }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && !isGameOver)
        {
            TogglePause();
        }

        if (!isGameOver && !isPaused) timePlayed += Time.deltaTime; 

        if (isTimeAttackMode && !isGameOver && !isPaused)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                currentTime = 0;
                PlayerController player = FindFirstObjectByType<PlayerController>();
                if (player != null) player.Die("Time's Up!");
                else TriggerGameOver("Time's Up!");
            }
            UpdateTimerUI();
        }
    }

    public void TogglePause()
    {
        if (isGameOver) return;
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; 
        pausePanel.SetActive(true);
        if (pauseHUDButtonImage != null && playIcon != null) pauseHUDButtonImage.sprite = playIcon;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; 
        pausePanel.SetActive(false);
        if (pauseHUDButtonImage != null && pauseIcon != null) pauseHUDButtonImage.sprite = pauseIcon;
    }

    public void AddScore(int points)
    {
        if (isGameOver) return;
        
        currentScore += points;

        if (points == 5) superBonusCount++;
        else regularBonusCount++;

        UpdateScoreUI();
        CheckWinCondition();
    }

    private void UpdateScoreUI()
    {
        if (countText != null)
        {
            if (winScore >= 9999) countText.text = "Count: " + currentScore.ToString();
            else countText.text = "Count: " + currentScore.ToString() + " / " + winScore.ToString();
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;
        int min = Mathf.FloorToInt(currentTime / 60F);
        int sec = Mathf.FloorToInt(currentTime - min * 60);
        timerText.text = string.Format("{0:00}:{1:00}", min, sec);
    }

    private void CheckWinCondition()
    {
        if (currentScore >= winScore) TriggerWin();
    }

    private void TriggerWin()
    {
        isGameOver = true;
        
        EnemyMovement[] allEnemies = FindObjectsByType<EnemyMovement>(FindObjectsSortMode.None);
        
        foreach (EnemyMovement enemy in allEnemies)
        {
            if (enemy != null) enemy.Die();
        }
        AudioManager.Instance.PlayWin();
        StopAllSystems();
        StartCoroutine(ShowEndScreen(true));
    }

    public void TriggerGameOver(string message)
    {
        if (isGameOver) return;
        isGameOver = true;
        
        EnemyMovement[] allEnemies = FindObjectsByType<EnemyMovement>(FindObjectsSortMode.None);
        
        foreach (EnemyMovement enemy in allEnemies)
        {
            if (enemy != null) enemy.Stop();
        }
        AudioManager.Instance.PlayLose();
        StopAllSystems();
        StartCoroutine(ShowEndScreen(false));
    }

    private void StopAllSystems()
    {
        BonusSpawner spawner = FindFirstObjectByType<BonusSpawner>();
        if (spawner != null) spawner.StopSpawning();
    }

    private IEnumerator ShowEndScreen(bool isWin)
    {
        yield return new WaitForSecondsRealtime(0.5f); 
        
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (countText != null) countText.gameObject.SetActive(false);
        if (pauseHUDButtonImage != null) pauseHUDButtonImage.gameObject.SetActive(false); 

        endGamePanel.SetActive(true);

        if (winHeader != null) winHeader.SetActive(isWin);
        if (loseHeader != null) loseHeader.SetActive(!isWin);
        
        if (endTitleText != null) endTitleText.text = "Complete!";

        int min = Mathf.FloorToInt(timePlayed / 60F);
        int sec = Mathf.FloorToInt(timePlayed - min * 60);
        int ms = Mathf.FloorToInt((timePlayed * 100F) % 100F);
        if (endTimeText != null) endTimeText.text = string.Format("{0:00}:{1:00}:{2:00}", min, sec, ms);
        
        if (endTotalScoreText != null) endTotalScoreText.text = "Score: " + currentScore.ToString();
        if (endRegularScoreText != null) endRegularScoreText.text = regularBonusCount.ToString();
        if (endSuperScoreText != null) endSuperScoreText.text = superBonusCount.ToString();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu");
    }
}