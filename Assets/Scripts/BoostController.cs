using UnityEngine;

public class BoostController : MonoBehaviour
{

    [SerializeField] private float speedMultiplier = 3f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // players = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && other.isTrigger)
        {
            other.gameObject.GetComponent<PlayerController>().FORCE_SCALE *= speedMultiplier;
            other.gameObject.GetComponent<PlayerController>().VELOCITY_CLAMP *= speedMultiplier;
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player" && other.isTrigger)
        {
            other.gameObject.GetComponent<PlayerController>().FORCE_SCALE /= speedMultiplier;
            other.gameObject.GetComponent<PlayerController>().VELOCITY_CLAMP /= speedMultiplier;
        }

    }
}
