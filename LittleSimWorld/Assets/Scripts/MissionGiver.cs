using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionGiver : MonoBehaviour
{
    public List<GameObject> MissionsToGive;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
           foreach(GameObject mission in MissionsToGive)
            {
                if(MissionHandler.CompletedMissions!= null && MissionHandler.CompletedMissions.Count > 0)
                foreach(string name in MissionHandler.CompletedMissions)
                {
                    if (mission.name == name)
                        return;
                }
                foreach (string name in MissionHandler.CurrentMissions)
                {
                    if (mission.name == name)
                        return;
                }
                var temp = Instantiate(Resources.Load<GameObject>("Missions/" + mission.name), MissionHandler.missionHandler.MissionsGameObjects.transform);
                temp.name = mission.name;
                MissionHandler.CurrentMissions.Add(mission.name);
                MissionHandler.missionHandler.CheckMissions();
                Destroy(this);
               
            }
           
        }
    }
}
