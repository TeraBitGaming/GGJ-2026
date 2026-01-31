using System;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour, ActivationSource
{
    int playerCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            playerCount++;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            playerCount--;
        }
    }
    public bool IsActive()
    {
        return playerCount > 0;
    }
}
