using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int minimumLight = 0;
    public int maxLight = 1000;

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
        // StartCoroutine(ChangeLightPercentage());
    }

    public void UpdateArduinoInput(int dial, int volt, int ldr)
    {
        Debug.Log("dial: "+dial + "volt: "+volt + "ldr:" +  ldr);
    }

    IEnumerator ChangeLightPercentage()
    {
        int lightPercentage = 0;
        while (true)
        {
            lightPercentage += 5;
            if (lightPercentage > 100)
            {
                lightPercentage = 0;
            }
            PlayerLamp.Instance.ChangeLightPercentage(lightPercentage);
            Debug.Log("Light percentage: " + lightPercentage);
            yield return new WaitForSeconds(.2f);
        }
    }
}
