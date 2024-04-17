using UnityEngine;
using UnityEngine.UI;

public class CrystalBarFinderUI : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Animator sliderHandleAnimator;

    // Start is called before the first frame update
    void Start()
    {
        SetSlider();    
    }
    private void OnEnable()
    {
        Stats.MineralsAmountUpdate += SetSlider;
        Stats.NoMoreMineralsReached += DeactivateSlider;
    }
    private void OnDisable()
    {
        Stats.MineralsAmountUpdate -= SetSlider;
        Stats.NoMoreMineralsReached -= DeactivateSlider;
    }

    private void SetSlider()
    {
        slider.value = ((float)Stats.Instance.Minerals)/ Stats.MineralsToGetSeeThrough;
        sliderHandleAnimator.CrossFade("FlashBig",0.1f);

    }
    
    private void DeactivateSlider()
    {
        slider.gameObject.SetActive(false);
        //slider.value = 1;
        //slider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
