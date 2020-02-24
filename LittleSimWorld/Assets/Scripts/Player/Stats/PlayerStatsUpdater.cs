using System.Collections.Generic;
using UnityEngine;
using PlayerStatus = System.Collections.Generic.Dictionary<PlayerStats.Status.Type, PlayerStats.Status>;
using Random = System.Random;

namespace PlayerStats
{
    public class PlayerStatsUpdater
    {
        float CooldownTime = 60;
        bool LastBubbleCD;
        float randomWarningFloat = 0;
        int LowStatsCount;
        bool StatWarningInitialized;
        Dictionary<Status.Type, float> statusCooldown = new Dictionary<Status.Type, float>();

        public void Update(PlayerStatus playerStats, float timeScale)
        {
            if (!StatWarningInitialized)
                InitializeStatWarnings(playerStats);
            LowStatsCount = 0;
            bool notPassOutAndActive = !GameLibOfMethods.passedOut && Player.gameObject.activeSelf;

            foreach (var item in playerStats)
            {
                var stat = item.Value;
                if (stat.CurrentAmount > 0)
                    stat.Drain(timeScale);

                if (randomWarningFloat == 0)
                {
                    ResetRandomFloat();
                }

                if (stat.CurrentAmount <= stat.MaxAmount * randomWarningFloat)
                {
                    if (!JobManager.Instance.isWorking)
                    {
                        LowStatsCount++;
                        StatWarningBubble(item.Key);
                        if (statusCooldown.ContainsKey(item.Key))
                            statusCooldown[item.Key] -= Time.deltaTime;
                    }
                    PlayerStatsManager.statWarning[stat] = true;
                }
                else PlayerStatsManager.statWarning[stat] = false;

                if (stat.CurrentAmount <= 0 && notPassOutAndActive)
                    stat.ZeroPenalty(timeScale);
            }
        }

        private void StatWarningBubble(Status.Type status)
        {
            if (ThoughtBubbleSpawner.Instance.LastBubbleCD)
                return;

            if (statusCooldown.ContainsKey(status))
            {
                if (statusCooldown[status] > 0)
                    return;
                statusCooldown[status] = CooldownTime;
                ResetRandomFloat();
            }
            else statusCooldown.Add(status, CooldownTime);

            int warningSprite = -1; ;
            switch (status)
            {
                case Status.Type.Health:
                    warningSprite = 0;
                    break;
                case Status.Type.Energy:
                    warningSprite = 1;
                    break;
                case Status.Type.Mood:
                    warningSprite = 2;
                    break;
                case Status.Type.Hygiene:
                    warningSprite = 3;
                    break;
                case Status.Type.Bladder:
                    warningSprite = 4;
                    break;
                case Status.Type.Thirst:
                    warningSprite = 5;
                    break;
                case Status.Type.Hunger:
                    warningSprite = 6;
                    break;
                
            }
            ThoughtBubbleSpawner.Instance.SpawnBubble(Player.gameObject.transform, Player.gameObject.transform.position, status, null, warningSprite);
        }

        private void InitializeStatWarnings(PlayerStatus playerStats)
        {
            foreach (var item in playerStats)
            {
                PlayerStatsManager.statWarning.Add(item.Value, false);
            }
            StatWarningInitialized = true;
        }

        private void ResetRandomFloat()
        {
            Random rnd = new Random();
            randomWarningFloat = (float)rnd.NextDouble() * (.3f - .2f) + .2f;
        }
    }
}