using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHeadBubble : MonoBehaviour
{
    private const string DefaultBubbleSpritePath = "Assets/AART/ui/speak.png";

    [SerializeField]
    private float displayDuration = 5f;

    [SerializeField]
    private int maxBubbleCount = 3;

    [SerializeField]
    private Vector3 localOffset = new Vector3(6.4f, 6.9f, -0.1f);

    [SerializeField]
    private float stackSpacing = 2.9f;

    [SerializeField]
    private float stackMoveSpeed = 14f;

    [SerializeField]
    private Vector2 bubbleSize = new Vector2(840f, 220f);

    [SerializeField]
    private float worldScale = 0.015f;

    [SerializeField]
    private Sprite bubbleSprite;

    private readonly List<BubbleInstance> activeBubbles = new List<BubbleInstance>();

    private sealed class BubbleInstance
    {
        public GameObject Root;
        public Vector3 TargetLocalPosition;
        public Coroutine HideRoutine;
    }

    private void Awake()
    {
        ResolveBubbleSprite();
        HideImmediate();
    }

    public void Show(string message)
    {
        BubbleInstance bubble = CreateBubble(message);
        activeBubbles.Insert(0, bubble);

        while (activeBubbles.Count > Mathf.Max(1, maxBubbleCount))
            RemoveBubble(activeBubbles[activeBubbles.Count - 1]);

        RefreshStackPositions();
        bubble.HideRoutine = StartCoroutine(HideAfterDelay(bubble, displayDuration));
    }

    private BubbleInstance CreateBubble(string message)
    {
        GameObject root = new GameObject("HeadBubble");
        root.transform.SetParent(transform, false);
        root.transform.localPosition = localOffset;
        root.transform.localScale = Vector3.one * worldScale;

        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 50;
        canvas.worldCamera = Camera.main;

        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10f;

        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.sizeDelta = bubbleSize;

        GameObject background = new GameObject("Background");
        background.transform.SetParent(root.transform, false);

        RectTransform backgroundRect = background.AddComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;

        Image backgroundImage = background.AddComponent<Image>();
        backgroundImage.raycastTarget = false;
        backgroundImage.color = bubbleSprite != null ? Color.white : new Color(0.08f, 0.08f, 0.08f, 0.85f);
        backgroundImage.sprite = bubbleSprite;
        backgroundImage.type = Image.Type.Simple;
        backgroundImage.preserveAspect = bubbleSprite != null;

        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(root.transform, false);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(48f, 56f);
        textRect.offsetMax = new Vector2(-32f, -24f);

        TextMeshProUGUI label = textObject.AddComponent<TextMeshProUGUI>();
        label.font = TMP_Settings.defaultFontAsset;
        label.fontSize = 100;
        label.color = new Color(0.12f, 0.12f, 0.12f);
        label.alignment = TextAlignmentOptions.Top;
        label.enableWordWrapping = true;
        label.raycastTarget = false;
        label.text = message;

        return new BubbleInstance
        {
            Root = root,
            TargetLocalPosition = localOffset
        };
    }

    private IEnumerator HideAfterDelay(BubbleInstance bubble, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        RemoveBubble(bubble);
    }

    private void RemoveBubble(BubbleInstance bubble)
    {
        if (!activeBubbles.Remove(bubble))
            return;

        if (bubble.HideRoutine != null)
            StopCoroutine(bubble.HideRoutine);

        if (bubble.Root != null)
            Destroy(bubble.Root);

        RefreshStackPositions();
    }

    private void RefreshStackPositions()
    {
        for (int index = 0; index < activeBubbles.Count; index++)
            activeBubbles[index].TargetLocalPosition = localOffset + Vector3.up * stackSpacing * index;
    }

    private void HideImmediate()
    {
        foreach (BubbleInstance bubble in activeBubbles)
        {
            if (bubble.HideRoutine != null)
                StopCoroutine(bubble.HideRoutine);

            if (bubble.Root != null)
                Destroy(bubble.Root);
        }

        activeBubbles.Clear();
    }

    private void LateUpdate()
    {
        Camera camera = Camera.main;

        foreach (BubbleInstance bubble in activeBubbles)
        {
            if (bubble.Root == null)
                continue;

            Transform bubbleTransform = bubble.Root.transform;
            bubbleTransform.localPosition = Vector3.Lerp(
                bubbleTransform.localPosition,
                bubble.TargetLocalPosition,
                1f - Mathf.Exp(-stackMoveSpeed * Time.deltaTime));

            if (camera != null)
                bubbleTransform.rotation = camera.transform.rotation;
        }
    }

    private void OnDestroy()
    {
        HideImmediate();
    }

    private void ResolveBubbleSprite()
    {
        if (bubbleSprite != null)
            return;

#if UNITY_EDITOR
        bubbleSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(DefaultBubbleSpritePath);
#endif
    }
}
