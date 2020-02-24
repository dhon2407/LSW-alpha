using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameTime;
using Sirenix.OdinInspector;
using UnityEngine.Events;
[System.Serializable]
public class JobOfferUi : SerializedMonoBehaviour
{
    public static JobOfferUi Instance;

    public TMPro.TextMeshProUGUI JobNameText;
    public TMPro.TextMeshProUGUI PaymentText;
    public TMPro.TextMeshProUGUI WorkingTimeText;
    public TMPro.TextMeshProUGUI WorkingDaysText;
    public Button AcceptButton;
    public UIPopUp anim;
    

    private void Awake()
    {
       if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    void Start()
    {
       
    }


    public void UpdateJobUi(Job job)
    {
        JobNameText.text = job.name;

        PaymentText.text = "£" + job.WagePerHour + "/Hour";

        WorkingTimeText.text = System.TimeSpan.FromHours(job.JobStartingTimeInHours).Hours.ToString("00") + ":" + System.TimeSpan.FromHours(job.JobStartingTimeInHours).Minutes.ToString("00") +
           " - " +
           System.TimeSpan.FromHours(job.JobStartingTimeInHours + job.WorkingTimeInHours).Hours.ToString("00") + ":" +
          System.TimeSpan.FromHours(job.JobStartingTimeInHours + job.WorkingTimeInHours).Minutes.ToString("00");


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
                WorkingDaysText.text += "<color=#9A0000>" + name.ToString()[0] + "</color> ";
            }

        }

        AcceptButton.onClick.RemoveAllListeners();
        AcceptButton.onClick.AddListener(delegate { JobManager.Instance.AssignToJob(job.jobType); });

      

        CareerUi.Instance.UpdateJobUi();
    }
    public void ShowJobOffer(int jobTypeIndex)
    {
        UpdateJobUi(JobManager.Instance.Jobs[(JobType)jobTypeIndex]);
        anim.Open();
        
    }
}
