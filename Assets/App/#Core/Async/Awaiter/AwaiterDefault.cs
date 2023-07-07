using System;
using System.Collections;
using UnityEngine;

namespace APP
{

    public class AwaiterDefault : AwaiterModel, IAwaiter
    {
        public AwaiterDefault() { }
        public AwaiterDefault(params object[] args)
            => Configure(args);

        public override void Configure(params object[] args)
        {
            if (IsConfigured == true)
                return;

            if (args.Length > 0)
            {
                base.Configure(args);
                return;
            }

            var label = $"Awaiter {this.GetHashCode()}";
            var parent = new GameObject("Async").transform;
            var config = new AwaiterConfig(this, label, parent);

            base.Configure(config);
            Send($"{this.GetName()} was configured by default!");
        }



        public override void Init()
        {
            if (IsInitialized == true)
                return;


            base.Init();
        }


    }




    public partial class AwaiterFactory : Factory<IAwaiter>, IFactory
    {
        private AwaiterDefault GetAwaiterDefault(params object[] args)
        {
            var prefabPath = $"{AwaiterModel.PREFAB_Folder}/{AwaiterModel.PREFAB_Label}";
            var prefab = Resources.Load<GameObject>(prefabPath);

            var obj = (prefab != null) ?
            GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) :
            new GameObject("Awaiter");

            obj.SetActive(false);

            var instance = obj.AddComponent<AwaiterDefault>();
            obj.name = $"Awaiter";

            if (args.Length > 0)
            {
                var config = (AwaiterConfig)args[AwaiterModel.PARAMS_Config];
                instance.Configure(config);
            }

            return instance;
        }
    }
}

namespace APP
{

    public partial class SceneRoot
    {
        public static Transform AWAITER;
        public static Transform AWAITER_POOL;

        public SceneRoot()
        {
            if (AWAITER == null)
                AWAITER = new GameObject("Awaiters").transform;

            if (AWAITER_POOL == null)
            {
                AWAITER_POOL = new GameObject("Pool").transform;
                AWAITER_POOL.transform.SetParent(AWAITER);
            }
        }
    }
}

