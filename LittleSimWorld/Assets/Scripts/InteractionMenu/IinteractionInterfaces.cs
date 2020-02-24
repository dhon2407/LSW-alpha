using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public interface IInteractionOptions
    {
        void Interact(int interactionNo);
    }

    public interface ImenuInteraction
    {
        void OpenMenu();
    }
}
