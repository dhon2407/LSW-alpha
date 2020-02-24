using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CharacterStats;
using UnityEditor;
using System.Linq;
using GameTime;
using GameClock = GameTime.Clock;
using CharacterData;
using Sirenix.OdinInspector;
using UI.Notifications;

[System.Serializable]
public class JobManager : SerializedMonoBehaviour

{
    public float currentJobProgres = 0;
    public float currentJobTime = 0;
    public float requiredJobTime = 10;

    [Space]


    public Job CurrentJob;
    public Dictionary<JobType, Job> WorkedJobs = new Dictionary<JobType, Job>();

    public static JobManager Instance;
    public bool isWorking;
    public float CurrentWorkingTime = 0;
    public static JobData JobData { get => Instance.GetCurrentJobData(); set => Instance.InitializeCurrentJob(value); }
    [SerializeField]
    public Dictionary<JobType, Job> Jobs = new Dictionary<JobType, Job>()
    {
         //{JobType.Cooking, AssistantDishwasher.Instance }
    };

    private bool _jobNotificationPosted;


    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }
    private void Start()
    {
        Clock.onDayPassed.AddListener(ChangeWorkedToday);
        Clock.onDayPassed.AddListener(ResetJobReminderNotification);
    }
    void ChangeWorkedToday()
    {
        if (JobManager.Instance.CurrentJob != null)
            JobManager.Instance.CurrentJob.WorkedToday = false;
    }
    public void FinishJobCycle(JobType job)
    {
        Jobs[job].Finish();
    }
    public void AssignToJob(JobType job)
    {
        if (CurrentJob == null || (CurrentJob != null && CurrentJob.name != Jobs[job].name))
            Jobs[job].AssignToThisJob();
        CareerUi.Instance.UpdateJobUi();
    }
    public void AssignToJob(Job job)
    {
        if (CurrentJob == null || (CurrentJob != null && CurrentJob.name != job.name))
            job.AssignToThisJob();
        CareerUi.Instance.UpdateJobUi();
    }

    public JobData GetCurrentJobData()
    {
        return new JobData
        {
            type = (CurrentJob != null) ? CurrentJob.jobType : JobType.Unemployed,
            joblevel = (CurrentJob != null) ? CurrentJob.JobCareerLevel : 0,
        };
    }

    public void InitializeCurrentJob(JobData data)
    {
        if (data.joblevel > 0 && Jobs.ContainsKey(data.type))
            UpdateCurrentJob(data);
        else if (data.joblevel > 0)
            Debug.LogWarning($"Job type {data.type} currently not suporrted.");

    }

    public void QuitJob()
    {
        if (CurrentJob != null)
        {
            CurrentJob.Penalize(true);
            WorkedJobs[CurrentJob.jobType] = CurrentJob;

            PlayerChatLog.Instance.AddChatMessege("You quit your job: " + CurrentJob.name);
            Notify.Show("You quit your job: " + CurrentJob.name, CurrentJob.icon);
        }
        else
        {
            PlayerChatLog.Instance.AddChatMessege("You don't have a job to quit");
        }
        CurrentJob = null;
        CareerUi.Instance.UpdateJobUi();
    }

    private void UpdateCurrentJob(JobData data)
    {
        Job newJob = Jobs[data.type];
        while (newJob.JobCareerLevel != data.joblevel)
            newJob = newJob.PromotionJob;

        CurrentJob = newJob;

        CareerUi.Instance.UpdateJobUi();
    }

    private void Update()
    {
        
        if (CurrentJob != null &&
            GameClock.Time >= (System.TimeSpan.FromHours( CurrentJob.JobStartingTimeInHours).TotalSeconds - System.TimeSpan.FromMinutes(15).TotalSeconds) &&
            !CurrentJob.WorkedToday
            && GameClock.Time < System.TimeSpan.FromHours(CurrentJob.JobStartingTimeInHours).TotalSeconds + System.TimeSpan.FromMinutes(30).TotalSeconds &&
            CurrentJob.WorkingDays.Contains(Calendar.CurrentWeekday))
        {
            JobCar.Instance.CarToPlayerHouse();
            Notify.Show($"Job car has arrived.", CurrentJob.icon);
            CurrentJob.WorkedToday = true;
        }

        CheckJobNotification();
    }

    private void CheckJobNotification()
    {
        if (!_jobNotificationPosted &&
            CurrentJob != null &&
            CurrentJob.WorkingDays.Contains(Calendar.CurrentWeekday) &&
            GameClock.Time >= (System.TimeSpan.FromHours(CurrentJob.JobStartingTimeInHours).TotalSeconds -
                               System.TimeSpan.FromMinutes(60).TotalSeconds))
        {
            Notify.Show($"You'll start your {CurrentJob.name} job in {60/60} hour, a car will pick you up in front of your house. Please don't be late.", CurrentJob.icon);
            _jobNotificationPosted = true;
        }
    }

    private void ResetJobReminderNotification()
    {
        _jobNotificationPosted = false;
    }

  
    
    /*
    [ShowInInspector]
    [System.Serializable]
     public class AssistantDishwasher : JobManager.Job
     {
        override public List<int> RequiredSkillsLevelsForPromotion { get; set; } = new List<int> { 2 };
        new public int JobCareerLevel = 1;
        new public static JobManager.Job Instance = new AssistantDishwasher();
        new public float DefaultMoneyAtTheEndOfWorkingDay = 45;
        override public List<SkillType> RequiredSkills { get { return RequiredSkills = new List<SkillType>() { { SkillType.Cooking } }; } }
         new public JobType jobType = JobType.Cooking;

        override public string JobName{ get { return JobName = "Assistant Dishwasher"; }}
        public override Job PromotionJob { get { return PromotionJob = HeadDishwasher.Instance; } } 

        public override Job DemoteJob { get; set; }
        //new public int CurrentPerfomanceLevel = 3;


        public override float XPbonus { get { return XPbonus = 10; } }
         override public List<Calendar.Weekday> WorkingDays
        {
            get
            {
                return WorkingDays = new List<Calendar.Weekday> { Calendar.Weekday.Wednesday , Calendar.Weekday.Thursday, Calendar.Weekday.Friday, Calendar.Weekday.Sunday,
            Calendar.Weekday.Saturday};
            }
         }

         override public float JobStartingTime
         {
             get { return Instance.JobStartingTime = 36000; ; }
         }

    }
    [ShowInInspector] 
    [System.Serializable]
    public class HeadDishwasher : JobManager.Job
    {
        override public List<int> RequiredSkillsLevelsForPromotion { get; set; } = new List<int> { 3 };
        new public int JobCareerLevel = 2;
        new public static JobManager.Job Instance = new HeadDishwasher();
        new public float DefaultMoneyAtTheEndOfWorkingDay = 68;

        override public List<SkillType> RequiredSkills { get { return RequiredSkills = new List<SkillType>() { { SkillType.Cooking } }; } }
        new public JobType jobType = JobType.Cooking;

        override public string JobName { get { return JobName = "Head Dishwasher"; } }
        
        public override Job DemoteJob { get { return PromotionJob = AssistantDishwasher.Instance; } }
        //new public int CurrentPerfomanceLevel = 3;


        public override float XPbonus { get { return XPbonus = 10; } }
        override public List<Calendar.Weekday> WorkingDays
        {
            get
            {
                return WorkingDays = new List<Calendar.Weekday> { Calendar.Weekday.Wednesday , Calendar.Weekday.Thursday, Calendar.Weekday.Friday, Calendar.Weekday.Sunday,
            Calendar.Weekday.Saturday};
            }
        }

        override public float JobStartingTime
        {
            get { return Instance.JobStartingTime = 36000; ; }
        }

    }*/




}



[System.Serializable]
public enum JobType
{
    Cooking, Journalism, Athlete, Science, Detective, Unemployed
}

[System.Serializable]
public struct JobData
{
    public int joblevel;
    public JobType type;
}