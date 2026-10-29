using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class WcrSceneUiSetup
{
    private const string ScenePath = "Assets/Scenes/0623.unity";

    [MenuItem("Tools/0610 WCR/Generate UI Controls")]
    public static void Generate()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        GenerateIntoScene(scene);
    }

    private static void GenerateIntoScene(Scene scene)
    {
        RectTransform canvas = FindComponent<Canvas>("CanvasChat")?.GetComponent<RectTransform>();
        if (canvas == null)
        {
            Debug.LogError("[WcrSceneUiSetup] CanvasChat was not found.");
            return;
        }

        GameObject messageBoard = FindGameObject("MessageBoardScroll");
        if (messageBoard == null)
        {
            Debug.LogError("[WcrSceneUiSetup] MessageBoardScroll was not found.");
            return;
        }

        CreateMessageBoardButton(canvas, messageBoard);
        CreateAlcoholPanel(canvas);
        CreateTimeDebugLabel(canvas);

        messageBoard.SetActive(false);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[WcrSceneUiSetup] UI controls generated in 0623.unity.");
    }

    private static void CreateMessageBoardButton(RectTransform canvas, GameObject messageBoard)
    {
        GameObject buttonObject = FindGameObject("MessageBoardToggleButton") ?? CreateButton(
            "MessageBoardToggleButton", canvas, new Vector2(-760f, -380f), new Vector2(260f, 70f), "Message Board");

        MessageBoardPanelToggle toggle = buttonObject.GetComponent<MessageBoardPanelToggle>();
        if (toggle == null)
            toggle = buttonObject.AddComponent<MessageBoardPanelToggle>();

        ApplyUiSprite(buttonObject.GetComponent<Image>());
        toggle.Configure(messageBoard);
    }

    private static void CreateAlcoholPanel(RectTransform canvas)
    {
        GameObject panelObject = FindGameObject("AlcoholPanel");
        if (panelObject == null)
        {
            panelObject = CreatePanel("AlcoholPanel", canvas, new Vector2(0f, -40f), new Vector2(560f, 330f));
            CreateText("Title", panelObject.transform, "Buy Alcohol", 42, new Vector2(0f, 110f), new Vector2(500f, 70f));
            CreateText("Description", panelObject.transform, "You are seated. Choose a drink to continue.", 24, new Vector2(0f, 40f), new Vector2(500f, 50f));
            CreateButton("BuyButton", panelObject.transform, new Vector2(0f, -90f), new Vector2(300f, 74f), "Buy");
        }

        AlcoholPanelUI panel = panelObject.GetComponent<AlcoholPanelUI>();
        if (panel == null)
            panel = panelObject.AddComponent<AlcoholPanelUI>();

        ApplyUiSprite(panelObject.GetComponent<Image>());
        ApplyUiSprite(panelObject.GetComponentInChildren<Button>(true)?.GetComponent<Image>());
        panel.Configure(panelObject.GetComponentInChildren<Button>(true));
        panelObject.SetActive(false);
    }

    private static void CreateTimeDebugLabel(RectTransform canvas)
    {
        GameObject textObject = FindGameObject("TimeOfDayDebugText");
        if (textObject == null)
        {
            textObject = new GameObject("TimeOfDayDebugText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(canvas, false);
            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -32f);
            rect.sizeDelta = new Vector2(620f, 56f);

            TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
            text.font = TMP_Settings.defaultFontAsset;
            text.fontSize = 28;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
        }

        SpriteRenderer background = FindComponent<SpriteRenderer>("background");
        if (background == null)
            return;

        ComputerTimeOfDayController controller = background.GetComponent<ComputerTimeOfDayController>();
        if (controller == null)
            controller = background.gameObject.AddComponent<ComputerTimeOfDayController>();

        controller.Configure(background, textObject.GetComponent<TextMeshProUGUI>());
    }

    private static GameObject CreatePanel(string name, Transform parent, Vector2 position, Vector2 size)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image = panel.GetComponent<Image>();
        image.sprite = GetUiSprite();
        image.type = Image.Type.Sliced;
        image.color = new Color(0.08f, 0.06f, 0.04f, 0.95f);
        return panel;
    }

    private static GameObject CreateButton(string name, Transform parent, Vector2 position, Vector2 size, string label)
    {
        GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image = buttonObject.GetComponent<Image>();
        image.sprite = GetUiSprite();
        image.type = Image.Type.Sliced;
        image.color = new Color(0.9f, 0.66f, 0.2f, 1f);
        buttonObject.GetComponent<Button>().targetGraphic = image;

        CreateText("Text", buttonObject.transform, label, 30, Vector2.zero, size);
        return buttonObject;
    }

    private static Sprite GetUiSprite()
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>("Assets/AART/ui/buttonNormal.png");
    }

    private static void ApplyUiSprite(Image image)
    {
        if (image != null)
            image.sprite = GetUiSprite();
    }

    private static void CreateText(string name, Transform parent, string value, float fontSize, Vector2 position, Vector2 size)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.font = TMP_Settings.defaultFontAsset;
        text.text = value;
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
    }

    private static GameObject FindGameObject(string objectName)
    {
        Transform[] transforms = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Transform transform in transforms)
        {
            if (transform.name == objectName)
                return transform.gameObject;
        }

        return null;
    }

    private static T FindComponent<T>(string objectName) where T : Component
    {
        GameObject gameObject = FindGameObject(objectName);
        return gameObject != null ? gameObject.GetComponent<T>() : null;
    }
}
