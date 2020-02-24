using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Objects;

public class HomePCInteractions : InteractableObject, IInteractionOptions
{
    FindCareerUi jobCanvas;
    int progressBarID;
    public float searchSpeed = 3;
    [Range(0, 100)]
    public float searchProgress;
    bool searching, quitting;

    HomeComputer mainPCFunctions;
    ComputerChair currentChair;
    Action[] buttonsActions;
    int currentAction;
    ComputerInteractionModel write, art, compose;
    public float[] savedProgress
    {
        get { return new float[] { write.progress, art.progress, compose.progress }; }
        set
        {
            write.progress = value[0];
            art.progress = value[1];
            compose.progress = value[2];
        }
    }
    public static HomePCInteractions instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this)
        {
            Destroy(instance);
            instance = null;
            instance = this;
        }
    }


    private void Start()
    {
        write = new ComputerInteractionModel(InteractType.write);
        art = new ComputerInteractionModel(InteractType.art);
        compose = new ComputerInteractionModel(InteractType.compose);

        jobCanvas = FindObjectOfType<FindCareerUi>();
        currentChair = FindObjectOfType<ComputerChair>();
        this.interactionOptions.PlayerPathfindingTarget = currentChair.interactionOptions.PlayerPathfindingTarget;
    }

    public override void Interact() {}


    public void Interact(int CurrentAction)
    {
        if (currentChair == null) currentChair = currentChair = FindObjectOfType<ComputerChair>();
        if (CurrentAction == 0) return;
        else if (CurrentAction == 1) CareersMenu();
        else if (CurrentAction == 2) ArtSubMenu();
        else if (CurrentAction == 3) ComposeSubMenu();
        else if (CurrentAction == 4) WriteSubMenu();
        else if (CurrentAction == 5) currentChair.TrueInteract(() => SimpleAction("Study"));
        else if (CurrentAction == 6) currentChair.TrueInteract(() => SimpleAction("Browse"));
    }

    public void ConfirmationMenu(Action action)
    {
        string[] options = { "Back", "Confirm" };
        var _actions = new Action[2];
        _actions[0] = () => InteractionMenu.instance.SecondaryMenuClose(2);
        _actions[1] = () => currentChair.TrueInteract(action);
        InteractionMenu.instance.SecondaryMenuActivation(options, _actions, 2);
    }

    #region Sub Menus

    public void WriteSubMenu()
    {
        string[] options = { "Practice Writing", "Write a Book" };
        var _actions = new Action[2];
        _actions[0] = () => currentChair.TrueInteract(() => SimpleAction("Write"));
        _actions[1] = () => WriteABookSubMenu();
        InteractionMenu.instance.SecondaryMenuActivation(options, _actions, 0);
    }

    public void WriteABookSubMenu()
    {
        if (write.progress == 0)
        {
            currentChair.TrueInteract(() => GeneralProgressAction(write));
        }
        else
        {
            string[] options = { "Scrap Book", "Continue Writing" };
            var _actions = new Action[2];
            _actions[0] = () => { write.progress = 0; ConfirmationMenu(() => GeneralProgressAction(write)); };
            _actions[1] = () => currentChair.TrueInteract(() => GeneralProgressAction(write));
            InteractionMenu.instance.SecondaryMenuActivation(options, _actions, 1);
        }
    }

    public void ComposeSubMenu()
    {
        string[] options = { "Practice Music", "Compose Music" };
        var _actions = new Action[2];
        _actions[0] = () => currentChair.TrueInteract(() => SimpleAction("Write"));
        _actions[1] = () => ComposeMusicSubMenu();
        InteractionMenu.instance.SecondaryMenuActivation(options, _actions, 0);
    }

    public void ComposeMusicSubMenu()
    {
        if (compose.progress == 0)
        {
            currentChair.TrueInteract(() => GeneralProgressAction(compose));
        }
        else
        {
            string[] options = { "Scrap Composition", "Continue Composing" };
            var _actions = new Action[2];
            _actions[0] = () => ConfirmationMenu(() => SimpleAction("Write"));
            _actions[1] = () => currentChair.TrueInteract(() => SimpleAction("Write"));
            InteractionMenu.instance.SecondaryMenuActivation(options, _actions, 1);
        }
    }

    public void ArtSubMenu()
    {
        if (art.progress == 0)
        {
            currentChair.TrueInteract(() => GeneralProgressAction(art));
        }
        else
        {
            string[] options = { "Practice Art", "Create Artpiece" };
            var _actions = new Action[2];
            _actions[0] = () => currentChair.TrueInteract(() => SimpleAction("Write"));
            _actions[1] = () => ComposeArtSubMenu();
            InteractionMenu.instance.SecondaryMenuActivation(options, _actions, 0);
        }

    }

    public void ComposeArtSubMenu()
    {
        string[] options = { "Scrap Drawing", "Continue Drawing" };
        var _actions = new Action[2];
        _actions[0] = () => ConfirmationMenu(() => SimpleAction("Write"));
        _actions[1] = () => currentChair.TrueInteract(() => GeneralProgressAction(art));
        InteractionMenu.instance.SecondaryMenuActivation(options, _actions, 1);
    }

    public void CareersMenu()
    {
        string[] options = { "Quit Job", "Find a Job" };
        var _actions = new Action[2];
        _actions[0] = () => ConfirmationMenu(() => QuitJob());
        _actions[1] = () => currentChair.TrueInteract(() => FindJob());
        InteractionMenu.instance.SecondaryMenuActivation(options, _actions, 0);
    }

    /*public void FindAJobMenu()
    {
        string[] options = { "Scrap Composition", "Continue Composing" };
        var _actions = new Action[2];
        _actions[0] = () => ConfirmationMenu(() => SimpleAction("Write"));
        _actions[1] = () => currentChair.TrueInteract(() => SimpleAction("Write"));
        InteractionMenu.instance.SecondaryMenuActivation(options, _actions, 1);
    }*/

    #endregion

    protected void Update()
    {
        if (searching) Search(quitting);
        else if (write.currentActive) Progression(write);
        else if (art.currentActive) Progression(art);
        else if (compose.currentActive) Progression(compose);

    }

    void SimpleAction(string actionName)
    {
        var result = currentChair.TrySetCurrentAction(actionName);
        if (result)
        {
            InteractionMenu.instance.menuLevel = 0;
            InteractionMenu.instance.ExitInterMenu();
            currentChair.BeginUsing();
        }
        else { result.PrintErrorMessage(); }
    }

    void GeneralProgressAction(ComputerInteractionModel interaction)
    {
        InteractionMenu.instance.menuLevel = 0;
        InteractionMenu.instance.ExitInterMenu();
        var result = currentChair.TrySetCurrentAction(interaction.actionName);
        if (result)
        {
            interaction.currentActive = true;
            currentChair.BeginUsing();
            float GetProgress() => interaction.progress;
            progressBarID = ProgressBar.StartTracking(interaction.description, GetProgress, 100);
        }
        else { result.PrintErrorMessage(); }
    }
    
    void QuitJob()
    {
        searchProgress = 0;
        InteractionMenu.instance.menuLevel = 0;
        InteractionMenu.instance.ExitInterMenu();
        var result = currentChair.TrySetCurrentAction("QuitJob");
        if (result)
        {
            searching = true;
            quitting = true;
            currentChair.BeginUsing();
            float GetProgress() => searchProgress;
            progressBarID = ProgressBar.StartTracking("Quitting Job", GetProgress, 100);
        }
        else { result.PrintErrorMessage(); }
    }

    void FindJob()
    {
        searchProgress = 0;
        InteractionMenu.instance.menuLevel = 0;
        InteractionMenu.instance.ExitInterMenu();
        var result = currentChair.TrySetCurrentAction("Find Work");
        if (result)
        {
            searching = true;
            currentChair.BeginUsing();
            float GetProgress() => searchProgress;
            progressBarID = ProgressBar.StartTracking("Searching For Work", GetProgress, 100);
        }
        else { result.PrintErrorMessage(); }

    }


    public enum TimeEnum { Full, Half, Quarter }
    public float GetFloat(TimeEnum waitTime)
    {
        switch (waitTime)
        {
            case TimeEnum.Full:
                break;
            case TimeEnum.Half:
                break;
            case TimeEnum.Quarter:
                break;
            default:
                break;
        }

        return 0;
    }

    public virtual bool Search(bool quitting = false)
    {
        searchProgress += searchSpeed * GameTime.Time.deltaTime;

        if (searchProgress >= 100)
        {
            searchProgress = 0;
            searching = false;
            ProgressBar.HideUI(progressBarID);
            if (!quitting)
            {
                jobCanvas.gameObject.SetActive(true);
                jobCanvas.anim.ReOpen();
            }

            else
            {
                JobManager.Instance.QuitJob();
                quitting = false;
            }
            PlayerAnimationHelper.ResetAnimations();

            return true;
        }

        return false;
    }

    void Progression(ComputerInteractionModel type)
    {
       type.progress += type.speed * GameTime.Time.deltaTime;
        if (type.progress >= 100)
        {
            type.progress = 0;
            type.currentActive = false;
            ProgressBar.HideUI(progressBarID);

            //reward funcion;

            PlayerAnimationHelper.ResetAnimations();

        }
    }

    public void FinishActions()
    {
        CloseJobCanvas();
        if (searching || art.currentActive || write.currentActive || compose.currentActive)
        {
            ProgressBar.HideUI(progressBarID);
            searching = false;
            art.currentActive = false;
            write.currentActive = false;
            compose.currentActive = false;
        }
    }

    public void CloseJobCanvas()
    {
        if (jobCanvas != null) jobCanvas.gameObject.SetActive(false);
        searchProgress = 0;
    }






}
