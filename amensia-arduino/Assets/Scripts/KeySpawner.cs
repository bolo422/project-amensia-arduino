using System.Collections;
using System.Collections.Generic;
using Consumables;
using UnityEngine;

public class KeySpawner : MonoBehaviour
{
    [SerializeField] private int level;
    public int Level => level;

    public void DestroyKey()
    {
        var key = GetComponentInChildren<Key>();
        if(key != null)
            Destroy(key.gameObject);
    }

    public void SpawnKey()
    {
        Instantiate(Resources.Load("Key"), transform);
    }
}
