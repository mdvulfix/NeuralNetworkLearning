using System;
using System.Collections.Generic;
using UnityEngine;


namespace APP.Test
{



    [Serializable]
    public class BollDefault : BollModel
    {

        private MeshRenderer m_Renderer;

        [SerializeField] private Color m_ColorDefault;
        [SerializeField] private Color m_Color;


        public override void Init()
        {
            m_ColorDefault = Color.black;
            m_Color = Color.yellow;

            m_Renderer = GetComponent<MeshRenderer>();


            SetColor(m_ColorDefault);



            base.Init();
        }

        private void Start()
        {
            //var delay = 3f;
            //StopCoroutine(nameof(SetColorAsync));
            //StartCoroutine(SetColorAsync(m_Color, delay));
            //StartCoroutine(SetColorAsync((state) => { if (state) Debug.Log("Boll color has been changed!"); }));
        }



        public override void SetColor(Color color)
        {
            m_Renderer.material.color = color;
        }

    }
}