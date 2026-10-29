using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AlcoholPanelUI : MonoBehaviour
{
    private const string AlcoholPanelRootName = "AlcoholPanel";
    private static bool hasReceivedVisibilityRequest;

    [SerializeField]
    private Button closeButton;

    [SerializeField]
    private UnityEvent onAlcoholPurchased;

    [SerializeField]
    private UnityEvent<string> onAlcoholSelected;

    private readonly List<Button> alcoholButtons = new List<Button>();
    private readonly Dictionary<Button, UnityAction> purchaseListeners = new Dictionary<Button, UnityAction>();

    private void Awake()
    {
        ResolveButtons();
        BindButtons();
    }

    private void OnEnable()
    {
        ResolveButtons();
        BindButtons();
    }

    private void Start()
    {
        // Covers panels left active in the scene before a local player has spawned.
        if (!hasReceivedVisibilityRequest)
            SetVisibleForLocalPlayer(false);
    }

    public static void SetVisibleForLocalPlayer(bool visible)
    {
        hasReceivedVisibilityRequest = true;

        GameObject panelRoot = FindAlcoholPanelRoot();
        if (panelRoot != null)
            panelRoot.SetActive(visible);
    }

    // Retained for the scene-generation utility. Product buttons are discovered automatically.
    public void Configure(Button _)
    {
        ResolveButtons();
        BindButtons();
    }

    public void BuyAlcohol()
    {
        BuyAlcohol("Alcohol");
    }

    private void BuyAlcohol(string alcoholName)
    {
        // The game currently has no currency or inventory system. This event is the
        // integration point for one; the selected product is supplied to the event.
        Debug.Log($"[AlcoholPanel] Purchased: {alcoholName}");
        onAlcoholPurchased?.Invoke();
        onAlcoholSelected?.Invoke(alcoholName);
        ClosePanel();
    }

    public void ClosePanel()
    {
        SetVisibleForLocalPlayer(false);
    }

    private void ResolveButtons()
    {
        if (closeButton == null)
            closeButton = FindButtonByName(transform, "CloseButton");

        alcoholButtons.Clear();

        // Every button in AlcoholPanel except CloseButton is a purchasable alcohol item.
        // This deliberately does not require a ScrollRect, so custom UI hierarchies work too.
        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            if (button != null && button != closeButton)
                alcoholButtons.Add(button);
        }
    }

    private void BindButtons()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(ClosePanel);
            closeButton.onClick.AddListener(ClosePanel);
        }

        foreach (Button button in alcoholButtons)
        {
            EnsureHoverEffect(button);

            if (purchaseListeners.ContainsKey(button))
                continue;

            Button selectedButton = button;
            UnityAction listener = () => BuyAlcohol(GetAlcoholName(selectedButton));
            purchaseListeners.Add(selectedButton, listener);
            selectedButton.onClick.AddListener(listener);
        }
    }

    private static void EnsureHoverEffect(Button button)
    {
        if (button.GetComponent<AlcoholButtonHoverScale>() == null)
            button.gameObject.AddComponent<AlcoholButtonHoverScale>();
    }

    private static Button FindButtonByName(Transform root, string buttonName)
    {
        foreach (Button button in root.GetComponentsInChildren<Button>(true))
        {
            if (button.name == buttonName)
                return button;
        }

        return null;
    }

    private static string GetAlcoholName(Button button)
    {
        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        return label != null && !string.IsNullOrWhiteSpace(label.text)
            ? label.text.Trim()
            : button.name;
    }

    private static GameObject FindAlcoholPanelRoot()
    {
        foreach (Transform transform in FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (transform.name == AlcoholPanelRootName)
                return transform.gameObject;
        }

        return null;
    }
}
