using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameTime;
using CharacterStats;
using Sirenix.OdinInspector;
[System.Serializable]
public class FindCareerUi : MonoBehaviour
{
    public static CareerUi Instance;
    public Job FirstSelectedJob;
    public TMPro.TextMeshProUGUI CareerNameText;
    public TMPro.TextMeshProUGUI CareerDescriptionText;
    public TMPro.TextMeshProUGUI JobNameText;
    public TMPro.TextMeshProUGUI PaymentText;
    public TMPro.TextMeshProUGUI WorkingTimeText;
    public TMPro.TextMeshProUGUI WorkingDaysText;
    public Job SelectedJob;
    public UIPopUp anim;
    // Start is called before the first frame update

    private void Awake()
    {
        anim = GetComponent<UIPopUp>();
    }

    void Start()
    {
        UpdateSelectedJob(FirstSelectedJob);
    }

    // Update is called once per frame
    [SerializeField]
    
    public void UpdateSelectedJob(Job job)
    {
        CareerDescriptionText.text = job.Description;

        CareerNameText.text = job.jobType.ToString();

            JobNameText.text = "Now Hiring: "+ job.name;

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

        SelectedJob = job;

    }
    public void ApplySelectedJob()
    {
        SelectedJob.AssignToThisJob();
    }
}

