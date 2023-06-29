using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ArduinoSettings : MonoBehaviour
{
    public static ArduinoSettings Instance { get; private set; }

    private string filePath;

    private int CurrentLDR { get; set; } = 0;
    private int MinimumLDR { get; set; } = 0;

    private int HighestLDR { get; set; } = 0;

    private int CurrentDial { get; set; } = 0;

    public int MinimumDial { get; set; } = 0;

    public int HighestDial { get; set; } = 0;

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
        
        if (HighestDial == 0)
            HighestDial = 1023;
        if (HighestLDR == 0)
            HighestLDR = 1023;
        
        // Load values from the file on game startup
        LoadValuesFromFile();
    }

    public void UpdateArduinoInput(int dial, int volt, int ldr)
    {
        //Debug.Log(dial + volt + ldr);
        UpdateArduinoDialInput(dial);
        UpdateArduinoLDRInput(ldr);
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
        Debug.Log("Saving values -> | minimum ld: " + MinimumLDR + " | maximum ldr: " + HighestLDR + " | minimum dial: " + MinimumDial + " | maximum dial: " + HighestDial);
        CorrectLdrAndDialEqualValues();
        try
        {
            // Create a new StreamWriter to write the values to the file
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"minimum ldr: {MinimumLDR}");
                writer.WriteLine($"maximum ldr: {HighestLDR}");
                writer.WriteLine($"minimum dial: {MinimumDial}");
                writer.WriteLine($"maximum dial: {HighestDial}");
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
                            MinimumDial = int.Parse(line.Substring("minimum dial: ".Length));
                        }
                        else if (line.StartsWith("maximum dial:"))
                        {
                            HighestDial = int.Parse(line.Substring("maximum dial: ".Length));
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
        if (HighestDial == MinimumDial)
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

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level1");
    }

    public void Quit()
    {
        Application.Quit();
    }
}