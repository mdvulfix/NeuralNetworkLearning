using System;
using System.Collections.Generic;
using UnityEngine;

using APP.Handler;

namespace APP.Brain
{
    [Serializable]
    public class Neuron : MonoBehaviour, IConfigurable
    {
        [SerializeField] private float m_NeuronSize;
        
        private float m_NeuronSizeDefault = 1f;
        private float m_DendriteWidtDefault = 0.5f;
        private int m_DendriteNumbre = 1;
        
        [SerializeField] private Axon m_Axon;
        [SerializeField] private List<Dendrite> m_Dendrites;
        
        private GameObject m_GameObject;
        private Transform m_Transform;
        private CircleCollider2D m_Collider;
        private SpriteRenderer m_Renderer;

        private Color m_ColorDefault = Color.white;
        private Color m_ColorHover = Color.green;


        public virtual void Configure(params object[] args)
        {
            m_GameObject = gameObject;
            
            if(m_GameObject.TryGetComponent<Transform>(out m_Transform) == false)
                m_Transform = m_GameObject.AddComponent<Transform>();

            if(m_GameObject.TryGetComponent<SpriteRenderer>(out m_Renderer) == false)
            {
                
                m_Renderer = m_GameObject.AddComponent<SpriteRenderer>();
                m_Renderer.sprite = HandlerSprite.Circle;
            }
                

            if(m_GameObject.TryGetComponent<CircleCollider2D>(out m_Collider) == false)
            {
                m_Collider = m_GameObject.AddComponent<CircleCollider2D>();
                m_Collider.radius = m_NeuronSizeDefault / 2;
                m_Collider.offset = Vector2.zero;
            }

            
            m_NeuronSize = m_NeuronSizeDefault;
            
            var neuronPosition = transform.position;

            var axonHead = neuronPosition;
            var axonTail = new Vector3(neuronPosition.x - 1, neuronPosition.y, neuronPosition.z);
            var axonWidth = m_NeuronSizeDefault / 2;

            m_Axon = Axon.Get(axonHead, axonTail, axonWidth);
            
            m_Dendrites = new List<Dendrite>();
            
            for (int i = 0; i < m_DendriteNumbre; i++)
            {
                var dendriteHead = neuronPosition;
                var dendriteTail = new Vector3(neuronPosition.x - 1, neuronPosition.y, neuronPosition.z);
                var dendriteWidth = m_DendriteWidtDefault / m_DendriteNumbre;
                
                
                m_Dendrites.Add(Dendrite.Get(dendriteHead, dendriteTail, dendriteWidth));

            }
        }

        private void Awake() =>
            Configure();
    }
}