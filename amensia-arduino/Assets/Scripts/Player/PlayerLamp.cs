using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Player
{
    public class PlayerLamp : MonoBehaviour
    {
        private Light2D light;
        [SerializeField] private float minimumLight = 1.5f;
        [SerializeField] private float maximumLight = 10f;
        private float currentLight;
        [SerializeField] private GameObject outerCircle;
        [SerializeField] private GameObject innerCircle;
        private float innerCircleFactor = 1f;
        private Coroutine lightTransitionCoroutine;
        [SerializeField] private Slider fuelSlider;
        private float maxOil = 100.0f;
        private float oil;
        private float oilConsumption;

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
            
            oil = maxOil;
            light = GetComponent<Light2D>();
        }

        void Start()
        {
            CurrentLight = minimumLight;
            oilConsumption = GameManager.Instance.OilConsumption;
            StartCoroutine(ConsumeOil());
        }
    
        // inputs between 0 and 100
        // public void ChangeLightPercentage(float lightPercentage) 
        // {
        //     CurrentLight = maximumLight * (lightPercentage / 100);
        // }

        public void ChangeLightPercentage(float lightPercentage, float transitionTime = 1f)
        {
            float targetLight = maximumLight * (lightPercentage / 100);
    
            // If there's an ongoing transition, stop it before starting a new one
            if (lightTransitionCoroutine != null)
            {
                StopCoroutine(lightTransitionCoroutine);
            }
    
            lightTransitionCoroutine = StartCoroutine(ChangeLightSmoothly(targetLight, transitionTime));
        }

        private IEnumerator ChangeLightSmoothly(float targetLight, float transitionTime)
        {
            float initialLight = CurrentLight;
            float elapsedTime = 0f;

            while (elapsedTime < transitionTime)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / transitionTime);
                CurrentLight = Mathf.Lerp(initialLight, targetLight, t);
                yield return null;
            }

            CurrentLight = targetLight;

            lightTransitionCoroutine = null;
        }

        private void Update()
        {
            fuelSlider.value = oil/100f;
        }

        private IEnumerator ConsumeOil()
        {
            var percentage = (CurrentLight - minimumLight) / ( maximumLight - minimumLight);
            
            if (percentage <= 0.15f) percentage = 0;

            oil -= oilConsumption * percentage;
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(ConsumeOil());
        }

        public void AddOil(float instanceOilRefuelQuanity)
        {
            oil += instanceOilRefuelQuanity;
            if (oil > maxOil) oil = maxOil;
        }
    }
}
