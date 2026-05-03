using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Settings")]
    public int maxLives = 3;
    public float timeLimit = 120f;

    [Header("Runtime State")]
    public int lives;
    public int collisionCount;
    public float elapsedTime;
    public int coinsCollected;
    public bool isGameOver;
    public bool isPlaying;

    public string playerName = "";

    public event Action OnGameOver;
    public event Action OnLevelFinished;

    private GameSaveData _saveData;
    private string SaveFilePath => Path.Combine(Application.persistentDataPath, "gamedata.json");

    public List<RecordEntry> Records => _saveData.records;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        LoadData();
    }

    void Update()
    {
        if (!isPlaying || isGameOver) return;

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= timeLimit)
        {
            TriggerGameOver();
        }
    }

    public void StartGame(string login)
    {
        playerName = login;
        lives = maxLives;
        collisionCount = 0;
        elapsedTime = 0f;
        coinsCollected = 0;
        isGameOver = false;
        isPlaying = true;
        _saveData.totalAttempts++;
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        isPlaying = false;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (!isGameOver)
        {
            isPlaying = true;
            Time.timeScale = 1f;
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void CollectCoin()
    {
        if (isGameOver || !isPlaying) return;
        coinsCollected++;
        _saveData.totalCoinsCollected++;
    }

    public void LoseLife()
    {
        if (isGameOver || !isPlaying) return;
        lives--;
        collisionCount++;
        _saveData.totalDeaths++;

        if (lives <= 0)
        {
            TriggerGameOver();
        }
    }

    public void FinishLevel()
    {
        if (isGameOver) return;
        isGameOver = true;
        isPlaying = false;

        SaveRecord();
        OnLevelFinished?.Invoke();
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        isPlaying = false;

        SaveRecord();
        OnGameOver?.Invoke();
    }

    private void SaveRecord()
    {
        RecordEntry record = new RecordEntry
        {
            playerName = playerName,
            coins = coinsCollected,
            collisions = collisionCount,
            time = elapsedTime,
            date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
        _saveData.records.Add(record);
        SaveData();
    }

    public List<RecordEntry> GetSortedRecords(int maxCount = 10)
    {
        List<RecordEntry> sorted = new List<RecordEntry>(_saveData.records);
        sorted.Sort((a, b) =>
        {
            int cmp = b.coins.CompareTo(a.coins);
            if (cmp != 0) return cmp;
            return a.time.CompareTo(b.time);
        });
        if (sorted.Count > maxCount)
            sorted.RemoveRange(maxCount, sorted.Count - maxCount);
        return sorted;
    }

    private void LoadData()
    {
        if (File.Exists(SaveFilePath))
        {
            string json = File.ReadAllText(SaveFilePath);
            _saveData = JsonUtility.FromJson<GameSaveData>(json);
        }
        else
        {
            _saveData = new GameSaveData();
        }
    }

    private void SaveData()
    {
        string json = JsonUtility.ToJson(_saveData, true);
        File.WriteAllText(SaveFilePath, json);
    }

    void OnApplicationQuit()
    {
        SaveData();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SaveData();
            Instance = null;
        }
    }
}
