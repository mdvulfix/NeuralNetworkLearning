using System;
using System.Collections.Generic;
using UnityEngine;
using UInput = UnityEngine.Input;
using UCamera = UnityEngine.Camera;


namespace APP.Input
{
    [Serializable]
    public class InputController : AController, IController, IUpdateble
    {

        private UCamera m_CameraMain;

        private int m_SelectableLayer;

        private List<ISelectable> m_IsSelected;
        private ISelectable m_IsHovered;

        private Color m_PointerColorDefault;
        private Color m_PointerColorSelected;
        private Color m_PointerColorUnselected;

        private Vector3 m_PointPosition;


        public IPointer Pointer { get; private set; }


        public InputController() { }
        public InputController(params object[] args)
            => Configure(args);


        public override void Configure(params object[] args)
        {
            var config = args.Length > 0 ?
            (InputControllerConfig)args[PARAM_INDEX_Config] :
            default(InputControllerConfig);

            m_CameraMain = config.CameraMain;

            base.Configure(args);
        }

        public override void Init()
        {
            m_IsSelected = new List<ISelectable>(100);

            m_PointerColorDefault = Color.cyan;
            m_PointerColorSelected = Color.green;
            m_PointerColorUnselected = Color.black;

            m_SelectableLayer = (1 << 8);

            Pointer = PointerModel.Get<PointerDefault>();

            var pointerConfig = new PointerConfig(Pointer, m_PointerColorDefault, m_SelectableLayer);

            Pointer.Configure(pointerConfig);
            Pointer.Init();
            Pointer.Activate();

            base.Init();
        }

        public override void Dispose()
        {
            Pointer.Dispose();

            base.Dispose();
        }




        public void Update()
        {
            PointerFollowMousePosition(m_CameraMain, UInput.mousePosition);
            HandleHover();
            HandleSelect();
        }

        public void FixedUpdate()
        {

        }


        private void HandleHover()
        {
            if (GetSelectable(m_SelectableLayer, out var selectable))
            {
                if (m_IsHovered != null && m_IsHovered != selectable)
                {
                    m_IsHovered.OnHovered(false);
                    m_IsHovered = selectable;
                    m_IsHovered.OnHovered(true);
                }
            }
        }

        private void HandleSelect()
        {
            
            if (UInput.GetMouseButton(0))
            {
                Pointer.SetColor(m_PointerColorSelected);

                if (GetSelectable(m_SelectableLayer, out var selectable))
                {
                    if (m_IsSelected.Contains(selectable) == false)
                    {
                        selectable.OnSelected(true);
                        m_IsSelected.Add(selectable);
                    }
                }
            }
            else
            if (UInput.GetMouseButton(1))
            {
                Pointer.SetColor(m_PointerColorUnselected);

                if (GetSelectable(m_SelectableLayer, out var selectable))
                {
                    if (m_IsSelected.Contains(selectable) == true)
                    {
                        m_IsSelected.Remove(selectable);
                        selectable.OnSelected(false);
                    }
                }
            }
            else
            {
                Pointer.SetColor(m_PointerColorDefault);
            }
        }


        private void PointerFollowMousePosition(Camera camera, Vector3 mousePosition)
        {
            var position = camera.ScreenToWorldPoint(mousePosition);
            position = new Vector3(position.x, position.y, -1);
            Pointer.SetPosition(position);
        }

        private bool GetSelectable(int targetLayer, out ISelectable selectable)
        {
            selectable = null;

            if (Physics.Raycast(Pointer.Position, Vector3.forward, out var hit, 100, targetLayer))
            {
                if (hit.collider.TryGetComponent<ISelectable>(out selectable))
                {
                    Send($"Hit {hit.transform.name}!");
                    return true;
                }
            }

            return false;
        }




        public static InputController Get(params object[] args)
            => Get<InputController>(args);

    }

    public class InputControllerConfig
    {
        public InputControllerConfig(UCamera cameraMain)
        {
            CameraMain = cameraMain;
        }

        public UCamera CameraMain { get; internal set; }
    }

    public class Selector
    {





    }
}

namespace APP
{
    public interface ISelectable
    {
        void OnSelected(bool selected);
        void OnHovered(bool hovered);
    }

}