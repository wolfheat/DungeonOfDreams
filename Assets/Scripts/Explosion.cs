using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;

	public static Explosion Instance { get; private set; }

	private void Start()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}
    int[][] explosionPositions = { new[] { -1, -1 }, new[] { -1, 0 }, new[] { -1, 1 }, new[] { 0, -1 }, new[] { 0, 0 }, new[] { 0, 1 }, new[] { 1, -1 }, new[] { 1, 0 }, new[] { 1, 1 } };
    int explosionDamage = 10;
    public void ExplodeNineAround(ParticleType particleType, Vector3 position)
    {
        // Make hurt and visual effects
        foreach (var p in explosionPositions)
        {
            Vector3 pos = position + new Vector3(p[0], 0, p[1]);

            // Particle Effects
            ParticleEffects.Instance.PlayTypeAt(particleType, pos);
            // Destroy Walls affected and hurt player or Enemy nearby
            Collider[] colliders = Physics.OverlapBox(pos, Game.boxSize, Quaternion.identity, layerMask);
            if(colliders.Length > 0)
            {
                foreach (var collider in colliders)
                {
                    if (collider.gameObject.TryGetComponent(out Wall wall))
                    {
                        Debug.Log("Bomb destroys wall at position "+pos+ " name: "+collider.name);
                        wall.Damage(explosionDamage);
                    }
                    else if(collider.gameObject.TryGetComponent(out PlayerColliderController playerColliderController))
                    {
                        Debug.Log("Bomb destroys player at position "+pos+ " name: "+collider.name);
                        playerColliderController.TakeDamage(explosionDamage,true);
                    }
                    else if(collider.gameObject.TryGetComponent(out EnemyColliderController enemyColliderController))
                    {
                        Debug.Log("Bomb destroys enemy at position "+pos+ " name: "+collider.name);
                        enemyColliderController.TakeDamage(explosionDamage, true);
                    }
                }
            }

        }
    }

    public void FireDamage(Vector3 pos)
    {        
        // Destroy Walls affected and hurt player or Enemy nearby
        Collider[] colliders = Physics.OverlapBox(pos, Game.boxSize, Quaternion.identity, layerMask);
        if(colliders.Length > 0)
        {
            foreach (var collider in colliders)
            {
                if (collider.gameObject.TryGetComponent(out Wall wall))
                {
                    Debug.Log("Fire destroys wall at position "+pos+ " name: "+collider.name);
                    wall.Damage(explosionDamage);
                }
                else if(collider.gameObject.TryGetComponent(out PlayerColliderController playerColliderController))
                {
                    Debug.Log("Fire set player on fire at "+pos+ " name: "+collider.name);    
                    //playerColliderController.TakeDamage(explosionDamage, true);
                    playerColliderController.SetOnFire();
                }
                else if(collider.gameObject.TryGetComponent(out EnemyColliderController enemyColliderController))
                {
                    Debug.Log("Fire destroys enemy at position "+pos+ " name: "+collider.name);
                    enemyColliderController.TakeDamage(explosionDamage, true);
                }
            }
        }
    }





}
