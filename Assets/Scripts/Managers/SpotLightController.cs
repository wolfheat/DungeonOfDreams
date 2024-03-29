using UnityEngine;

public class SpotLightController : MonoBehaviour
{
    [SerializeField] Light spotLight;

    int postProcessingRoom;
    private void Start()
    {
        postProcessingRoom = LayerMask.NameToLayer("PostProcessingRoom");
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == postProcessingRoom)
            spotLight.enabled = false;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == postProcessingRoom)
            spotLight.enabled = true;            
    }

}
