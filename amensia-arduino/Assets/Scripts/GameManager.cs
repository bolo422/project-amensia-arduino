using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Player;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private float lightLerpSpeed;
    
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject interactMessage;
    private string filePath;
    private bool isPaused = false;
    [SerializeField] private int currentLevel;
    private Dictionary<int, int> keysPerLevel;
    private Dictionary<int, List<KeySpawner>> keySpawners;
    private Dictionary<int, int>  playerCurrentKeys;
    private Dictionary<int, bool> isDoorUnlocked;
    private Dictionary<int, FinalDoor> finalDoors;
    private Dictionary<int, Color> levelColors;
    [SerializeField] Color level0Color;
    [SerializeField] Color level1Color;
    [SerializeField] Color level2Color;
    [SerializeField] Light2D globalLight;
    private bool playerIsImmortal = false;
    [SerializeField] private GameObject lightHackText;
    [SerializeField] private GameObject playerImmortalText;

    public float LockPickingDifficulty { get; set; } = 0.1f;
    public float OilConsumption { get; set; } = 0.2f;
    public float OilRefuelQuanity { get; set; } = 65f;


    public bool ActivateArduino { get; set; } = false;
    private int minimumDial = 0;
    private int highestDial = 0;
    private int currentDial = 0;
    
    public event Action<int> OnDialChanged;
    public event Action OnDialLimitsChanged;
    
    private int CurrentLDR { get; set; } = 0;
    private int MinimumLDR { get; set; } = 0;

    private int HighestLDR { get; set; } = 0;

    private int CurrentDial 
    {
        get => currentDial;
        set
        {
            currentDial = value;
            OnDialChanged?.Invoke(value);
        }
    }

    public int MinimumDial
    {
        get => minimumDial;
        set
        {
            minimumDial = value;
            OnDialLimitsChanged?.Invoke();
        }
    }
    public int HighestDial
    {
        get => highestDial;
        set
        {
            highestDial = value;
            OnDialLimitsChanged?.Invoke();
        }
    }
    
    public bool InteractMessage
    {
        set => interactMessage.SetActive(value);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // this is the first instance
            
            // Set the file path based on the platform-specific persistent data path
            filePath = Path.Combine(Application.persistentDataPath, "game_data.txt");
        }
        else
        {
            Destroy(gameObject); // this must be a duplicate from a scene reload - DESTROY!
        }
        
        playerCurrentKeys = new Dictionary<int, int>();
        finalDoors = new Dictionary<int, FinalDoor>();
        keysPerLevel = new Dictionary<int, int>();
        levelColors = new Dictionary<int, Color>();
        isDoorUnlocked = new Dictionary<int, bool>();
        keySpawners = new Dictionary<int, List<KeySpawner>>();
        
        keysPerLevel.Add(0, 1);
        keysPerLevel.Add(1, 3);
        keysPerLevel.Add(2, 3);
        
        levelColors.Add(0, level0Color);
        levelColors.Add(1, level1Color);
        levelColors.Add(2, level2Color);
        
        foreach (var finalDoor in FindObjectsOfType<FinalDoor>())
        {
            finalDoors.Add(finalDoor.Level, finalDoor);
        }


        if (highestDial == 0)
            highestDial = 1023;
        if (HighestLDR == 0)
            HighestLDR = 1023;
        
        // Load values from the file on game startup
        LoadValuesFromFile();
    }

    private void Start()
    {

        foreach (var door in FindObjectsOfType<FinalDoor>())
        {
            isDoorUnlocked.Add(door.Level, false);
        }
        
        
        foreach (var keySpawner in FindObjectsOfType<KeySpawner>())
        {
            if(keySpawners.ContainsKey(keySpawner.Level))
                keySpawners[keySpawner.Level].Add(keySpawner);
            else
                keySpawners.Add(keySpawner.Level, new List<KeySpawner>{keySpawner});
        }

        foreach (var keySpawner in keySpawners[currentLevel])
        {
            keySpawner.SpawnKey();
        }
    }

    public bool FinalDoorUnlocked(int level)
    {
        return isDoorUnlocked[level];
    }

    public void AddKey(int level)
    {
        if(playerCurrentKeys.ContainsKey(level))
            playerCurrentKeys[level]++;
        else
        {
            playerCurrentKeys.Add(level, 1);
        }
        
        if (playerCurrentKeys[level] >= keysPerLevel[currentLevel])
        {
            isDoorUnlocked[level] = true;
        }
    }

    private void Update()
    {
        if (!ActivateArduino)
        {
            if (Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus))
                UpdateArduinoLDRInput(Mathf.Clamp(CurrentLDR - 20, MinimumLDR, HighestLDR));
            if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
                UpdateArduinoLDRInput(Mathf.Clamp(CurrentLDR + 20, MinimumLDR, HighestLDR));
        }
        
        if(!isPaused)
            PlayerLamp.Instance.ChangeLightPercentage(CalculatePercentage(CurrentLDR, MinimumLDR, HighestLDR), lightLerpSpeed);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (globalLight.intensity == 0) {
                globalLight.intensity = 0.8f;
                lightHackText.SetActive(true);
            }
            else
            {
                globalLight.intensity = 0;
                lightHackText.SetActive(false);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerIsImmortal = !playerIsImmortal;
            playerImmortalText.SetActive(playerIsImmortal);
        }
        
        
    }

    public void PauseGame()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        Time.timeScale = pauseMenu.activeSelf ? 0 : 1;
    }

    private static float CalculatePercentage(float value, float min, float max)
    {
        value = Mathf.Clamp(value, min, max);
        return InvertValue((value - min) * 100 / (max - min), 100);
    }

    public void UpdateArduinoInput(int dial, int volt, int ldr)
    {
        //Debug.Log(dial + volt + ldr);
        UpdateArduinoDialInput(dial);
        UpdateArduinoLDRInput(ldr);
    }

    private static float InvertValue(float value, float maxValue)
    {
        return (value - maxValue) * -1;
    }

    public void UpdateArduinoDialInput(int dial)
    {
        CurrentDial = dial;
    }

    public void UpdateArduinoLDRInput(int ldr)
    {
        CurrentLDR = ldr;
    }

    public void SetMaxLDR()
    {
        MinimumLDR = CurrentLDR;

        SaveValuesToFile();
    }

    public void SetMinLDR()
    {
        HighestLDR = CurrentLDR;

        SaveValuesToFile();
    }

    public void SetMaxDial()
    {
        if(CurrentDial < 500)
            MinimumDial = CurrentDial;
        else
            HighestDial = CurrentDial;

        SaveValuesToFile();
    }

    public void SetMinDial()
    {
        if(CurrentDial > 500)
            HighestDial = CurrentDial;
        else
            MinimumDial = CurrentDial;

        SaveValuesToFile();
    }
    
    private void SaveValuesToFile()
    {
        Debug.Log("Saving values -> | minimum ld: " + MinimumLDR + " | maximum ldr: " + HighestLDR + " | minimum dial: " + minimumDial + " | maximum dial: " + highestDial);
        CorrectLdrAndDialEqualValues();
        try
        {
            // Create a new StreamWriter to write the values to the file
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"minimum ldr: {MinimumLDR}");
                writer.WriteLine($"maximum ldr: {HighestLDR}");
                writer.WriteLine($"minimum dial: {minimumDial}");
                writer.WriteLine($"maximum dial: {highestDial}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save values to file: " + e.Message);
        }
    }

    private void LoadValuesFromFile()
    {
        if (File.Exists(filePath))
        {
            try
            {
                // Create a new StreamReader to read the values from the file
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("minimum ldr:"))
                        {
                            MinimumLDR = int.Parse(line.Substring("minimum ldr: ".Length));
                        }
                        else if (line.StartsWith("maximum ldr:"))
                        {
                            HighestLDR = int.Parse(line.Substring("maximum ldr: ".Length));
                        }
                        else if (line.StartsWith("minimum dial:"))
                        {
                            minimumDial = int.Parse(line.Substring("minimum dial: ".Length));
                        }
                        else if (line.StartsWith("maximum dial:"))
                        {
                            highestDial = int.Parse(line.Substring("maximum dial: ".Length));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load values from file: " + e.Message);
            }
        }
        if(CorrectLdrAndDialEqualValues())
            SaveValuesToFile();
    }

    private bool CorrectLdrAndDialEqualValues()
    {
        bool equal = false;
        if (highestDial == minimumDial)
        {
            HighestDial++;
            equal = true;            
        }

        if (HighestLDR == MinimumLDR)
        {
            HighestLDR++;
            equal = true;            
        }
        return equal;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void NextLevel(FinalDoor door)
    {
        currentLevel = door.Level + 1;

        if (LastLevel())
            Credits();
        
        foreach (var keySpawner in FindObjectsOfType<KeySpawner>())
        {
            keySpawner.DestroyKey();
        }
        foreach (var keySpawner in keySpawners[currentLevel])
        {
            keySpawner.SpawnKey();
        }
        Destroy(door.gameObject);
    }

    private void Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    private bool LastLevel()
    {
        foreach (var door in finalDoors.Values)
        {
            if (door.Level == currentLevel)
                return false;
        }
        return true;
    }

    public Color GetLevelColor(int level)
    {
        return levelColors[level];
    }

    public void SetVolume(Slider slider)
    {
        AudioListener.volume = slider.value;
    }

    public bool IsPlayerImmortal()
    {
        return playerIsImmortal;
    }
}
