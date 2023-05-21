using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissiveTest : MonoBehaviour
{

    [SerializeField]
    private Material emissiveMaterial;
    [SerializeField]
    private Renderer LightObject;
    // Start is called before the first frame update
    void Start()
    {
        emissiveMaterial = LightObject.GetComponent<Renderer>().material;
    }
    
    public void TurnOff()
    {
        emissiveMaterial.DisableKeyword("_EMISSION");
    }

}
