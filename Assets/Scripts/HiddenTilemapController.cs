using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class HiddenTilemapController : MonoBehaviour
{
    bool isVisible = true;
    InputAction toggleVisionAction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        toggleVisionAction = InputSystem.actions.FindAction("ToggleVision");
    }

    // Update is called once per frame
    void Update()
    {
        if (toggleVisionAction.triggered)
        {
            isVisible = !isVisible;
            // gameObject.GetComponent<TilemapCollider2D>().enabled = isVisible;
            // gameObject.GetComponent<TilemapRenderer>().enabled = isVisible; 
        }
    }
}
