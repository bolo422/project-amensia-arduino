using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    
    // Make character movement with WASD and joystick using character controller
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical);

        controller.Move(direction * Time.deltaTime * 5f);
    }


}
