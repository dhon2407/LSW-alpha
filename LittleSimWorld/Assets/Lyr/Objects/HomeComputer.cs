using GUI_Animations;

namespace Objects
{
    using UnityEngine;
    using UnityEngine.UI;

    public class HomeComputer : MonoBehaviour
    {
        public FindCareerUi JobCanvas;

        ComputerChair _chair;
        ComputerChair chair => _chair ? _chair : (_chair = FindObjectOfType<ComputerChair>());

        private Vector2 _computerLocation;
        //private UIPopUp _popup;

        public static HomeComputer instance;
        int progressBarID;
        public float searchSpeed = 3;
        [Range(0, 100)]
        public float searchProgress;
        bool searching;

        /*public void Open(Vector2 location)
        {
            _computerLocation = location;
            _popup.Open(_computerLocation, () =>
            {
                if (!_chair)
                    _chair = FindObjectOfType<ComputerChair>();
            });
        }

        public void Close()
        {
            _popup.ForceClose(_computerLocation, () =>
            {
                JobCanvas.gameObject.SetActive(false);
            });
            searchProgress = 0;
            if (searching)
            {
                ProgressBar.HideUI(progressBarID);
            }
        }*/


        
    }
}