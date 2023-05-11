using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool activateArduino = false;
    public int highestLDR = 0;
    public int minimumLDR = 0;
    public int currentLDR = 0;
    public int currentDial = 0;
    public int minimumDial = 0;
    public int highestDial = 0;
    [SerializeField] private GameObject pauseMenu;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // this is the first instance
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // this must be a duplicate from a scene reload - DESTROY!
        }
    }

    private void Start()
    {
        if (highestDial == 0)
            highestDial = 1023;
        if (highestLDR == 0)
            highestLDR = 1023;
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
        PlayerLamp.Instance.ChangeLightPercentage(CalculatePercentage(currentLDR, minimumLDR, highestLDR));

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            Time.timeScale = pauseMenu.activeSelf ? 0 : 1;
        }
    }

    private static float CalculatePercentage(float value, float min, float max)
    {
        return InvertValue((value - min) * 100 / (max - min), 100);
    }

    public void UpdateArduinoInput(int dial, int volt, int ldr)
    {
        //Debug.Log(dial + volt + ldr);
        UpdateArduinoDialInput(Mathf.Clamp(dial, minimumDial, highestDial));
        UpdateArduinoLDRInput(Mathf.Clamp(ldr, minimumLDR, highestLDR));
    }

    private static float InvertValue(float value, float maxValue)
    {
        return (value - maxValue) * -1;
    }

    public void UpdateArduinoDialInput(int dial)
    {
        currentDial = dial;
    }

    public void UpdateArduinoLDRInput(int ldr)
    {
        currentLDR = ldr;
    }

    public void SetMaxLDR()
    {
        minimumLDR = currentLDR;
    }

    public void SetMinLDR()
    {
        highestLDR = currentLDR;
    }

    public void SetMaxDial()
    {
        minimumDial = currentDial;
    }

    public void SetMinDial()
    {
        highestDial = currentDial;
    }
}
