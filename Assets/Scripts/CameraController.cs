using UnityEngine;
using UInput = UnityEngine.Input;
using UCamera = UnityEngine.Camera;
using System;

using APP.Draw;

namespace APP
{

    [RequireComponent(typeof(UCamera))]
    public class CameraController : MonoBehaviour
    {

        private UCamera m_Camera;

        private void Awake()
        {
            m_Camera = GetComponent<UCamera>();
        }



        private void Update()
        {
            if (UInput.GetMouseButtonUp(0))
                Select(Color.yellow);

            if (UInput.GetMouseButtonUp(1))
                Select(Color.black);
        }



        private void Select(Color color)
        {

            var mousePositionInWorld = m_Camera.ScreenToWorldPoint(UInput.mousePosition);
            var position = new Vector3(mousePositionInWorld.x, mousePositionInWorld.y, -1);
            var layerMask = 1 << 8;
            
            RaycastHit2D hit = Physics2D.Raycast(position, Vector3.forward, 100f, layerMask);
            Debug.DrawLine(position, Vector3.forward * 100, Color.yellow);

            if (hit.collider != null)
            {
                if(hit.collider.TryGetComponent<IPixel>(out var pixel))
                {
                    Debug.Log($"Hit pixel { pixel.GetName() }! Layer: {hit.collider.gameObject.layer }");
                    pixel.SetColor(color, ColorMode.Draw);
                }
                    
            }
        }
    }
}