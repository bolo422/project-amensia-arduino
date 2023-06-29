using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSoundManager : MonoBehaviour
{
    public void SetVolume(Slider slider)
    {
        AudioListener.volume = slider.value;
    }
}
