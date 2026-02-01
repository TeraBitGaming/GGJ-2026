using UnityEngine;
using UnityEngine.SceneManagement;

public class IntrosceneManager : MonoBehaviour
{
    [SerializeField]
    private Scene sceneToGoTo;

    [SerializeField]
    private SpriteRenderer maksyMask;

    private bool soundPlayed = false;
    private bool soundCanBePlayed = false;

    void Start()
    {
        AudioManager.Instance.playSound("Intro");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
