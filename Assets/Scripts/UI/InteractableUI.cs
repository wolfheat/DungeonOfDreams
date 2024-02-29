using System.Collections.Generic;
using UnityEngine;

public class InteractableUI : MonoBehaviour
{
    List<InteractableUIItem> items;
    [SerializeField] InteractableUIItem uiItemPrefab;

    [SerializeField] GameObject holder;
    [SerializeField] GameObject pickedHolder;


    public void AddItem(string name)
    {
        Instantiate(uiItemPrefab,holder.transform);
    }
    
    public void UpdateItems(List<string> names, bool resetList)
    {
        if (resetList)
            foreach (Transform child in holder.transform)
                Destroy(child.gameObject);

        foreach (string name in names)
        {
            //Debug.Log("  - UI Update - Adding Item "+name);
            InteractableUIItem item = Instantiate(uiItemPrefab, holder.transform);
            item.SetName(name);
        }
    }

    private List<InteractableUIItem> pickedUp = new();
    public void AddPickedUp(string name)
    {
        InteractableUIItem pickedUpItem = Instantiate(uiItemPrefab, pickedHolder.transform);
        pickedUpItem.SetName(name);
        StartCoroutine(pickedUpItem.StartRemoveTimer());
        pickedUp.Add(pickedUpItem);
        
    }

}
