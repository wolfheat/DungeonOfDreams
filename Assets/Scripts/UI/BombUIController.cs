using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BombUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI amountText;

    public static BombUIController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UpdateBombs(Stats.Instance.Bombs);
    }

    private void OnDisable()
    {
        Stats.BombUpdate -= UpdateBombs;
    }

    private void OnEnable()
    {
        Stats.BombUpdate += UpdateBombs;
    }

    public void UpdateBombs(int amt)
    {
        Debug.Log("Updating amount of bombs shown to: " + amt);
        amountText.text = amt.ToString();
    }

}
