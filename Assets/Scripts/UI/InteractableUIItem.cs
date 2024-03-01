using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractableUIItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI shownName;
    [SerializeField] Image image;

    public void SetName(string newName)
    {
        shownName.text = newName;
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
