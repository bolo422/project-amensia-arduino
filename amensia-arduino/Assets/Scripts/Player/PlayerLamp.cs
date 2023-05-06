using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLamp : MonoBehaviour
{
    private Light2D light;
    [SerializeField] private float minimumLight = 1.5f;
    [SerializeField] private float maximumLight = 10f;
    private float currentLight;
    [SerializeField] private GameObject outerCircle;
    [SerializeField] private GameObject innerCircle;
    private float innerCircleFactor = .7f;

    public static PlayerLamp Instance { get; private set; }
    
    public float CurrentLight
    {
        get => currentLight;
        set
        {
            currentLight = Mathf.Clamp(value, minimumLight, maximumLight);
            light.pointLightOuterRadius = currentLight;
            outerCircle.transform.localScale = new Vector3(currentLight*2, currentLight*2, 1);
            innerCircle.transform.localScale = new Vector3(currentLight*2 * innerCircleFactor, currentLight*2 * innerCircleFactor, 1);
        }
    }
    
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
        
        light = GetComponent<Light2D>();
    }

    void Start()
    {
        CurrentLight = minimumLight;
    }
    
    // inputs between 0 and 100
    public void ChangeLightPercentage(float lightPercentage) 
    {
        CurrentLight = maximumLight * (lightPercentage / 100);
    }

}
