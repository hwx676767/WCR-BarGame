using UnityEngine;
using UnityEngine.EventSystems;

public class AlcoholButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField, Min(1f)]
    private float hoverScale = 1.12f;

    private Vector3 restingScale;
    private bool isHovering;

    private void Awake()
    {
        CaptureRestingScale();
    }

    private void OnEnable()
    {
        if (!isHovering)
            CaptureRestingScale();
    }

    private void OnDisable()
    {
        ResetScale();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        transform.localScale = restingScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        ResetScale();
    }

    private void CaptureRestingScale()
    {
        restingScale = transform.localScale;
    }

    private void ResetScale()
    {
        transform.localScale = restingScale;
    }
}
