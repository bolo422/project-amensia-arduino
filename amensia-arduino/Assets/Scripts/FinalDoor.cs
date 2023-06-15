using System;
using System.Collections.Generic;
using System.IO;
using Player;
using UnityEngine;

public class FinalDoor : MonoBehaviour
{

    private bool playerInsideRange;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInsideRange = true;
        if (gameManager.FinalDoorUnlocked)
            gameManager.InteractMessage = true;
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        gameManager.InteractMessage = false;
        playerInsideRange = false;
    }

    private void Update()
    {
        if (!playerInsideRange || !gameManager.FinalDoorUnlocked) return;
        
        // if player press E, go to next level
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Avançando para a próxima fase");
        }
    }
}
