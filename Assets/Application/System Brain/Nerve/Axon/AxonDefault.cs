using System;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace APP.Brain
{
    [Serializable]
    public class AxonDefault : NerveModel, IAxon
    {
        private LineRenderer m_LineRenderer;
        
        public static readonly string PREFAB_Label = "Axon";

        public AxonDefault() { }
        public AxonDefault(params object[] args)
            => Configure(args);

        public override void Configure(params object[] args)
        {
            if (VerifyOnConfigure())
                return;

            if (GetComponent<LineRenderer>(out m_LineRenderer) == false)
                m_LineRenderer = SetComponent<LineRenderer>();


            if (args.Length > 0)
            {
                base.Configure(args);
                return;
            }
            
            // CONFIGURE BY DEFAULT //
            var position = Vector3.zero;
            var size = URandom.Range(0f, 100f);
            var energy = URandom.Range(0f, 100f);
            var layerMask = 9;
            
            Transform parent = null;
            if (Seacher.Find<IScene>(out var scenes))
                if(scenes[0].GetComponent<Transform>(out var neuronParent))
                    parent = neuronParent != null ? neuronParent : transform.parent;

            var config = new NerveConfig(this, position, size, layerMask, parent);

            base.Configure(config);
            Send($"The instance was configured by default!");
        }


        public void Excite() { }
        protected override void Impulse() {}

        public override void UpdateBond(Color color, params Vector3[] points)
        {
            for (int i = 0; i < points.Length; i++)
                m_LineRenderer.SetPosition(i, points[i]);
 
        }

    }

    public interface IAxon: INerve
    {
        
    }

    public partial class NerveFactory : Factory<INerve>
    {
        private AxonDefault GetAxonDefault(params object[] args)
        {       
            var prefabPath = $"{NerveModel.PREFAB_Folder}/{AxonDefault.PREFAB_Label}";
            var prefab = Resources.Load<GameObject>(prefabPath);
            
            var obj = (prefab != null) ? 
            GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) : 
            new GameObject("Axon");
            
            obj.SetActive(false);

            var instance = obj.AddComponent<AxonDefault>();
            obj.name = $"Axon";

            //var instance = new Pixel3D();

            if (args.Length > 0)
            {
                var config = (NerveConfig)args[NerveModel.PARAMS_Config];
                instance.Configure(config);
            }

            return instance;
        }
    }

}