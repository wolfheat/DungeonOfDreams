using System.Collections;
using UnityEngine;

public class Wall : Interactable
{
    new public WallData Data { get { return base.Data as WallData; } set { } }

    [SerializeField] MeshRenderer meshRenderer;
    int health = 6;
    private Coroutine shock;

    public int Health { get { return health;} }
    public bool Damage()
    {
        health--;

        if (health == 0)
            Shrink();
        else if(meshRenderer != null)
        {
            Material[] materials = meshRenderer.materials;
            if(materials.Length==1)
                materials = new Material[2] {materials[0],null};
            materials[1] = CrackMaster.Instance.GetCrack(health-1);
            meshRenderer.materials = materials;
            Hit();
        }
        return health == 0;
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

        if(Data.mineralStored != null)
            CreateItem(Data.mineralStored);
        
    }

    public void Hit()
    {
        //Debug.Log("HIT ");
        if (shock != null)
        {
            transform.position = startPosition;
            StopCoroutine(shock);
        }
        shock = StartCoroutine(SineShock());
    }

    Vector3 startPosition;
    private IEnumerator SineShock()
    {

        float shockTimer = 0;
        const float ShockTime = 0.35f;
        //Debug.Log("Wall position starts at " + startPosition);
        startPosition = transform.position;
        float speed = 70f;
        float amplitude = 0.01f;
        //float amplitude = 0.05f;
        const float Dampening = 0.8f;
        float dampening;

        bool xshake = false;
        if(Mathf.Abs(PlayerController.Instance.transform.position.x - transform.position.x)<0.5f)
            xshake = true;

        while (shockTimer < ShockTime)
        {
            dampening = 1 - (shockTimer / ShockTime)*Dampening;
            float shakePos = Mathf.Sin(shockTimer*speed)*amplitude*dampening;            
            Vector3 newPos = startPosition + (xshake?Vector3.right:Vector3.forward) * shakePos;
            transform.position = newPos;

            yield return null;
            shockTimer += Time.deltaTime;
        }
        //Debug.Log("Reset Wall position to "+startPosition);
        transform.position = startPosition;
        shock = null;
    }
    
    private IEnumerator Shock()
    {

        float shockTimer = 0;
        const float ShockTime = 0.25f;
        Vector3 startPosition = transform.position;
        Vector3 oldPosition = transform.position;

        float animationAcceleration = 20.00f;
        float animationVelocity = -0.4f;
        float dampening = 0.7f;
        int dir;

        bool xshake = false;
        if(Mathf.Abs(PlayerController.Instance.transform.position.x - transform.position.x)<0.5f)
            xshake = true;

        while (shockTimer < ShockTime)
        {
            dir = xshake?((oldPosition.x > startPosition.x) ? -1 : 1) : ((oldPosition.z > startPosition.z) ? -1 : 1);
            animationVelocity += animationAcceleration * dir * Time.deltaTime;
            animationVelocity *= Mathf.Pow(dampening,Time.deltaTime);
            Vector3 newPos = xshake?  new Vector3(oldPosition.x + animationVelocity * Time.deltaTime, oldPosition.y, oldPosition.z)
                                    : new Vector3(oldPosition.x, oldPosition.y, oldPosition.z + animationVelocity * Time.deltaTime);
            transform.position = newPos;
            oldPosition = newPos;
            yield return null;
            shockTimer += Time.deltaTime;
        }
        transform.position = startPosition;
        shock = null;
    }

    private void CreateItem(MineralData mineralData)
    {
        gameObject.SetActive(false);
        ItemSpawner.Instance.SpawnMineralAt(mineralData,transform.position);
    }
}
