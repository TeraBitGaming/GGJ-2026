using Unity.Burst.CompilerServices;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitController : MonoBehaviour
{
    [SerializeField]
    private string nextScene;
    private int touchingPlayerCount = 0;
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
        if (other.tag == "Player" && !other.isTrigger)
        {
            touchingPlayerCount++;
            if (touchingPlayerCount == 2)
            {
                SceneManager.LoadScene(nextScene);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player" && !other.isTrigger)
        {
            touchingPlayerCount--;
        }
    }
}
