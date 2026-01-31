using UnityEngine;

public class DoorController : MonoBehaviour
{
    ActivationTarget t;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        t = gameObject.GetComponent<ActivationTarget>();
    }

    // Update is called once per frame
    void Update()
    {
        if (t.objectIsActive)
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.black;
            gameObject.GetComponent<Collider2D>().enabled = false;
        } else
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            gameObject.GetComponent<Collider2D>().enabled = true;
        }
    }
}
