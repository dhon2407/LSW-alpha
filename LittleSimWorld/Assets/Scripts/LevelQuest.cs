using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static PlayerStats.Skill;
using Stats = PlayerStats.Stats;

public class LevelQuest : Mission

     
{
    public int RequieredStrLevel = 2;
    public int RequiredIntLevel = 2;
    public int RequiredFitLevel = 2;
    public int RequiredChrLevel = 2;
    public int RequiredCookingLevel = 2;
    public int RequiredRepairLevel = 2;

    // Start is called before the first frame update
    private void Start()
    {
        GetRewardButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Stats.Skill(Type.Strength).CurrentLevel >= RequieredStrLevel ||
            Stats.Skill(Type.Intelligence).CurrentLevel >= RequiredIntLevel ||
            Stats.Skill(Type.Fitness).CurrentLevel >= RequiredFitLevel ||
            Stats.Skill(Type.Charisma).CurrentLevel >= RequiredChrLevel||
            Stats.Skill(Type.Cooking).CurrentLevel >= RequiredCookingLevel||
            Stats.Skill(Type.Repair).CurrentLevel >= RequiredRepairLevel)

        {
            if (accomplished == false)
            {
              
                    

                Accomplish();
                

               
               
               
            }
        }
        
    }
}
