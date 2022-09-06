using System;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Brain
{
    [Serializable]
    public class Axon : NerveModel, IAxon
    {

        public Axon() { }
        public Axon(params object[] args)
            => Configure(args);

        public Vector3 Position {get; private set; }

        public override void Configure(params object[] args)
        {
            
            
            base.Configure(args);
        }

        public void Excite()
        {
            
        }

        public override void Init()
        {

            
            base.Init();
        }
    }

    public interface IAxon: INerve, ISensible
    {
        
    }
}