using UnityEngine;

public class Root : MonoBehaviour
{
    internal void SetMaterial(Material material)
    {
        GetComponent<Renderer>().material = material;
    }
}
