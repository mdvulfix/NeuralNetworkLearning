using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace APP.Brain
{
    public class NeuronController
    {
        
        
        
        
        private NeuronControllerConfig m_Config;
        
        private IEnumerable m_NeuronArr;
        

        public virtual void Configure(params object[] args)
        {
            if(args.Length > 0)
                foreach (var arg in args)
                    if(arg is NeuronControllerConfig)
                    {
                        m_Config = (NeuronControllerConfig)arg;
                        m_NeuronArr = m_Config.NeuronArr;
                    }    
        }

        public virtual void Init() { }
        public virtual void Dispose() { }


        private void InteractionsCalculate()
        { 
            
        }
    }

    public class NeuronControllerConfig
    {
        public IEnumerable NeuronArr { get; internal set; }
    }
}