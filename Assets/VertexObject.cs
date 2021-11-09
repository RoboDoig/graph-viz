using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VertexObject : MonoBehaviour, IDragHandler
{
    public int renderIndex;
    private RectTransform rectTransform;
    private Text text;
    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        text = GetComponentInChildren<Text>();
        transform.SetSiblingIndex(renderIndex);
    }

    public void SetPosition(Vector3 position) {
        rectTransform.anchoredPosition3D = position;
    }

    public Vector3 GetPosition() {
        return rectTransform.anchoredPosition3D;
    }

    public void SetText(string displayText) {
        text.text = displayText;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }
}
