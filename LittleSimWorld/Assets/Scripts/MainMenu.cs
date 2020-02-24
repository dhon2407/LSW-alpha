using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Sirenix.Serialization;
using System.IO;
using TMPro;
using System.Linq;
using System;
using GameFile;

public class MainMenu : MonoBehaviour
{
    private static MainMenu instance;
    private const int GameSceneIndex = 1;
    
    public GameObject loadingPanal;
    public Slider loadingSlider;
    public TextMeshProUGUI loadingText;
    public GameObject saveGO;
    public GameObject noSaveGO;
    public ScrollSnap loadPreview;
    public GameObject loadMenu;

    [SerializeField]
    private Texture2D cursor = null;

    private void Awake()
    {
        instance = this;
        CursorSet(cursor);
    }
    public static void LoadGameScene(int sceneIndex = GameSceneIndex)
    {
        instance.loadingPanal.SetActive(true);
        instance.StartCoroutine(instance.LoadAsyncronously(sceneIndex));
    }

    public void LoadSaveFiles()
    {
        LoadSaves();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public static void LoadSaves()
    {
        instance.loadPreview.ClearElements();

        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        var files = dir.GetFiles().Where(obj => obj.Name.EndsWith(Save.fileExtension));

        if (files.Count() == 0)
            instance.NoSavePreview();
        else
            instance.PopulateSavePreview(files);
    }

    private void NoSavePreview()
    {
        var nodata = Instantiate(noSaveGO).GetComponent<NoDataPreview>();
        nodata.SetBackButtonAction(() => loadMenu.SetActive(false));
        nodata.name = "No save files";
        loadPreview.PushLayoutElement(nodata.GetComponent<LayoutElement>());

        loadPreview.SnapToIndex(0);
    }

    private void PopulateSavePreview(IEnumerable<FileInfo> files)
    {
        foreach (FileInfo file in files)
        {
            DataFormat format = DataFormat.Binary;
            var bytes = File.ReadAllBytes(file.FullName);
            if (bytes == null)
            {
                Debug.LogWarning($"Failed reading file: {file.Name}.");
                continue;
            }

            SaveData saveFile = SerializationUtility.DeserializeValue<SaveData>(bytes, format);
            if (saveFile == null)
            {
                Debug.LogWarning($"Failed serializing file: {file.Name}.");
                continue;
            }

            LoadDataPreview save = Instantiate(saveGO).GetComponent<LoadDataPreview>();
            save.name = file.Name.Replace(Save.fileExtension, "");

            save.Initialize(save.name, saveFile);
            save.SetBackAction(() => loadMenu.SetActive(false));
            
            loadPreview.PushLayoutElement(save.GetComponent<LayoutElement>());
        }

        loadPreview.SnapToIndex(0);
    }

    private IEnumerator LoadAsyncronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {

            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingSlider.value = progress;
            loadingText.text = Mathf.Round((progress * 100f)) + "%";

            if (progress > 0.49)
            {
                loadingText.color = Color.white;
            }

            yield return null;
        }

		SpriteControler.Instance.FaceDOWN();
    }
    private void CursorSet(Texture2D tex)
    {
        CursorMode mode = CursorMode.Auto;
        var xspot = 0;
        var yspot = 0;
        Vector2 hotSpot = new Vector2(xspot, yspot);
        Cursor.SetCursor(tex, hotSpot, mode);
    }
}
