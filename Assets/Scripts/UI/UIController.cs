using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] InteractableUI interactableUI;

	public static UIController Instance { get; private set; }

	private void Start()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}


	public void UpdateShownItemsUI(List<string> names)
	{
		interactableUI.UpdateItems(names);
	}
	
	public void AddPickedUp(string name)
	{
		interactableUI.AddPickedUp(name);
	}




}
