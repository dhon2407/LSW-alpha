using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerStats.Status;
using Stats = PlayerStats.Stats;

public class VeryPoorHealthDebuff : Buffs
{
    public static VeryPoorHealthDebuff veryPoorHealthDebuffInstance;
    // Start is called before the first frame update
    void Start()
    {
        maxDuration = 999999;
        Stats.MoveSpeed -= MovementSpeedModifier;
        veryPoorHealthDebuffInstance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Stats.Status(Type.Health).CurrentAmount / Stats.Status(Type.Health).MaxAmount > MinHealthPercentageForVeryPoorHealth)
        {
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        Stats.MoveSpeed += MovementSpeedModifier;
    }
}
