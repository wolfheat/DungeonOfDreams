using UnityEngine;

public class Compass : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] RectTransform compassImage;
    
    void Update()
    {
        float angle = player.rotation.eulerAngles.y;
        float percent = angle / 360f;
        float compassPosition = Mathf.Lerp(512, -512, percent);
        compassImage.anchoredPosition = new Vector2(compassPosition,0);           
    }
}
