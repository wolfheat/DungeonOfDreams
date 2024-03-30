using UnityEngine;

public class Pebble : MonoBehaviour
{
    internal void SetMaterial(Material pebbleMaterial)
    {
        GetComponent<Renderer>().material = pebbleMaterial;
    }
}
