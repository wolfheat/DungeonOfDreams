using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrystalBarFinderUI : MonoBehaviour
{
    [SerializeField] Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        SetSlider();    
    }
    private void OnEnable()
    {
        Stats.MineralsAmountUpdate += SetSlider;
    }
    private void OnDisable()
    {
        Stats.MineralsAmountUpdate -= SetSlider;
    }

    private void SetSlider()
    {
        slider.value = Stats.Instance.Minerals/100f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
