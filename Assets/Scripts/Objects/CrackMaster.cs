using UnityEngine;

public class CrackMaster : MonoBehaviour
{
    [SerializeField] Material[] materials;

    public static CrackMaster Instance { get; private set; }

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public Material GetCrack(int id)
    {
        return materials[id];
    }
}