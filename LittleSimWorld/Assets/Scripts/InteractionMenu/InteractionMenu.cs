using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;
using HighlightPlus2D;
using System.Collections.Generic;

namespace Objects
{
    public class InteractionMenu : MonoBehaviour
    {
        HighlightManager2D hlm;
        GameObject mainCam;
        [SerializeField] List<MainMenuButtons> buttonInfo;
        [SerializeField] GameObject mainInteractionMenuUI = default, mouseHoverUI = default, player = default;
        [SerializeField] float speed = default, distMulti = default, playerDistanceForOppLocation = default;
        [SerializeField] GameObject[] secondaryMenuParents;
        TextMeshProUGUI hoverText;
        CanvasGroup uiAlpha;
        GridLayoutGroup gridLayout;
        Camera _cam;
        InteractionMenuModel[] secondaryMenus;
        public int menuLevel = 0;
        
        

        #region simple singleton & awake

        public static InteractionMenu instance;

        private void Awake()
        {
            //checking if the instance already exits, and implementing it by the use of
            //if and else to allow content to be added in the future
            if (instance == null)
            {
                instance = this;
                return;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        private void Start()
        {
            //initializing cached variables
            gridLayout = mainInteractionMenuUI.GetComponentInChildren<GridLayoutGroup>();
            hoverText = mouseHoverUI.GetComponentInChildren<TextMeshProUGUI>();
            uiAlpha = mainInteractionMenuUI.GetComponentInChildren<CanvasGroup>();
            mainCam = Camera.main.gameObject;
            _cam = mainCam.GetComponent<Camera>();
            hlm = FindObjectOfType<HighlightManager2D>();

            foreach (var item in buttonInfo)
            {
                item.SetupVariables();
                item.obj.SetActive(false);
            }

            secondaryMenus = new InteractionMenuModel[secondaryMenuParents.Length];
            for (int i = 0; i < secondaryMenuParents.Length; i++)
            {
                secondaryMenus[i] = new InteractionMenuModel(secondaryMenuParents[i]);
                secondaryMenus[i].mainObj.SetActive(false);
            }

            mainInteractionMenuUI.SetActive(false);
            uiAlpha.alpha = 0;

            GameLibOfMethods.OnPassOut.AddListener(ExitInterMenu);
        }

        

        /// <summary>
        /// Activate the interaction menu for the object interacted
        /// </summary>
        /// <param name="obj"> the object the player interacted with </param>
        /// <param name="activeOpt"> active and interactable options </param>
        /// <param name="greydOutOpt"> greyed out buttons not interactable options </param>
        /// <param name="hoverDescrip"> hover text for the greyed out buttons </param>
        public void ActivateInterMenu(GameObject obj, Transform centerPoint, List<MainMenuOptions> invokerOptions)
        {
            GameLibOfMethods.cantMove = true;
            GameLibOfMethods.canInteract = false;
            hlm.menuOpen = true;

            // checking if the UI can handle the number of interactions input by the object
            var activeOL = invokerOptions.Count;
            if (activeOL > buttonInfo.Count) { Debug.Log("Number of interactions is larger than number of buttons, create new buttons on interactionUI"); return; }

            //find the menu position on the canvas axis
            int a = (centerPoint.position.x > player.transform.position.x) ? -1 : 1;
            int b = ((centerPoint.position - player.transform.position).sqrMagnitude > playerDistanceForOppLocation) ? -1 : 1;
            mainInteractionMenuUI.transform.position = _cam.WorldToScreenPoint(centerPoint.position) - new Vector3(a,0,0) * distMulti * b;

            //setting the grid constrain to not allow funny layouts
            if (mainInteractionMenuUI.transform.position.x > _cam.WorldToScreenPoint(centerPoint.position).x)
            {
                gridLayout.childAlignment = TextAnchor.MiddleLeft;
                gridLayout.startCorner = GridLayoutGroup.Corner.LowerLeft;
            }
            else
            {
                gridLayout.childAlignment = TextAnchor.MiddleRight;
                gridLayout.startCorner = GridLayoutGroup.Corner.LowerRight;
            }
            

            // enable all the relevant buttons and set their function
            var component = obj.GetComponent<IInteractionOptions>();
            if (component == null) { Debug.LogError("IInteractionOptions Interface not set up, please contact Henrique / Developers"); return; }
            
            for (int i = 0 ; i < activeOL; i++)
            {
                buttonInfo[i].obj.SetActive(true);
                buttonInfo[i].interactable = invokerOptions[i].interactable;
                if (invokerOptions[i].interactable) buttonInfo[i].trigger.enabled = false;
                else { buttonInfo[i].interactable = false; buttonInfo[i].trigger.enabled = true; buttonInfo[i].trigger.buttonSetup(true); }
                buttonInfo[i].buttonText.text = invokerOptions[i].optionName;
                buttonInfo[i].hoverMessage = invokerOptions[i].hoverText;
                buttonInfo[i].button.onClick.RemoveAllListeners();
                var stepp = i;
                buttonInfo[stepp].button.onClick.AddListener(() => MenuMouseTrigger.activeButton = buttonInfo[stepp].obj.transform);
                var step = invokerOptions[i].id;
                if (i > 0) buttonInfo[i].button.onClick.AddListener(delegate { component.Interact(step); });
                buttonInfo[i].button.onClick.AddListener(ExitInterMenu);
                
            }

            //finally actvating it and start the listener for actions that exit the menu
            mainInteractionMenuUI.SetActive(true);
            StartCoroutine(CancellingInput());
            uiAlpha.LeanAlpha(1, speed);
        }

        #region Secondary Menu
        GameObject currentButton; Transform parent; int childIndex; Image buttonImage;
        public void SecondaryMenuActivation(string[] options, Action[] actions, int level) // 0 being the first secondary level
        {
            if (level == 0) uiAlpha.alpha = .5f;
            else secondaryMenus[level - 1].canvasGroup.alpha = .5f;
            secondaryMenus[level].canvasGroup.alpha = 1;

            var activButton = MenuMouseTrigger.activeButton;
            secondaryMenus[level].mainObj.transform.position = activButton.position;
            secondaryMenus[level].previousButtonTextContainer.text = activButton.GetComponentInChildren<TextMeshProUGUI>().text;
            secondaryMenus[level].previousButton.SetActive(true);

            if (secondaryMenus[level].buttons.Length < options.Length) { Debug.Log("Add more Buttons into the secondary menu canvas."); return; }
            secondaryMenus[level].mainObj.SetActive(true);
            menuLevel = level + 1;

            for (int i = 0; i < options.Length; i++)
            {
                secondaryMenus[level].buttons[i].gameObject.SetActive(true);
                secondaryMenus[level].buttons[i].onClick.RemoveAllListeners();
                var stepp = i;
                secondaryMenus[level].buttons[stepp].onClick.AddListener(() => MenuMouseTrigger.activeButton = secondaryMenus[level].buttons[stepp].gameObject.transform);
                var step = i;
                secondaryMenus[level].buttons[i].onClick.AddListener(delegate { actions[step](); });
                secondaryMenus[level].buttons[i].onClick.AddListener(ExitInterMenu);
                secondaryMenus[level].textContainers[i].text = options[i];
            }
            if (buttonInfo.Count > options.Length)
            {
                for (int i = options.Length; i < secondaryMenus[0].buttons.Length; i++)
                {
                    secondaryMenus[level].buttons[i].gameObject.SetActive(false);
                    secondaryMenus[level].buttons[i].onClick.RemoveAllListeners();
                }
            }
        }




        #endregion

        #region Hover Functions
        bool moveUIbo;
        public void ShowUINotAvailable(GameObject butt, bool activeStatus)
        {
            if (!activeStatus) { mouseHoverUI.SetActive(activeStatus); moveUIbo = false; return; }
            hoverText.text = buttonInfo.Find(x => x.obj == butt).hoverMessage;
            mouseHoverUI.SetActive(true);
            StartCoroutine(MoveUI());
        }
        IEnumerator MoveUI()
        {
            moveUIbo = true;
            while (moveUIbo)
            {
                mouseHoverUI.transform.position = Input.mousePosition;
                yield return null;
            }
        }
        #endregion

        #region Exitng Menu Functions
        public void ExitInterMenu()
        {
            if (menuLevel > 0) return;
            foreach (var item in secondaryMenus)
            {
                item.previousButton.SetActive(false);
                item.canvasGroup.LeanAlpha(0, .2f);
            }
            GameLibOfMethods.cantMove = false;
            GameLibOfMethods.canInteract = true;
            buttonInfo[0].interactable = false;
            for (int i = 1; i < buttonInfo.Count; i++)
            {
                buttonInfo[i].interactable = false;
                buttonInfo[i].trigger.buttonSetup(false);
                buttonInfo[i].trigger.enabled = false;
            }
            uiAlpha.LeanAlpha(0, speed);
            StartCoroutine(DeactivateUI());
        }

        IEnumerator DeactivateUI()
        {
            while (uiAlpha.alpha > 0) yield return null;
            for (int i = 1; i < buttonInfo.Count; i++)
            {
                buttonInfo[i].obj.SetActive(false);
            }
            foreach (var item in secondaryMenus)
            {
                item.mainObj.SetActive(false);
            }
            hlm.menuOpen = false;
            mainInteractionMenuUI.SetActive(false);
            mouseHoverUI.SetActive(false);
            StopAllCoroutines();
        }

        IEnumerator CancellingInput()
        {
            yield return null;
            for (; ; )
            {
                if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 || Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.E)) break;
                yield return null;
            }
            yield return null;
            menuLevel = 0;
            ExitInterMenu();
        }

        public void SecondaryMenuClose(int a)
        {
            if (a == 0) uiAlpha.alpha = 1;
            else secondaryMenus[a - 1].canvasGroup.alpha = 1;
            menuLevel = a;
            secondaryMenus[a].previousButton.SetActive(false);
            var corout = secondaryMenus[a].Close();
            StartCoroutine(corout);
        }
        #endregion
    }
}
