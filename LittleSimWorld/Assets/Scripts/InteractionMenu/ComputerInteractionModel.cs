using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum InteractType
{
    write,
    art,
    compose
}


public class ComputerInteractionModel
{
    public InteractType interactionType;
    [Range(0, 100)]
    public float progress;
    public float speed;
    public string actionName;
    public string contAction;
    public string description;
    public bool currentActive;
    public string[] primaryMenuOptions, SecondaryMenuOptions;

    public ComputerInteractionModel(InteractType type)
    {
        currentActive = false;

        if (type == InteractType.write)
        {
            speed = 1;
            actionName = "Write";
            description = "Writing a Novel";
        }
        else if(type == InteractType.art)
        {
            speed = 1;
            actionName = "Create Art";
            description = "Creating an Artpiece";
        }
        else if (type == InteractType.compose)
        {
            speed = 1;
            actionName = "Create Music";
            description = "Composing a Song";
        }
    }


}

