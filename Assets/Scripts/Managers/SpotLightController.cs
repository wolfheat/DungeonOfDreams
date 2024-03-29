using UnityEngine;

public class SpotLightController : MonoBehaviour
{
    [SerializeField] Light spotLight;


    private void OnTriggerEnter(Collider other)
    {
        spotLight.enabled = false;
    }
    
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Enable SPOTLIGHT");
        spotLight.enabled = true;
    }

}
