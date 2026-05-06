using UnityEngine;
using UnityEngine.EventSystems;

public class MouseClick_Debuger : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private bool m_debug = false;

    // Detect if a click occurs
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!m_debug)
            return;

        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            Debug.Log($"[MouseClick_Debuger] Clicked: {clickedObject.name}", clickedObject);
        }
    }
}