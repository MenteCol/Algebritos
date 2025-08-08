using UnityEngine;
using UnityEngine.EventSystems;

public class Option : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isCorrect;
    private CanvasGroup canvasGroup;
    private Vector3 startPosition;
    private Transform startParent;

    void Awake()
    {
        startPosition = transform.localPosition;
        startParent = transform.parent;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if(transform.parent == startParent)
        {
            ResetPosition();
        }
    }

    public void ResetPosition()
    {
        transform.localPosition = startPosition;
        transform.SetParent(startParent);
    }
}
