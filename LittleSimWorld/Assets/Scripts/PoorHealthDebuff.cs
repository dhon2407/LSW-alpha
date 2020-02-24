using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerStats.Status;
using Stats = PlayerStats.Stats;

public class PoorHealthDebuff : Buffs
{
    public static PoorHealthDebuff poorHealthDebuffInstance;
    // Start is called before the first frame update
    void Start()
    {
        maxDuration = 999999;
        Stats.MoveSpeed -= MovementSpeedModifier;
        poorHealthDebuffInstance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(Stats.Status(Type.Health).CurrentAmount / Stats.Status(Type.Health).MaxAmount > MinHealthPercentageForPoorHealth || VeryPoorHealthDebuff.veryPoorHealthDebuffInstance)
        {
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        Stats.MoveSpeed += MovementSpeedModifier;
    }
}
