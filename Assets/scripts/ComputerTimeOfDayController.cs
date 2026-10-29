using System;
using TMPro;
using UnityEngine;

public class ComputerTimeOfDayController : MonoBehaviour
{
    private enum TimeOfDay
    {
        Day,
        Dusk,
        Night
    }

    [Header("Scene targets")]
    [SerializeField]
    private SpriteRenderer backgroundRenderer;

    [SerializeField]
    private TMP_Text debugText;

    [Header("Sprites (assign when art is ready)")]
    [SerializeField]
    private Sprite daySprite;

    [SerializeField]
    private Sprite duskSprite;

    [SerializeField]
    private Sprite nightSprite;

    [Header("Computer-time boundaries")]
    [SerializeField, Range(0, 23)]
    private int dayStartsAtHour = 6;

    [SerializeField, Range(0, 23)]
    private int duskStartsAtHour = 18;

    [SerializeField, Range(0, 23)]
    private int nightStartsAtHour = 20;

    private TimeOfDay? currentTimeOfDay;
    private float nextRefreshTime;

    public void Configure(SpriteRenderer renderer, TMP_Text text)
    {
        backgroundRenderer = renderer;
        debugText = text;
    }

    private void Start()
    {
        Refresh();
    }

    private void Update()
    {
        if (Time.unscaledTime < nextRefreshTime)
            return;

        Refresh();
    }

    private void Refresh()
    {
        DateTime computerTime = DateTime.Now;
        TimeOfDay timeOfDay = GetTimeOfDay(computerTime.Hour);
        ApplyBackground(timeOfDay);

        if (debugText != null)
            debugText.text = $"{GetLabel(timeOfDay)}  |  Computer time {computerTime:HH:mm:ss}";

        if (currentTimeOfDay != timeOfDay)
        {
            currentTimeOfDay = timeOfDay;
            Debug.Log($"[TimeOfDay] Switched to {GetLabel(timeOfDay)} using computer time.");
        }

        nextRefreshTime = Time.unscaledTime + 1f;
    }

    private TimeOfDay GetTimeOfDay(int hour)
    {
        if (hour >= dayStartsAtHour && hour < duskStartsAtHour)
            return TimeOfDay.Day;

        if (hour >= duskStartsAtHour && hour < nightStartsAtHour)
            return TimeOfDay.Dusk;

        return TimeOfDay.Night;
    }

    private void ApplyBackground(TimeOfDay timeOfDay)
    {
        if (backgroundRenderer == null)
            return;

        Sprite sprite = timeOfDay == TimeOfDay.Day
            ? daySprite
            : timeOfDay == TimeOfDay.Dusk
                ? duskSprite
                : nightSprite;

        // Keep the current art visible until the corresponding replacement sprite is assigned.
        if (sprite != null)
            backgroundRenderer.sprite = sprite;
    }

    private static string GetLabel(TimeOfDay timeOfDay)
    {
        switch (timeOfDay)
        {
            case TimeOfDay.Day:
                return "DAY";
            case TimeOfDay.Dusk:
                return "DUSK";
            default:
                return "NIGHT";
        }
    }
}
