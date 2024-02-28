using System.Collections;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    int health = 6;

    public void Damage()
    {
        health--;

        if (health == 0)
            Shrink();
        else if(meshRenderer != null)
        {
            Material[] materials = meshRenderer.materials;
            materials[1] = CrackMaster.Instance.GetCrack(health-1);
            meshRenderer.materials = materials;
        }
    }
    
    private void Shrink()
    {
        GetComponent<BoxCollider>().enabled = false;

        StartCoroutine(ShrinkRoutine());
    }

    private IEnumerator ShrinkRoutine()
    {
        float startSize = 1f;
        float endSize = 0.1f;

        float shrinkTimer = 0f;
        const float ShrinkTime = 0.15f;

        while (shrinkTimer < ShrinkTime)
        {
            transform.localScale = Vector3.Lerp(Vector3.one * startSize, Vector3.one * endSize,shrinkTimer/ShrinkTime);
            yield return null;
            shrinkTimer += Time.deltaTime;
        }
        transform.localScale = Vector3.one*endSize;

        CreateItem();
        
    }

    private void CreateItem()
    {
        gameObject.SetActive(false);
        ItemSpawner.Instance.SpawnMineralAt(MineralType.Chess,transform.position);
    }
}
