using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUICircleController : MonoBehaviour
{
    
    [SerializeField] private GameObject m_CirclesPrefab;
    [SerializeField] private DebugUICircle[] m_Circles;

    private Vector3 m_CanvasSize;
    
    
    private void Awake()
    {
        
        var parent = transform;
        m_Circles = new DebugUICircle[10]
        {
            GetCircle(new Vector3(0, 0, 0), parent),
            GetCircle(new Vector3(0, 1, 0), parent),
            GetCircle(new Vector3(0, 2, 0), parent),
            GetCircle(new Vector3(0, 3, 0), parent),
            GetCircle(new Vector3(0, 4, 0), parent),
            
            GetCircle(new Vector3(1, 0, 0), parent),
            GetCircle(new Vector3(1, 1, 0), parent),
            GetCircle(new Vector3(1, 2, 0), parent),
            GetCircle(new Vector3(1, 3, 0), parent),
            GetCircle(new Vector3(1, 4, 0), parent)
        };
    }

    private DebugUICircle GetCircle(Vector3 position, Transform parent)
    {

        var obj = Instantiate<GameObject>(m_CirclesPrefab, Vector3.zero, Quaternion.identity);
        obj.transform.SetParent(parent);
        var circle = obj.GetComponent<DebugUICircle>();
        circle.SetPosition(position);
        return circle;
    }




}
