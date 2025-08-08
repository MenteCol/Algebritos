using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler
{
    public DragDropManager dragDropManager;
    public Image highlightImage;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject != null)
        {
            Option option = droppedObject.GetComponent<Option>();

            if (option != null)
            {
                droppedObject.transform.SetParent(transform);
                droppedObject.transform.localPosition = Vector3.zero;
                dragDropManager.CheckAnswer(option);
            }
        }
    }

    public void Blink(Color color)
    {
        StartCoroutine(BlinkEffect(color));
    }

    private System.Collections.IEnumerator BlinkEffect(Color color)
    {
        Color original = highlightImage.color;
        highlightImage.color = color;
        yield return new WaitForSeconds(0.5f);
        highlightImage.color = original;
    }
}
