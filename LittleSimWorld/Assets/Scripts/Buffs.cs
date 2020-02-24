using UnityEngine;
using UnityEngine.UI;

using static PlayerStats.Status;
using Stats = PlayerStats.Stats;

public class Buffs : MonoBehaviour
{
    public float XPmodifier = 0;
    public float MovementSpeedModifier = 0;
    public float maxDuration = 10;
    public float currentDuration = 0;
    public Image CooldownImage;
    public GameObject poorHealthDebuff;
    public GameObject veryPoorHealthDebuff;
    public float MinHealthPercentageForPoorHealth = 0.5f;
    public float MinHealthPercentageForVeryPoorHealth = 0.25f;

    public static GameObject Instance;

    private void Awake()
    {
        Instance = gameObject;
    }
    void Start()
    {
        
    }

    void Update()
    {
        Instance = gameObject;
        if (Stats.Status(Type.Health).CurrentAmount / Stats.Status(Type.Health).MaxAmount < MinHealthPercentageForPoorHealth && !PoorHealthDebuff.poorHealthDebuffInstance && !VeryPoorHealthDebuff.veryPoorHealthDebuffInstance)
        {
           Instantiate(poorHealthDebuff, Instance.transform);
        }
        if (Stats.Status(Type.Health).CurrentAmount / Stats.Status(Type.Health).MaxAmount < MinHealthPercentageForVeryPoorHealth && !VeryPoorHealthDebuff.veryPoorHealthDebuffInstance)
        {
            Instantiate(veryPoorHealthDebuff, Instance.transform);
        }
    }
}
