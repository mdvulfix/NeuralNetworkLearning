using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace APP.Brain
{
    public class NeuronControllerDefault: ModelController, INeuronController
    {
        private NeuronControllerConfig m_Config;
        
        private INeuron[,,] m_InputLayer;
        private INeuron[,,] m_AnalyzeLayer;

        private List<INeuron> m_Neurons;
        

        public NeuronControllerDefault() { }
        public NeuronControllerDefault(params object[] args)
            => Configure(args);


        public override void Configure(params object[] args)
        {
            if(VerifyOnConfigure())
                return;
            
            m_Config = args.Length > 0 ?
            (NeuronControllerConfig)args[PARAMS_Config] :
            default(NeuronControllerConfig);

            base.Configure(args);
        }

        public override void Init()
        {
            if(VerifyOnInit())
                return;

            m_Neurons = new List<INeuron>(100);
            
            base.Init();
        }
        
        public override void Dispose()
        {


            base.Dispose();
        }
        
        
        
        public void SetInputLayer(INeuron[,,] layer)
        {
            m_InputLayer = layer;
            
            foreach (var neuron in layer)
                m_Neurons.Add(neuron);
        }
        
        public void SetAnalyzeLayer(INeuron[,,] layer)
        {
            m_AnalyzeLayer = layer;
            
            foreach (var neuron in layer)
                m_Neurons.Add(neuron);
        }

        
        public void Activate()
        {
            foreach (var neuron in m_Neurons)
                neuron.Activate();
        
        }

        public void Deactivate()
        {
            foreach (var neuron in m_Neurons)
                neuron.Deactivate();
        }
   
        public void Update()
        {
            foreach (var neuron in m_Neurons)
                neuron.Update();
        }
    
    
        public static NeuronControllerDefault Get(params object[] args)
            => Get<NeuronControllerDefault>(args);

    }

    public interface INeuronController: IController, IActivable, IUpdateble
    {
        void SetInputLayer(INeuron[,,] layer);       
        void SetAnalyzeLayer(INeuron[,,] layer);
    }

    public class NeuronControllerConfig
    {
        /*
        public NeuronControllerConfig(IEnumerable<INeuron> inputLayer)
        {
            InputLayer = inputLayer;
        }

        public IEnumerable<INeuron> InputLayer { get; private set; }
        */
    }
}