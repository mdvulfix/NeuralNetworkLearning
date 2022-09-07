using System;
using UnityEngine;

namespace APP.Input
{

    [Serializable]
    public class CursorDefault : CursorModel, ICursor
    {
        public void Follow(Func<Vector3> getPositon)
            => SetPosition(getPositon.Invoke());

    }


    public abstract class CursorModel : AConfigurable, IConfigurable, IUpdateble
    {
        private static IFactory m_Factory = new FactoryDefault();

        private GameObject m_GameObject;
        private Transform m_Transform;
        private SpriteRenderer m_Renderer;

        [SerializeField] private Vector3 m_Position;


        

        public override void Configure(params object[] args)
        {
            var config = args.Length > 0 ?
            (CursorConfig)args[PARAM_INDEX_Config] :
            default(CursorConfig);


            //m_GameObject = OnSceneObject;
            m_GameObject = new GameObject("Cursor");
            m_Transform = m_GameObject.transform;

            if (m_GameObject.TryGetComponent<SpriteRenderer>(out m_Renderer) == false)
                m_Renderer = m_GameObject.AddComponent<SpriteRenderer>();


            m_Renderer.sprite = config.Sprite;
            m_Renderer.color = config.Color;

            m_Transform.localPosition = Vector3.back;
            m_Transform.localScale = Vector3.one * 3;

            m_Position = m_Transform.localPosition;

            if (config.Parent != null)
                m_Transform.parent = config.Parent.transform;

            base.Configure(args);

        }

        public bool Select(Camera camera, Vector3 mousePosition, int targetLayer, out ISelectable selectable)
        {
            selectable = null;
            
            var mousePositionInWorld = camera.ScreenToWorldPoint(mousePosition);
            var worldPosition = new Vector3(mousePositionInWorld.x, mousePositionInWorld.y, -1);

            var hit = Physics2D.Raycast(worldPosition, Vector3.forward, 100, targetLayer);
            Debug.DrawLine(worldPosition, Vector3.forward * 100, Color.yellow);

            if (hit == true)
            {
                if (hit.collider.TryGetComponent<ISelectable>(out selectable))
                {
                    Send($"Hit {hit.transform.name}!");
                    return true;
                }
            }

            return false;
        }


        public void Update()
        {
            HandlePosition();
        }



        public void SetPosition(Vector3 position)
        {
            if (m_Position == position)
                return;

            m_Position = position;
        }

        public void SetColor(Color color)
        {
            m_Renderer.color = color;
        }


        private void HandlePosition()
        {
            if (m_Position == m_Transform.position)
                return;

            m_Transform.position = m_Position;
        }

        // FACTORY // 
        public static TCursor Get<TCursor>(params object[] args)
        where TCursor : IConfigurable
            => Get<TCursor>(null, args);

        public static TCursor Get<TCursor>(IFactory factory, params object[] args)
        where TCursor : IConfigurable
        {

            var cursorFactory = (factory == null) ? m_Factory : factory;
            var cursor = cursorFactory.Get<TCursor>();

            //var obj = new GameObject("Pixel");
            //obj.SetActive(false);
            //obj.transform.SetParent(ROOT_POOL);

            //var pixel = obj.AddComponent<TPixel>();

            return cursor;
        }


    }

    public class CursorConfig
    {
        public CursorConfig(Sprite sprite, Color color, Transform parent)
        {
            Sprite = sprite;
            Color = color;
            Parent = parent;
        }

        public Sprite Sprite { get; private set; }
        public Color Color { get; private set; }

        public Transform Parent { get; private set; }
    }

    public interface ICursor
    {

    }




}