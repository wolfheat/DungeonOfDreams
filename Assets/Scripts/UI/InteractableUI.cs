using System.Collections.Generic;
using UnityEngine;
using Wolfheat.StartMenu;

public class InteractableUI : MonoBehaviour
{
    List<InteractableUIItem> items;
    [SerializeField] InteractableUIItem uiItemPrefab;
    [SerializeField] BoostUIItem boostuiItemPrefab;

    [SerializeField] GameObject holder;
    [SerializeField] GameObject pickedHolder;
    [SerializeField] GameObject boostsHolder;

    [SerializeField] RectTransform pickedUpRect;

    private const float PickedScreenLowPosition = 0f;
    private const float PickedScreenHighPosition = -40f;
    private Vector2 pickedUpStartAnchoredPosition;

    private void Start()
    {
        pickedUpStartAnchoredPosition = pickedUpRect.anchoredPosition;
    }
    public void AddItem(string name)
    {
        Instantiate(uiItemPrefab,holder.transform);
    }
    
    public void UpdateItems(List<ItemData> itemDatas, bool resetList)
    {
        if (resetList)
            foreach (Transform child in holder.transform)
                Destroy(child.gameObject);

        foreach (var data in itemDatas)
        {
            if (data == null) continue;
            InteractableUIItem item = Instantiate(uiItemPrefab, holder.transform);
            //Debug.Log("data"+data+" resetList: "+resetList);
            item.SetName(data.itemName);
            item.SetSprite(data.sprite);
        }
    }
    
    private List<InteractableUIItem> pickedUp = new();
    public void AddPickedUp(ItemData data)
    {
        if (data is PowerUpData)
        {
            if (((PowerUpData)data).powerUpType == PowerUpType.Health)
            {
                Debug.Log("Adding health with heart " + data.value);
                Stats.Instance.AddHealth(data.value); // Dont add health to picked up list?
                SoundMaster.Instance.PlaySound(SoundName.MoreLifeNow);
                return;
            }

            // Check If boost is allready active and if so updat the timer
            BoostUIItem[] uiBoosts = boostsHolder.GetComponentsInChildren<BoostUIItem>();
            foreach (BoostUIItem item in uiBoosts)
            {
                if (item.nameString == data.itemName)
                {
                    item.AddBoost(data as PowerUpData);
                    return; // Dont add boosts to picked up list?
                }
            }

            Debug.Log("Picking Up never used Power Up: " + (data as PowerUpData).itemName);
            BoostUIItem boostItem = Instantiate(boostuiItemPrefab, boostsHolder.transform);
            boostItem.SetName(data.itemName);
            boostItem.SetSprite(data.sprite);
            boostItem.AddBoost(data as PowerUpData);
            SoundMaster.Instance.PlaySound(SoundName.Energize);
            return;
        }
        else if (data is UsableData)
        {
            if (((UsableData)data).usableType == UsableType.Bomb)
            {
                Debug.Log("Adding bomb to inventory " + data.value);
                Stats.Instance.AddBomb(data.value);
            }else if (((UsableData)data).usableType == UsableType.SledgeHammer)
            {
                Debug.Log("Adding sledgehammer " + data.value);
                SoundMaster.Instance.PlaySound(SoundName.INowHaveASledgehammer);

                Stats.Instance.ActivateSledgeHammer();
            }
            else if (((UsableData)data).usableType == UsableType.Compass)
            {
                Debug.Log("Adding compass " + data.value);
                SoundMaster.Instance.PlaySound(SoundName.IHaveACompass);
                Stats.Instance.ActivateCompass();
                //Stats.Instance.AddBomb(data.value);
            }
        }

        InteractableUIItem pickedUpItem = Instantiate(uiItemPrefab, pickedHolder.transform);
        pickedUpItem.SetName(data.itemName);
        pickedUpItem.SetSprite(data.sprite);
        StartCoroutine(pickedUpItem.StartRemoveTimer());
        pickedUp.Add(pickedUpItem);
        
    } 
    
    public void PositionPickedUpMenu(bool low)
    {
        pickedUpRect.anchoredPosition = new Vector2() { x = pickedUpStartAnchoredPosition.x, y = pickedUpStartAnchoredPosition.y + (low ?  PickedScreenLowPosition: PickedScreenHighPosition) };
    }

}
