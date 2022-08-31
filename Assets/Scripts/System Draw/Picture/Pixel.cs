using System;
using UnityEngine;

using APP.Brain;

namespace APP
{
    [Serializable]
    public class Pixel : AConfigurableOnAwake, ISensible, IUpdateble
    {
        private Transform m_Transform;
        private BoxCollider2D m_Collider;
        private SpriteRenderer m_Renderer;


        private Color m_ColorDefault = Color.black;
        private Color m_ColorHover = Color.grey;


        public Sensor Sensor { get; private set; }


        public override void Configure(params object[] args)
        {
            m_Transform = OnSceneObject.transform;

            if (OnSceneObject.TryGetComponent<SpriteRenderer>(out m_Renderer) == false)
                m_Renderer = OnSceneObject.AddComponent<SpriteRenderer>();

            if (OnSceneObject.TryGetComponent<BoxCollider2D>(out m_Collider) == false)
            {
                m_Collider = OnSceneObject.AddComponent<BoxCollider2D>();
                m_Collider.size = Vector2.one;
                m_Collider.offset = Vector2.one / 2;
            }

            var config = (PixelConfig)args[PARAM_INDEX_Config];
            m_Renderer.sprite = config.Sprite;

            m_ColorDefault = config.ColorDefault;
            m_Renderer.color = m_ColorDefault;

            m_ColorHover = config.ColorHover;

            m_Transform.position = config.Position;
            m_Transform.parent = config.Parent.transform;
            m_Transform.name = $"Pixel ({m_Transform.position.x.ToString()}; {m_Transform.position.y.ToString()})";

        }


        public void SetColor() =>
            SetColor(m_ColorDefault);

        public void SetColor(Color color, ColorMode mode = ColorMode.None)
        {
            if (mode == ColorMode.Draw)
                m_ColorDefault = color;

            m_Renderer.color = color;
        }

        public void SetSensor(Sensor sensor)
        {
            Sensor = sensor;
        }

        public void Update()
        {
            Sensor.Excite();
        }



        private void OnMouseOver()
        {
            if (m_IsActive == false)
            {
                m_IsActive = true;
                SetColor(m_ColorHover);
            }

        }

        private void OnMouseExit()
        {
            if (m_IsActive == true)
            {
                m_IsActive = false;
                SetColor(m_ColorDefault);
            }
        }





        /*
        private void Awake()
        {
            
            var folder = "Sprites";
            var label = "box_white";
            
            var sprite = Resources.Load<Sprite>($"{folder}/{label}");
            var pixelConfig = new PixelConfig(Vector3.zero, gameObject, sprite, Color.black, Color.gray);

            Configure(pixelConfig);
        }
        */


    }







    public struct PixelConfig
    {
        public Vector3 Position { get; private set; }
        public GameObject Parent { get; private set; }
        public Sprite Sprite { get; private set; }
        public Color ColorDefault { get; private set; }
        public Color ColorHover { get; private set; }

        public PixelConfig(Vector3 position, GameObject parent, Sprite sprite, Color colorDefault, Color colorHover)
        {
            Parent = parent;
            Sprite = sprite;
            ColorDefault = colorDefault;
            ColorHover = colorHover;
            Position = position;
        }
    }

    public enum ColorMode
    {
        None,
        Draw
    }
}