using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool activateArduino = false;
    public int minimumLight = 0;
    public int maxLight = 1000;
    public int currentLight = 0;
    public int currentLockPickValue = 0;
    public int maxLockPickValue = 0;
    public int minLockPickValue = 1000;
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
        if (maxLockPickValue == 0)
            maxLockPickValue = 1023;
        if (maxLight == 0)
            maxLight = 1023;
    }

    private void Update()
    {
        if (!activateArduino)
        {
            if (Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus))
                UpdateArduinoLDRInput(Mathf.Clamp(currentLight + 1, minimumLight, maxLight));
            if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
                UpdateArduinoLDRInput(Mathf.Clamp(currentLight - 1, minimumLight, maxLight));
        }
        PlayerLamp.Instance.ChangeLightPercentage(CalculatePercentage(currentLight, minimumLight, maxLight));

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            Time.timeScale = pauseMenu.activeSelf ? 0 : 1;
        }
    }

    private static float CalculatePercentage(float value, float min, float max)
    {
        return (value - min) * 100 / (max - min);
    }

    public void UpdateArduinoInput(int dial, int volt, int ldr)
    {
        Debug.Log(dial + volt + ldr);
        UpdateArduinoDialInput(InvertValue(Mathf.Clamp(dial, minLockPickValue, maxLockPickValue), maxLockPickValue));
        UpdateArduinoLDRInput(InvertValue(Mathf.Clamp(ldr, minimumLight, maxLight), maxLight));
    }

    private int InvertValue(int value, int maxValue)
    {
        return (value - maxValue) * -1;
    }

    public void UpdateArduinoDialInput(int dial)
    {
        currentLockPickValue = dial;
    }

    public void UpdateArduinoLDRInput(int ldr)
    {
        currentLight = ldr;
    }

    public void SetMaxLDR()
    {
        maxLight = currentLight;
    }

    public void SetMinLDR()
    {
        minimumLight = currentLight;
    }

    public void SetMaxDial()
    {
        maxLockPickValue = currentLockPickValue;
    }

    public void SetMinDial()
    {
        minLockPickValue = currentLockPickValue;
    }
}
