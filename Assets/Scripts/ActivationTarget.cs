using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActivationTarget : MonoBehaviour
{
    public bool objectIsActive;

    [SerializeField]
    private List<GameObject> sources;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        objectIsActive = (sources.Any((s) => s.GetComponent<ActivationSource>().IsActive()));
    }
}
