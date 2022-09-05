using System;
using UnityEngine;
using UInput = UnityEngine.Input;
using UCamera = UnityEngine.Camera;


namespace APP.Input
{
    [Serializable]
    public class InputController : AConfigurable, IConfigurable, IUpdateble
    {

        private readonly string FOLDER_SPRITES = "Sprite/Cursor";
        private string m_SpriteLabel = "Default";

        private UCamera m_CameraMain;

        [SerializeField] private CursorDefault m_Cursor;

        private int m_SelectableLayer;

        private ICursor Cursor => m_Cursor;


        public event Action<int, ISelectable> Selected;


        public InputController() { }
        public InputController(params object[] args)
        {
            Configure(args);
            Init();
        }

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
            m_SelectableLayer = (1 << 8);

            var sprite = Resources.Load<Sprite>($"{FOLDER_SPRITES}/{m_SpriteLabel}");
            var color = Color.cyan;
            var cursorConfig = new CursorConfig(sprite, color, null);

            m_Cursor = CursorModel.Get<CursorDefault>();
            m_Cursor.Configure(cursorConfig);
            m_Cursor.Init();



            base.Init();
        }

        public override void Dispose()
        {
            m_Cursor.Dispose();

            base.Dispose();
        }




        public void Update()
        {
            HandleSelection();





            m_Cursor.Follow(() => FollowPositionCalculate(UInput.mousePosition));
            m_Cursor.Update();

        }

        private void HandleSelection()
        {
            int buttonIndex = -1;
            bool isSelecting = false;

            if (UInput.GetMouseButton(0))
            {
                isSelecting = true;
                buttonIndex = 0;
            }
            else
            if (UInput.GetMouseButton(1))
            {
                isSelecting = true;
                buttonIndex = 1;
            }
            else
            {
                isSelecting = false;
                buttonIndex = -1;
            }


            if (isSelecting == true)
            {
                if (m_Cursor.Select(m_CameraMain, UInput.mousePosition, m_SelectableLayer, out var selectable))
                    Selected?.Invoke(buttonIndex, selectable);

                if(buttonIndex ==0)
                    m_Cursor.SetColor(Color.yellow);
                else
                if(buttonIndex ==1)
                    m_Cursor.SetColor(Color.black);
            }
            else
            {
                m_Cursor.SetColor(Color.white);
            }
        }

        private Vector3 FollowPositionCalculate(Vector3 position)
        {
            var newPosition = m_CameraMain.ScreenToWorldPoint(position);
            return new Vector3(newPosition.x, newPosition.y, -1);
        }


        private void OnPointSelected(Vector3 point)
        {
            /*
            if (Physics2D.Raycast(point, Vector3.forward))
            {
                Debug.DrawLine(point, Vector3.forward * 100, Color.yellow);
                Send($"Hit {hit.GetName()}!");
            }
            */

        }
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

    }

}