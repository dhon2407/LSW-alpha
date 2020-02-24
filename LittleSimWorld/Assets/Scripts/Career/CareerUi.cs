using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameTime;
using Sirenix.OdinInspector;

using PlayerStats;
using Stats = PlayerStats.Stats;

public class CareerUi : SerializedMonoBehaviour
{
    public static CareerUi Instance;

    public TMPro.TextMeshProUGUI JobNameText;
    public TMPro.TextMeshProUGUI PaymentText;
    public TMPro.TextMeshProUGUI WorkingTimeText;
    public TMPro.TextMeshProUGUI WorkingDaysText;
    public TMPro.TextMeshProUGUI RequiredLevel;
    public TMPro.TextMeshProUGUI ProgressLevel;
    public Image JobIcon;
    [SerializeField]
    public Dictionary<JobType, Sprite> JobIconSprites;
    public Slider PerformanceSlider;

    public TMPro.TextMeshProUGUI CurrentEmplymentStatus;
    // Start is called before the first frame update
    void Start()
    {
        UpdateJobUi();
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void UpdateJobUi()
    {
        if (JobManager.Instance.CurrentJob != null)
        {
            Job job = JobManager.Instance.CurrentJob;
            JobNameText.text = job.name;

            PaymentText.text = "£" + job.WagePerHour + "/Hour";

            WorkingTimeText.text = System.TimeSpan.FromHours(job.JobStartingTimeInHours).Hours.ToString("00") + ":" + System.TimeSpan.FromHours(job.JobStartingTimeInHours).Minutes.ToString("00") +
               " - " +
               System.TimeSpan.FromHours(job.JobStartingTimeInHours + job.WorkingTimeInHours).Hours.ToString("00") + ":" +
              System.TimeSpan.FromHours( job.JobStartingTimeInHours + job.WorkingTimeInHours).Minutes.ToString("00");


            WorkingDaysText.text = "";
            string[] WeekdayNames = System.Enum.GetNames(typeof(Calendar.Weekday));
            string WorkingDaysNames = "";
            foreach (Calendar.Weekday weekday in job.WorkingDays)
            {
                WorkingDaysNames += weekday.ToString();
            }


            foreach (string name in WeekdayNames)
            {
                if (WorkingDaysNames.Contains(name))
                {
                    WorkingDaysText.text += name.ToString()[0] + " ";
                }
                else
                {
                    WorkingDaysText.text += "<color=#9A0000><s>" + name.ToString()[0] + "</color></s> ";
                }

            }

            PerformanceSlider.value = job.GetPerformanceLevel();

            JobIcon.sprite = JobIconSprites[job.jobType];

            RequiredLevel.text = "";
            ProgressLevel.text = "";

            if (Stats.Initialized)
            {
                foreach (Skill.Type type in job.RequiredSkills.Keys)
                {
                    RequiredLevel.text += "Level " + job.RequiredSkills[type].ToString() + " " + type.ToString() + "\n";
                }
                //+ string.Join(", ", job.RequiredSkills);
                foreach (Skill.Type type in job.RequiredSkills.Keys)
                {
                    ProgressLevel.text += Stats.Skill(type).CurrentLevel + "/" + job.RequiredSkills[type].ToString() + "\n";
                }
            }

            CurrentEmplymentStatus.text = job.name;
        }
        else
        {
           
            JobNameText.text = "Unemployed";

            PaymentText.text = "-";

            WorkingTimeText.text = "-";


            WorkingDaysText.text = "-";

            RequiredLevel.text = "";
            ProgressLevel.text = "-";

            CurrentEmplymentStatus.text = "Unemployed";

        }
      

   

    }
}
