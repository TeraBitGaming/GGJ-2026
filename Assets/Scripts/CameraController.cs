using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject[] players;
    // camera speed


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");


        
    }

    // Update is called once per frame
    void Update()
    {
        float sum = 0;
        int number = 0;
        foreach (GameObject player in players)
        {
            sum += player.transform.position.x;
            number++;
            
        }
        float averageX = sum / number;

        gameObject.transform.position = new Vector3(averageX, gameObject.transform.position.y, gameObject.transform.position.z);

        //get x of players
        //calc position

        //set camera position
        // delay?
        
    }
}
