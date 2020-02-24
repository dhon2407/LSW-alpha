using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.U2D;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using GameSettings;
using Stats = PlayerStats.Stats;
using System;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI HPText;

    private static bool UIExists;
    public bool IsFullscreen = true;
    [Header("UI elements references")]
    public GameObject statsUI;
    public GameObject SoundOptionsUI;
    public GameObject TutorialUI;
    public GameObject exitUi;
    public GameObject CurrentLevelingSkillIcon;
    public GameObject HealthIcon, EnergyIcon, MoodIcon, ShowerIcon, BladderIcon, HungerIcon, ThirstIcon;
    public UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera perfectCamera;
    [Header("Key options")]
    public KeyCode switchStats = KeyCode.F1;
    public KeyCode switchOptions = KeyCode.F3;
    public KeyCode switchChat = KeyCode.F4;
    public KeyCode switchTutorial = KeyCode.F9;
    public KeyCode switchExit = KeyCode.Escape;


    [Header("Status bars")]

    public TextMeshProUGUI Money;

    public GameObject XPbarParent;
    public Image XPbar;
    public float ShowTimeInSeconds = 2;
    public TextMeshProUGUI LevelingSkill;
    public Image CurrentSkillImage;
    public Sprite Intelligence;
    public Sprite Strength;
    public Sprite Fitness;
    public Sprite Repair;
    public Sprite Charisma;
    public Sprite Cooking;
    public Sprite Writing;
    public Sprite Mixing;

    public Color MaxColor;
    public Color MiddleColor;
    public Color MinColor;
    public Gradient gradient;                       //How bars change their color depending on fill amount
    public float StartingColorTime = 1;

    public static UIManager Instance;

    public GameObject ChatGameObject;


    [Header("Video options")]
    public TMP_Dropdown ResolutionOptions;
    public TMP_Dropdown DisplayModeOptions;


    private float previousFillAmount;
    private bool isXPBarCoroutineRunning = false;
    private float CurrentShowTime = 0;
    // Start is called before the first frame update
    private void Awake()
    {
		if (!Instance) {
			Instance = this;
			DontDestroyOnLoad(gameObject);

            SceneManager.activeSceneChanged += DestroyOnMenuScreen;
        }
		else {
			Destroy(gameObject);
			return;
		}
    }

    private void DestroyOnMenuScreen(Scene oldScene, Scene newScene)
    {
        if (newScene.buildIndex == 0)//Main Menu index = 0
            Destroy(Instance.gameObject);
    }

    void Start()
    {
        if (!PlayerPrefs.HasKey("Was tutorial done"))
        {
            PlayerPrefs.SetInt("Was tutorial done", 1);
            TutorialUI.GetComponent<UIPopUp>().Open();
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SwitchTo(int sceneIndex)
    {
        GameFile.Data.Save();
        SceneManager.LoadScene(sceneIndex);
    }

    public void SwitchChat()
    {
        ChatGameObject.SetActive(!ChatGameObject.activeSelf);
    }
    public void SwitchSoundOptions()
    {
        SoundOptionsUI.GetComponent<UIPopUp>()?.ToggleState();
    }

    public void SwitchTutorialWindow()
    {
        TutorialUI.GetComponent<UIPopUp>()?.ToggleState();
    }
    public void ChangeDisplayMode(TMP_Dropdown dropdown)
    {
        switch (dropdown.value)
        {
            case 0:
                {
                    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                    Debug.Log("Fullscreen mode.");
                    PlayerPrefs.SetInt("DisplayMode", 0);
                    IsFullscreen = true;
                    break;
                }
            case 1:
                {
                    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, false);
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    Debug.Log("Fullscreen borderless window mode.");
                    PlayerPrefs.SetInt("DisplayMode", 1);
                    IsFullscreen = false;
                    break;
                }
            case 2:
                {
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    Debug.Log("Windowed mode.");
                    PlayerPrefs.SetInt("DisplayMode", 2);
                    IsFullscreen = false;
                    break;
                }
        }


    }
    public void ChangeResolution(TMP_Dropdown dropdown)
    {
        switch (dropdown.value)
        {
            case 0:
                {
                    Screen.SetResolution(1080, 720, IsFullscreen);
                    PlayerPrefs.SetInt("Resolution", 0);
                    Debug.Log("1080:720");
                    PlayerPrefs.Save();
                    break;
                }
            case 1:
                {
                    Screen.SetResolution(1920, 1080, IsFullscreen);
                    PlayerPrefs.SetInt("Resolution", 1);
                    Debug.Log("1920:1080");
                    PlayerPrefs.Save();
                    break;
                }
            case 2:
                {
                    Screen.SetResolution(3440, 1440, IsFullscreen);
                    PlayerPrefs.SetInt("Resolution", 2);
                    Debug.Log("3440:1440");
                    PlayerPrefs.Save();
                    break;
                }

        }


    }

    public void SwitchExit()
    {
        exitUi.GetComponent<UIPopUp>()?.ToggleState();
    }

    public void SwitchStats()
    {
        statsUI.GetComponent<UIPopUp>()?.ToggleState();
    }

    public IEnumerator<float> ShowLevelingSkill()
    {
        CurrentShowTime = 0;
        if (isXPBarCoroutineRunning)
        {
            CurrentShowTime = 0;
        }
        if (!isXPBarCoroutineRunning)
        {
            isXPBarCoroutineRunning = true;


            while (CurrentShowTime < ShowTimeInSeconds)
            {
                CurrentShowTime += GameTime.Time.deltaTime;
                XPbarParent.gameObject.SetActive(true);
				yield return 0f;
            }
            /*while (BubbleSpawner.Instance.particlesystem.isPlaying)
            {
                yield return 0f;
            }*/
            Debug.Log("Closing");
            XPbarParent.gameObject.SetActive(false);
            isXPBarCoroutineRunning = false;
        }
    }

}
