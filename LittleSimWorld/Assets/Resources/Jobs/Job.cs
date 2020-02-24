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
using PlayerStats;
using PlayerStats.Buffs;
using PlayerStats.Buffs.Core;
using UI.Notifications;

[ShowInInspector]
[SerializeField]
[System.Serializable]
[CreateAssetMenu(fileName = "Job")]
public class Job : SerializedScriptableObject
{
    [Header("Job Settings")]

    public JobType jobType;

    public float XPbonus;

    public float WagePerHour = 50;

    [ShowInInspector]
    [System.NonSerialized]
    private int CurrentPerfomanceLevel = 3;

    public int JobCareerLevel;

    public Job PromotionJob;

    public Job DemoteJob;

    public Dictionary<Skill.Type, int> RequiredSkills;


    [Header("Job Timings")]

    public float JobStartingTimeInHours;

    public float WorkingTimeInHours = 28800f;

    public List<Calendar.Weekday> WorkingDays;
    [Header("Misc.")]

    public int MaxSegments;
    public float MaxCarWaitTime = 45;
    public string Description;
    [System.NonSerialized]
    [ShowInInspector]
    public bool WorkedToday = false;

    public Sprite icon;

    public int GetPerformanceLevel()
    {
        return CurrentPerfomanceLevel;
    }

    public virtual void Penalize(bool jobQuit = false)
    {
        if (!jobQuit)
        {
            PlayerChatLog.Instance.AddChatMessege("You performing badly on your job.");
            Notify.Show("You performing badly on your job.", icon);
        }

        CurrentPerfomanceLevel -= 1;
        if (CurrentPerfomanceLevel == 0)
        {
            Demote();
        }
        CareerUi.Instance.UpdateJobUi();
    }
    public virtual void Demote()
    {
        PlayerChatLog.Instance.AddChatMessege("You got fired from your job.");
        Notify.Show("You got fired from your job.", icon);
        CurrentPerfomanceLevel = 3;
        JobManager.Instance.CurrentJob = DemoteJob ? DemoteJob : null;
        CareerUi.Instance.UpdateJobUi();
    }
    public virtual void PositiveWorkProgress()
    {
        CurrentPerfomanceLevel += 1;
        PlayerChatLog.Instance.AddChatMessege("You are doing well in your job!");
        Notify.Show("You are doing well in your job!", icon);
        if (CurrentPerfomanceLevel > MaxSegments)
        {

            Promote();
        }
        CareerUi.Instance.UpdateJobUi();
    }
    public virtual void Promote()
    {
        int i = 0;
        foreach (Skill.Type type in RequiredSkills.Keys)
        {
            i++;
            if (Stats.Skill(type).CurrentLevel >= RequiredSkills[type])
            {

            }
            else
            {
                return;
            }
        }
        CurrentPerfomanceLevel = 3;
        JobManager.Instance.CurrentJob = PromotionJob ? PromotionJob : this;
        PlayerChatLog.Instance.AddChatMessege("You got promoted on your job!");
        Notify.Show("You got promoted on your job!", icon);
        CareerUi.Instance.UpdateJobUi();
    }

    public virtual void Finish()
    {
        foreach (Skill.Type type in RequiredSkills.Keys)
        {
            Stats.AddXP(type, XPbonus);


        }
        WorkedToday = true;

        var salary = WagePerHour *
                     (float) System.TimeSpan.FromSeconds(JobManager.Instance.CurrentWorkingTime).TotalHours;

        if (PlayerBuff.HasBuff<Motivated>())
        {
            var statIncrease = new StatIncreaseArgs(salary);
            PlayerBuff.GetBuff<Motivated>()?.TakeEffect(statIncrease);
            salary = statIncrease.Amount;
        }

        Stats.AddMoney(salary);
    }
    public virtual void ShowUpAtWork()
    {
        float PerformanceScore = 0;
        foreach (Status stat in Stats.PlayerStatus.Values)
        {
            if (stat.type != PlayerStats.Status.Type.Bladder)
                PerformanceScore += stat.CurrentAmount;
        }
        if (PerformanceScore > 525)
        {
            PositiveWorkProgress();
        }
        if (PerformanceScore < 350)
        {
            Penalize();
        }
        CareerUi.Instance.UpdateJobUi();
    }

    public virtual void AssignToThisJob()
    {
        bool exists = false;
        if (JobManager.Instance.CurrentJob != this)
        {
            foreach(var job in JobManager.Instance.WorkedJobs){
                if (job.Key == JobManager.Instance.CurrentJob.jobType)
                {
                    exists = true;
                }
            }
            if (exists)
            {
                JobManager.Instance.CurrentJob.Penalize();
                JobManager.Instance.WorkedJobs[JobManager.Instance.CurrentJob.jobType] = JobManager.Instance.CurrentJob;
            }
        }
        if (JobManager.Instance.WorkedJobs.ContainsKey(jobType))
        {
            JobManager.Instance.CurrentJob = JobManager.Instance.WorkedJobs[jobType];
        }
        else
        {
            JobManager.Instance.CurrentJob = this;
            JobManager.Instance.WorkedJobs.Add(jobType, this);
        }

        PlayerChatLog.Instance.AddChatMessege("You are working on " + JobManager.Instance.CurrentJob.name + " job now.");
        Notify.Show("You are working on " + JobManager.Instance.CurrentJob.name + " job now.", JobManager.Instance.CurrentJob.icon);
        CareerUi.Instance.UpdateJobUi();
    }

}