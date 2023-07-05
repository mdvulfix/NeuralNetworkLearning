using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DebugUICircle : MonoBehaviour
{

    [SerializeField] private Vector3 m_Scale;

    private float m_Padding = 1.2f;
    private float m_Spacing = 1.2f;


    public void Awake()
    {
        m_Scale = transform.localScale;
    }

    public void SetPosition(Vector3 position)
    {
        var objRectTransform = GetComponent<RectTransform>();
        var objSize = new Vector3(objRectTransform.rect.width, objRectTransform.rect.height, 0);

        var canvasObj = transform.parent.gameObject;
        var canvasRectTransform = canvasObj.GetComponent<RectTransform>();
        var canvasSize = new Vector3(canvasRectTransform.rect.width, canvasRectTransform.rect.height, 0);

        objRectTransform.position = new Vector3(m_Padding * objSize.x + position.x * objSize.x * m_Spacing,
                                                m_Padding * objSize.y + position.y * objSize.y * m_Spacing,
                                                0);

    }
}
