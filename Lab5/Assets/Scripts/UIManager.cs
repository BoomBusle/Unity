using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject mainMenuPanel;
    public GameObject hudPanel;
    public GameObject gameOverPanel;
    public GameObject recordsPanel;

    [Header("Login")]
    public InputField loginInput;
    public Button loginButton;

    [Header("Main Menu")]
    public Button playButton;
    public Button recordsButton;
    public Button exitButton;

    [Header("HUD")]
    public Text livesText;
    public Text coinsText;
    public Text timerText;
    public Text collisionsText;

    [Header("Game Over")]
    public Text gameOverTitle;
    public Text gameOverInfo;
    public Text gameOverRecordsText;
    public Button restartButton;
    public Button menuButton;

    [Header("Records Panel")]
    public Text recordsTitle;
    public Text recordsTableText;
    public Button recordsBackButton;

    private string _currentLogin = "";

    void Start()
    {
        ShowLoginPanel();
        BindButtons();
        SubscribeEvents();
    }

    void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private void SubscribeEvents()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver += OnGameOver;
            GameManager.Instance.OnLevelFinished += OnLevelFinished;
        }
    }

    private void UnsubscribeEvents()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver -= OnGameOver;
            GameManager.Instance.OnLevelFinished -= OnLevelFinished;
        }
    }

    void Update()
    {
        if (hudPanel != null && hudPanel.activeSelf && GameManager.Instance != null)
        {
            UpdateHUD();
        }
    }

    private void BindButtons()
    {
        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginConfirm);

        if (playButton != null)
            playButton.onClick.AddListener(OnPlay);

        if (recordsButton != null)
            recordsButton.onClick.AddListener(OnShowRecords);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExit);

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestart);

        if (menuButton != null)
            menuButton.onClick.AddListener(OnBackToMenu);

        if (recordsBackButton != null)
            recordsBackButton.onClick.AddListener(OnRecordsBack);
    }

    private void HideAllPanels()
    {
        if (loginPanel != null) loginPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (recordsPanel != null) recordsPanel.SetActive(false);
    }

    private void ShowLoginPanel()
    {
        HideAllPanels();
        if (loginPanel != null) loginPanel.SetActive(true);
        if (loginInput != null) loginInput.text = "";
        Time.timeScale = 0f;
    }

    private void ShowMainMenu()
    {
        HideAllPanels();
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void ShowHUD()
    {
        HideAllPanels();
        if (hudPanel != null) hudPanel.SetActive(true);
    }

    private void ShowGameOverPanel(string title, Color titleColor, string info)
    {
        HideAllPanels();
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        if (gameOverTitle != null)
        {
            gameOverTitle.text = title;
            gameOverTitle.color = titleColor;
        }

        if (gameOverInfo != null)
            gameOverInfo.text = info;

        if (gameOverRecordsText != null)
            gameOverRecordsText.text = BuildRecordsTable();

        Time.timeScale = 0f;
    }

    private void ShowRecordsPanel()
    {
        HideAllPanels();
        if (recordsPanel != null) recordsPanel.SetActive(true);

        if (recordsTableText != null)
            recordsTableText.text = BuildRecordsTable();

        Time.timeScale = 0f;
    }

    private void OnLoginConfirm()
    {
        if (loginInput == null) return;
        string login = loginInput.text.Trim();
        if (string.IsNullOrEmpty(login)) return;

        _currentLogin = login;
        ShowMainMenu();
    }

    private void OnPlay()
    {
        ShowHUD();
        if (GameManager.Instance != null)
            GameManager.Instance.StartGame(_currentLogin);
    }

    private void OnShowRecords()
    {
        ShowRecordsPanel();
    }

    private void OnExit()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.QuitGame();
    }

    private void OnRestart()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartLevel();
    }

    private void OnBackToMenu()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartLevel();
    }

    private void OnRecordsBack()
    {
        ShowMainMenu();
    }

    private void UpdateHUD()
    {
        GameManager gm = GameManager.Instance;

        if (livesText != null)
            livesText.text = $"Lives: {gm.lives}/{gm.maxLives}";

        if (coinsText != null)
            coinsText.text = $"Coins: {gm.coinsCollected}";

        if (timerText != null)
        {
            int min = (int)(gm.elapsedTime / 60f);
            int sec = (int)(gm.elapsedTime % 60f);
            timerText.text = $"Time: {min:00}:{sec:00}";

            float remaining = gm.timeLimit - gm.elapsedTime;
            timerText.color = (remaining < 15f && !gm.isGameOver) ? Color.red : Color.white;
        }

        if (collisionsText != null)
            collisionsText.text = $"Collisions: {gm.collisionCount}";
    }

    private void OnGameOver()
    {
        GameManager gm = GameManager.Instance;
        string reason = gm.lives <= 0 ? "Lives = 0" : "Time is up!";
        string info = $"{reason}\nCoins: {gm.coinsCollected}  |  Collisions: {gm.collisionCount}\nTime: {gm.elapsedTime:F1}s";
        ShowGameOverPanel("GAME OVER", Color.red, info);
    }

    private void OnLevelFinished()
    {
        GameManager gm = GameManager.Instance;
        string info = $"Coins: {gm.coinsCollected}  |  Collisions: {gm.collisionCount}\nTime: {gm.elapsedTime:F1}s";
        ShowGameOverPanel("FINISH!", Color.green, info);
    }

    private string BuildRecordsTable()
    {
        if (GameManager.Instance == null) return "No records";

        List<RecordEntry> records = GameManager.Instance.GetSortedRecords(10);
        if (records.Count == 0) return "No records yet";

        string table = "# | Player       | Coins | Collisions | Time    | Date\n";
        table +=       "--|-------------|-------|-----------|---------|-------------------\n";

        for (int i = 0; i < records.Count; i++)
        {
            RecordEntry r = records[i];
            string name = r.playerName;
            if (string.IsNullOrEmpty(name)) name = "???";
            if (name.Length > 12) name = name.Substring(0, 12);

            table += $"{i + 1,2} | {name,-12} | {r.coins,5} | {r.collisions,9} | {r.time,5:F1}s | {r.date}\n";
        }

        return table;
    }
}
