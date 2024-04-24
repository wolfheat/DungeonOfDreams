using System;
using Unity.VisualScripting;
using UnityEngine;
using Wolfheat.StartMenu;

public class WildFireSpawner : MonoBehaviour
{
    private Vector2Int position;
    private Vector2Int direction;

    private float stepTimer = 0;
    private const float StepTime = 0.1f;
    
        
        
    public void InitiateAt(Vector2Int pos, Vector2Int dir)
    {
        position = pos;
        direction = dir;
    }

    void Update()
    {
        stepTimer -= Time.deltaTime;
        if (stepTimer <= 0)
        {
            stepTimer += StepTime;
            Move();
        }

        Vector3 newPosition = transform.position+transform.forward* Time.deltaTime / StepTime;
        transform.position = newPosition;
    }

    private void Move()
    {
        position += direction;

        // check for Wall, if not create wildfire
        Vector3 alignedPos = Convert.V2IntToV3(position);
        transform.position = alignedPos;

        if (LevelCreator.Instance.TargetHasWall(alignedPos))
        {
            //Debug.Log("Wildfire can not be created at Wall");
            Destroy(gameObject);
            return;
        }


        // Legal Wildfire Position
        ParticleEffects.Instance.PlayTypeAt(ParticleType.WildFire, alignedPos+Vector3.down*0.48f);

        Explosion.Instance.FireDamage(transform.position);
        //SoundMaster.Instance.PlaySound(SoundName.FireSound);

    }
}
