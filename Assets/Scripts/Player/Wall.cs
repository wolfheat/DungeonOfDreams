using System;
using System.Collections;
using UnityEngine;

public class Wall : MonoBehaviour
{

    int health = 5;

    public void Damage()
    {
        health--;
        if (health == 0)
            Shrink();
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


        //gameObject.SetActive(false);
    }
}
