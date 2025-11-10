using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EscapeRoomTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float startTimeInMinutes = 10f;
    [SerializeField] private float penaltyTimeInSeconds = 60f; // 1 minute penalty
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color dangerColor = Color.red;
    [SerializeField] private float warningThreshold = 180f; // 3 minutes
    [SerializeField] private float dangerThreshold = 60f; // 1 minute
    
    [Header("End Game Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject successPanel;
    [SerializeField] private string mainSceneName = "BasicScene";
    
    private float timeRemaining;
    private bool timerRunning = true;
    private bool gameEnded = false;
    
    public static EscapeRoomTimer Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Hide panels at start
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (successPanel != null) successPanel.SetActive(false);
    }

    void Start()
    {
        timeRemaining = startTimeInMinutes * 60f;
        UpdateTimerDisplay();
    }

    void Update()
    {
        if (!timerRunning || gameEnded) return;

        timeRemaining -= Time.deltaTime;
        
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            OnTimeUp();
        }
        
        UpdateTimerDisplay();
    }

    void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        
        // Change color based on time remaining
        if (timeRemaining <= dangerThreshold)
        {
            timerText.color = dangerColor;
        }
        else if (timeRemaining <= warningThreshold)
        {
            timerText.color = warningColor;
        }
        else
        {
            timerText.color = normalColor;
        }
    }

    public void ApplyPenalty()
    {
        if (gameEnded) return;
        
        timeRemaining -= penaltyTimeInSeconds;
        
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            OnTimeUp();
        }
    }

    public void OnAccessGranted()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        timerRunning = false;
        
        if (successPanel != null)
        {
            successPanel.SetActive(true);
        }
    }

    void OnTimeUp()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        timerRunning = false;
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    // Public method to restart from UI buttons
    public void RestartGame()
    {
        SceneManager.LoadScene(mainSceneName);
    }

    // Public method to quit from UI buttons
    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}