using System.Collections.Generic;
using UnityEngine;
public class HealthUIController : MonoBehaviour
{
    [SerializeField] GameObject heartsHolder;
    [SerializeField] UIHeart heartPrefab;

    public static HealthUIController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public float AnimationTime { get { return heartsList.Count > 0 ? heartsList[0].AnimationTime:0; }}

    private List<UIHeart> heartsList = new();
    // Start is called before the first frame update
    private void Start()
    {
        //Debug.Log("Initializing hearts Stats: "+Stats.Instance);
        InitiateHearts(Stats.Instance.Health,Stats.Instance.CurrentMaxHealth);
    }

    private void OnDisable()
    {
        Stats.HealthUpdate -= UpdateHearts;
    }
    
    private void OnEnable()
    {
        Stats.HealthUpdate += UpdateHearts;
    }

    private void UpdateHearts(int amt)
    {
        int max = Stats.Instance.CurrentMaxHealth;
        //Debug.Log("Max hearts is "+max+" currently "+heartsList.Count+ " hearts ");
        while (max > heartsList.Count)
        {
            heartsList.Add(Instantiate(heartPrefab, heartsHolder.transform));
            Debug.Log("Add one heart");
        }

        for (int i = 0; i < heartsList.Count; i++)
            heartsList[i].Show(i < amt);
    }

    private void RemoveAllHearts()
    {
        foreach(UIHeart child in heartsHolder.GetComponentsInChildren<UIHeart>())
            Destroy(child.gameObject);
    }

    private void InitiateHearts(int v,int activate = 10)
    {
        // Remove the ones present at start
        RemoveAllHearts();

        for (int i = 0; i < v; i++)
        {
            UIHeart heart = Instantiate(heartPrefab, heartsHolder.transform);
            heart.Show(i < activate);
            heartsList.Add(heart);
        }
    }

}
