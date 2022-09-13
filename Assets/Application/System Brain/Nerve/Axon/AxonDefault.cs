using System;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Brain
{
    [Serializable]
    public class AxonDefault : NerveModel, IAxon
    {
        public static readonly string PREFAB_Label = "Axon";

        public AxonDefault() { }
        public AxonDefault(params object[] args)
            => Configure(args);

        public void Excite() { }
        protected override void Impulse() {}

    }

    public interface IAxon: INerve, ISensible
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
            obj.name = $"Axon { instance.GetHashCode() } ";

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