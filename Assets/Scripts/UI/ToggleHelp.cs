using UnityEngine;

public class ToggleHelp : MonoBehaviour
{
    [SerializeField] private GameObject panelToToggle;
    [SerializeField] private InteractableUI interactableUI;
    
    public void Toggle()
    {
        Debug.Log("Toggle Help");
        panelToToggle.SetActive(!panelToToggle.gameObject.activeSelf);

        interactableUI.PositionPickedUpMenu(panelToToggle.activeSelf);
    }
}
