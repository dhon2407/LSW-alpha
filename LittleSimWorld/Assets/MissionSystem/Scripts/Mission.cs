using UnityEngine;
using CharacterStats;
using UnityEngine.UI;
using System.Linq;
using Stats = PlayerStats.Stats;

/// <summary>
/// Holds the extra information needed for each mission
/// </summary>
public class Mission : MonoBehaviour
{
    /// <summary>
    /// Is this a mission that can be accomplished several times?
    /// </summary>
    [Tooltip("Can this mission be done several times?")]
    public bool recurring = false;

    public MissionHandler handler;

    public Button GetRewardButton;


    public GameObject nextMission;

    public int MoneyBonus;

    /// <summary>
    /// Has this mission been completed?
    /// </summary>
    public bool accomplished;
    private void Awake()
    {
        handler = FindObjectOfType<MissionHandler>();
    }
    private void Start()
    {
        GetRewardButton.gameObject.SetActive(false);
    }
    public void Accomplish()
    {
        if(accomplished == false)
        {
            accomplished = true;
            GetRewardButton.gameObject.SetActive(true);
            GetRewardButton.onClick.AddListener(GetReward);

            MissionHandler.missionHandler.activeMissions.Remove(MissionHandler.missionHandler.activeMissions.Where(obj => obj.name == name).FirstOrDefault());
            MissionHandler.missionHandler.CheckMissions();
           
            PlayerChatLog.Instance.AddChatMessege("You've completed a quest, collect the reward to get " + MoneyBonus + " £!");

        }

    }
    public void GetReward()
    {
        
       
        Stats.AddMoney(MoneyBonus);
        if (nextMission)
        {
            GameObject NextMission = Instantiate(nextMission, MissionHandler.missionHandler.MissionsGameObjects.transform);
            NextMission.name = nextMission.name;

            MissionHandler.CurrentMissions.Add(NextMission.name);
            MissionHandler.missionHandler.CheckMissions();


        }
        MissionHandler.missionHandler.RemoveAccomplished(this);

    }
}