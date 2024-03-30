using UnityEngine;

public class MushRoom : MonoBehaviour
{
    internal void SetMaterial(Material material)
    {
        GetComponent<Renderer>().material = material;
    }
}
