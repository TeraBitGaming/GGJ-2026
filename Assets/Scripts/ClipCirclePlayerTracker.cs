using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class ClipCirclePlayerTracker : MonoBehaviour
{
    [SerializeField]
    float trackPlayerOne = 1;
    [SerializeField]
    float trackPlayerTwo = 1;
    GameObject p1;
    GameObject p2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (p.GetComponent<PlayerController>().secondPlayer)
            {
                p1 = p;
            } else
            {
                p2 = p;
            }
        }
        transform.Find("ToggleTilemap").GetComponent<TilemapRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
    }

    // Update is called once per frame
    void Update()
    {
        float weight = trackPlayerOne + trackPlayerTwo;
        if (Math.Abs(weight) > 0.01)
        {
            Vector3 weightedPosition = trackPlayerOne * p1.transform.position + trackPlayerTwo * p2.transform.position;
            transform.Find("MaskCircle").transform.position = weightedPosition;
        }
    }
}
