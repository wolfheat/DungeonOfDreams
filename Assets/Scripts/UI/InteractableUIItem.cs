using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractableUIItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI shownName;
    public string nameString;
    [SerializeField] Image image;

    public void SetName(string newName)
    {
        nameString = newName;
        UpdateName(newName);
    }
    
    public void UpdateName(string fullString)
    {
        shownName.text = fullString;
    }

    public IEnumerator StartRemoveTimer()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    internal void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }
}
