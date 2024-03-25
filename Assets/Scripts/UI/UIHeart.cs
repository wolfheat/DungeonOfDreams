using UnityEngine;

public class UIHeart : MonoBehaviour
{
    [SerializeField] GameObject red;
    [SerializeField] GameObject grey;
    internal void Show(bool show)
    {
        red.SetActive(show);
    }
}
