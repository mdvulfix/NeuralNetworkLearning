using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Network
{
    public class NodeControllerDefault: ModelController, INodeController
    {
        private NodeControllerConfig m_Config;
        
        private INode[,,] m_LayerInput;
        private INode[,,] m_LayerAnalyze;

        private List<INode> m_Nodes;
        

        public NodeControllerDefault() { }
        public NodeControllerDefault(params object[] args)
            => Configure(args);


        public override void Configure(params object[] args)
        {
            if(VerifyOnConfigure())
                return;
            
            m_Config = args.Length > 0 ?
            (NodeControllerConfig)args[PARAMS_Config] :
            default(NodeControllerConfig);

            base.Configure(args);
        }

        public override void Init()
        {
            if(VerifyOnInit())
                return;

            m_Nodes = new List<INode>(100);
            
            base.Init();
        }
        
        public override void Dispose()
        {


            base.Dispose();
        }
        
        
        
        public void SetLayerInput(INode[,,] layer)
        {
            m_LayerInput = layer;
            
            foreach (var node in layer)
                m_Nodes.Add(node);
        }
        
        public void SetLayerAnalyze(INode[,,] layer)
        {
            m_LayerAnalyze = layer;
            
            foreach (var node in layer)
                m_Nodes.Add(node);
        }

        
        public void Activate()
        {
            foreach (var node in m_Nodes)
                node.Activate();
        
        }

        public void Deactivate()
        {
            foreach (var node in m_Nodes)
                node.Deactivate();
        }
   
    
    
        public static NodeControllerDefault Get(params object[] args)
            => Get<NodeControllerDefault>(args);

    }

    public interface INodeController: IController, IActivable
    {
        void SetLayerInput(INode[,,] layer);       
        void SetLayerAnalyze(INode[,,] layer);
    }

    public class NodeControllerConfig
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