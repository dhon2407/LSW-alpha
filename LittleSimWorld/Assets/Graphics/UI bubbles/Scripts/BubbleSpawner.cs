using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStats;
public class BubbleSpawner : MonoBehaviour
{
    public static BubbleSpawner Instance;
    public GameObject ForceFieldGO;
    public GameObject BubbleParticleSystemGO;
    public float Cooldown = 1;
    [SerializeField]
    public Dictionary<object, float> Cooldowns = new Dictionary<object, float>();
    // Start is called before the first frame update
    private void Awake()
    {
      
    }
    void Start()
    {
        if (Instance == null)
            Instance = this;
    }

    

    private void FixedUpdate()
    {
        if(Cooldowns.Count > 0)
        {
            List<object> temp = new List<object>(Cooldowns.Keys);
            foreach (object type in temp)
            {
                if (Cooldowns[type] > 0)
                {
                    Cooldowns[type] -= Time.deltaTime;
                }
            }
        }
       
    }
    /*public void SpawnBubble(Vector3 WhereToSpawnFrom)
    {
       

            //CurrentBubbleSpawnerGO = Instantiate(BubbleSpawnerGO, WhereToSpawnFrom, Quaternion.Euler(Vector3.zero));
            particlesystem.Play();
            //Destroy(CurrentBubbleSpawnerGO, MaximumLifetimeOfEmitter);
        
        
    }*/
    public void SpawnBubble(Transform whereToGravitateParent, Vector3 positionToSpawnFrom, Sprite sprite, LayerMask layerMask, Color colorOfParticle, object Object)
    {
        if (!Cooldowns.ContainsKey(Object))
        {
            Cooldowns.Add(Object, 0);
        }

      if(Cooldowns[Object] <= 0)
        {
            
            GameObject CurrentForceField = Instantiate(ForceFieldGO, whereToGravitateParent.position, Quaternion.Euler(Vector3.zero), whereToGravitateParent);
            ParticleSystem particlesystem = Instantiate(BubbleParticleSystemGO, positionToSpawnFrom, Quaternion.Euler(Vector3.zero), null).GetComponent<ParticleSystem>();
            Destroy(CurrentForceField, particlesystem.main.startLifetime.Evaluate(1));
            Destroy(particlesystem.gameObject, particlesystem.main.startLifetime.Evaluate(1));
            ParticleSystemForceField temp = CurrentForceField.GetComponent<ParticleSystemForceField>();
            particlesystem.transform.position = positionToSpawnFrom;
            particlesystem.externalForces.SetInfluence(0, temp);
            //particlesystem.textureSheetAnimation.SetSprite(0, sprite);   // use this if you want sprites as bubbles
            var particleMain = particlesystem.main;
            particleMain.startColor = colorOfParticle;
            var collision = particlesystem.collision;
            collision.collidesWith = layerMask;
            particlesystem.trigger.SetCollider(0, whereToGravitateParent.GetComponent<Collider>());
            particlesystem.Play();
            Cooldowns[Object] = Cooldown;
        }
    }
    /* private void OnParticleTrigger()
     {
         //Debug.Log("playing");
          if (!particlesystem.trigger.GetCollider(0).GetComponent<Animation>().isPlaying)
         {
             particlesystem.trigger.GetCollider(0).GetComponent<Animation>().Play();
             Debug.Log("Playing animation");
         }

     }*/
    private void OnParticleTrigger()
    {
        

    }

}
