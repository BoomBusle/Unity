using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

    public event Action OnGameOver;

    private GameSaveData _saveData;
    private string SaveFilePath => Path.Combine(Application.persistentDataPath, "gamedata.json");

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        LoadData();
        StartNewAttempt();
    }

    void Update()
    {
        if (isGameOver) return;

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= timeLimit)
        {
            Debug.Log("[GameManager] Час вичерпано!");
            TriggerGameOver();
        }
    }

    public void StartNewAttempt()
    {
        lives = maxLives;
        collisionCount = 0;
        elapsedTime = 0f;
        coinsCollected = 0;
        isGameOver = false;
        _saveData.totalAttempts++;
        Debug.Log($"[GameManager] Нова спроба #{_saveData.totalAttempts}. Життів: {lives}, Ліміт часу: {timeLimit}с");
    }

    public void CollectCoin()
    {
        if (isGameOver) return;
        coinsCollected++;
        _saveData.totalCoinsCollected++;
        Debug.Log($"[GameManager] Монету зібрано! Всього: {coinsCollected}");
    }

    public void LoseLife()
    {
        if (isGameOver) return;
        lives--;
        collisionCount++;
        _saveData.totalDeaths++;
        Debug.Log($"[GameManager] Зіткнення! Життів залишилось: {lives}");

        if (lives <= 0)
        {
            TriggerGameOver();
        }
    }

    public void FinishLevel()
    {
        if (isGameOver) return;
        isGameOver = true;

        RecordEntry record = new RecordEntry
        {
            coins = coinsCollected,
            time = elapsedTime,
            date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
        _saveData.records.Add(record);

        Debug.Log($"[GameManager] ФІНІШ! Монет: {coinsCollected}, Час: {elapsedTime:F1}с");
        PrintRecords();
        SaveData();
    }

    private void TriggerGameOver()
    {
        isGameOver = true;

        RecordEntry record = new RecordEntry
        {
            coins = coinsCollected,
            time = elapsedTime,
            date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
        _saveData.records.Add(record);

        Debug.Log($"[GameManager] ПРОГРАШ! Монет: {coinsCollected}, Зіткнень: {collisionCount}, Час: {elapsedTime:F1}с");
        PrintRecords();
        SaveData();

        OnGameOver?.Invoke();
    }

    private void PrintRecords()
    {
        Debug.Log("=== ТАБЛИЦЯ РЕКОРДІВ ===");
        List<RecordEntry> sorted = new List<RecordEntry>(_saveData.records);
        sorted.Sort((a, b) => b.coins.CompareTo(a.coins));
        int rank = 1;
        foreach (var r in sorted)
        {
            Debug.Log($"  #{rank}: Монет: {r.coins}, Час: {r.time:F1}с ({r.date})");
            rank++;
            if (rank > 10) break;
        }
        Debug.Log("========================");
    }

    private void LoadData()
    {
        if (File.Exists(SaveFilePath))
        {
            string json = File.ReadAllText(SaveFilePath);
            _saveData = JsonUtility.FromJson<GameSaveData>(json);
            Debug.Log($"[GameManager] Дані завантажено: спроб={_saveData.totalAttempts}, рекордів={_saveData.records.Count}");
        }
        else
        {
            _saveData = new GameSaveData();
            Debug.Log("[GameManager] Новий файл збереження.");
        }
    }

    private void SaveData()
    {
        string json = JsonUtility.ToJson(_saveData, true);
        File.WriteAllText(SaveFilePath, json);
        Debug.Log($"[GameManager] Дані збережено у {SaveFilePath}");
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
