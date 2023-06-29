using System;
using System.Collections.Generic;
using System.IO;
using Player;
using UnityEngine;

public class FinalDoor : MonoBehaviour
{

    private bool playerInsideRange;
    private GameManager gameManager;
    [SerializeField] private int level;
    public int Level => level;
    [SerializeField] private List<SpriteRenderer> sprites;

    private void Start()
    {
        gameManager = GameManager.Instance;

        var levelColor = gameManager.GetLevelColor(level);
        foreach (var sprite in sprites)
        {
            sprite.color = levelColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInsideRange = true;
        if (gameManager.FinalDoorUnlocked(level))
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
        if (!playerInsideRange || !gameManager.FinalDoorUnlocked(level)) return;
        
        // if player press E, go to next level
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Avançando para a próxima fase");
            gameManager.NextLevel(this);
        }
    }
}
