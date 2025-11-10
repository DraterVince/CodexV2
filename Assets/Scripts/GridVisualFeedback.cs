using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridVisualFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    public Image backgroundImage;
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;

    void Awake()
    {
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
        backgroundImage.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
            backgroundImage.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        backgroundImage.color = normalColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        backgroundImage.color = normalColor;
    }
}