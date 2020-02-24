using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Stats = PlayerStats.Stats;

public class SleptPoorDebuff : Buffs
{

    void Start()
    {
        Stats.BonusXpMultiplier -= XPmodifier;
    }

    void FixedUpdate()
    {
        CooldownImage.fillAmount = 1 - currentDuration / maxDuration;
        currentDuration += Time.deltaTime * GameTime.Clock.TimeMultiplier;
        if (currentDuration >= maxDuration)
        {
            Destroy(gameObject);
        }

    }
    private void OnDestroy()
    {
        Stats.BonusXpMultiplier += XPmodifier;
    }
}
