using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParallaxBackgroundController : MonoBehaviour
{

    [SerializeField]
    private GameObject backgroundImage1;
    private RawImage _rawImage1;

    [SerializeField]
    private GameObject backgroundImage2;
    private RawImage _rawImage2;

    [SerializeField]
    private GameObject backgroundImage3;
    private RawImage _rawImage3;

    [SerializeField]
    private GameObject backgroundImage4;
    private RawImage _rawImage4;

    [SerializeField]
    private float speed = 2;

    [SerializeField]
    private GameObject camera;

    private float currentScrollImage1;
    private float currentScrollImage2;
    private float currentScrollImage3;
    private float currentScrollImage4;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rawImage1 = backgroundImage1.GetComponent<RawImage>();
        _rawImage2 = backgroundImage2.GetComponent<RawImage>();
        _rawImage3 = backgroundImage3.GetComponent<RawImage>();
        _rawImage4 = backgroundImage4.GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        currentScrollImage1 = camera.transform.position.x * (speed / 10);
        // currentScrollImage1 += speed / 4 * Time.deltaTime;
        _rawImage1.uvRect = new Rect(currentScrollImage1, 0, 1, 1);

        currentScrollImage2 = camera.transform.position.x * (speed / 5);
        // currentScrollImage2 += speed / 2 * Time.deltaTime;
        _rawImage2.uvRect = new Rect(currentScrollImage2, 0, 1, 1);

        currentScrollImage3 = camera.transform.position.x * (speed / 4);
        // currentScrollImage3 += speed * Time.deltaTime;
        _rawImage3.uvRect = new Rect(currentScrollImage3, 0, 1, 1);

        currentScrollImage4 = camera.transform.position.x * (speed / 2);
        // currentScrollImage3 += speed * Time.deltaTime;
        _rawImage4.uvRect = new Rect(currentScrollImage4, 0, 1, 1);
    }
}
