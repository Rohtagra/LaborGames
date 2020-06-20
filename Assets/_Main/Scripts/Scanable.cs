using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanable : MonoBehaviour
{

    public Material[] material;

    private Renderer rend;

    public float scanDuration;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = true;
    }

    public void Scanned()
    {
        rend.sharedMaterial = material[1];
        //Debug.Log("Scanned");
        Invoke("Restore", scanDuration);
    }

    private void Restore()
    {
        rend.sharedMaterial = material[0];
    }


}
