using System;
using System.IO;
using Player;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private float lightLerpSpeed;
    
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject interactMessage;
    private string filePath;
    private bool isPaused = false;

    public float LockPickingDifficulty { get; set; } = 0.1f;


    public bool activateArduino = false;
    private int highestLDR = 0;
    private int minimumLDR = 0;
    private int currentLDR = 0;
    private int currentDial = 0;
    private int minimumDial = 0;
    private int highestDial = 0;
    
    public event Action<int> OnLDRChanged;
    public event Action<int> OnDialChanged;
    public event Action<int> OnMinimumLDRChanged;
    public event Action<int> OnMinimumDialChanged;
    public event Action<int> OnHighestLDRChanged;
    public event Action<int> OnHighestDialChanged;
    
    public int CurrentLDR
    {
        get => currentLDR;
        set
        {
            currentLDR = value;
            OnLDRChanged?.Invoke(currentLDR);
        }
    }
    
    public int CurrentDial
    {
        get => currentDial;
        set
        {
            currentDial = value;
            OnDialChanged?.Invoke(currentDial);
        }
    }

    public int MinimumLDR
    {
        get => minimumLDR;
        set
        {
            minimumLDR = value;
            OnMinimumLDRChanged?.Invoke(minimumLDR);
        }
    }
    
    public int MinimumDial
    {
        get => minimumDial;
        set
        {
            minimumDial = value;
            OnMinimumDialChanged?.Invoke(minimumDial);
        }
    }
    
    public int HighestLDR
    {
        get => highestLDR;
        set
        {
            highestLDR = value;
            OnHighestLDRChanged?.Invoke(highestLDR);
        }
    }
    
    public int HighestDial
    {
        get => highestDial;
        set
        {
            highestDial = value;
            OnHighestDialChanged?.Invoke(highestDial);
        }
    }
    
    public bool IsInteractMessageActive
    {
        set => interactMessage.SetActive(value);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // this is the first instance
            DontDestroyOnLoad(gameObject);
            
            // Set the file path based on the platform-specific persistent data path
            filePath = Path.Combine(Application.persistentDataPath, "game_data.txt");
        }
        else
        {
            Destroy(gameObject); // this must be a duplicate from a scene reload - DESTROY!
        }
        
        if (highestDial == 0)
            highestDial = 1023;
        if (highestLDR == 0)
            highestLDR = 1023;
        
        // Load values from the file on game startup
        LoadValuesFromFile();
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (!activateArduino)
        {
            if (Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus))
                UpdateArduinoLDRInput(Mathf.Clamp(currentLDR - 20, minimumLDR, highestLDR));
            if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
                UpdateArduinoLDRInput(Mathf.Clamp(currentLDR + 20, minimumLDR, highestLDR));
        }
        
        if(!isPaused)
            PlayerLamp.Instance.ChangeLightPercentage(CalculatePercentage(currentLDR, minimumLDR, highestLDR), lightLerpSpeed);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            Time.timeScale = pauseMenu.activeSelf ? 0 : 1;
        }
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
        if(currentDial > 500)
            HighestDial = CurrentDial;
        else
            MinimumDial = CurrentDial;

        SaveValuesToFile();
    }
    
    private void SaveValuesToFile()
    {
        Debug.Log("Saving values -> | minimum ld: " + minimumLDR + " | maximum ldr: " + highestLDR + " | minimum dial: " + minimumDial + " | maximum dial: " + highestDial);
        CorrectLdrAndDialEqualValues();
        try
        {
            // Create a new StreamWriter to write the values to the file
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"minimum ldr: {minimumLDR}");
                writer.WriteLine($"maximum ldr: {highestLDR}");
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
                            minimumLDR = int.Parse(line.Substring("minimum ldr: ".Length));
                        }
                        else if (line.StartsWith("maximum ldr:"))
                        {
                            highestLDR = int.Parse(line.Substring("maximum ldr: ".Length));
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

        if (highestLDR == minimumLDR)
        {
            HighestLDR++;
            equal = true;            
        }
        return equal;
    }
}
