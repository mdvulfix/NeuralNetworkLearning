using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Network
{
    public class NodeControllerDefault : ModelController, INodeController
    {
        private NodeControllerConfig m_Config;

        private INode[,] m_LayerRecognize;
        private INode[,] m_LayerAnalyze;
        private INode[,] m_LayerResponse;

        private List<INode> m_Nodes;

        private Vector3 m_StartPoint;

        public NodeControllerDefault() { }
        public NodeControllerDefault(params object[] args)
            => Configure(args);


        public override void Configure(params object[] args)
        {
            if (VerifyOnConfigure())
                return;

            m_Config = args.Length > 0 ?
            (NodeControllerConfig)args[PARAMS_Config] :
            default(NodeControllerConfig);

            base.Configure(args);
        }

        public override void Init()
        {
            if (VerifyOnInit())
                return;



            m_Nodes = new List<INode>(100);

            m_StartPoint = new Vector3(6, -10, 0);

            base.Init();
        }

        public override void Dispose()
        {


            base.Dispose();
        }



        public void SetLayerRecognize(INode[,] layer)
        {
            m_LayerRecognize = layer;

            var dimension = layer.GetLength(0);
            var size = layer.GetLength(1);
            var colorDefault = Color.blue;

            var offset = 0;

            for (int j = 0; j < size; j++)
            {
                for (int i = 0; i < dimension; i++)
                {

                    layer[i, j].SetPosition(new Vector3(j + offset, (dimension / 2) - i, 0) - m_StartPoint);
                    layer[i, j].SetColor(colorDefault);
                    m_Nodes.Add(layer[i, j]);
                }
            }
        }

        public void SetLayerAnalyze(INode[,] layer)
        {
            m_LayerAnalyze = layer;

            var dimension = layer.GetLength(0);
            var size = layer.GetLength(1);
            var colorDefault = Color.red;

            var offset = m_LayerRecognize.GetLength(1) + 1;

            for (int j = 0; j < size; j++)
            {
                for (int i = 0; i < dimension; i++)
                {

                    layer[i, j].SetPosition(new Vector3(j + offset, (dimension / 2) - i, 0) - m_StartPoint);
                    layer[i, j].SetColor(colorDefault);
                    m_Nodes.Add(layer[i, j]);
                }
            }
        }

        public void SetLayerResponse(INode[,] layer)
        {
            m_LayerResponse = layer;

            var dimension = layer.GetLength(0);
            var size = layer.GetLength(1);
            var colorDefault = Color.gray;

            var offset = m_LayerRecognize.GetLength(1) + m_LayerAnalyze.GetLength(1) + 2;

            for (int j = 0; j < size; j++)
            {
                for (int i = 0; i < dimension; i++)
                {

                    layer[i, j].SetPosition(new Vector3(j + offset, (dimension / 2) - i, 0) - m_StartPoint);
                    layer[i, j].SetColor(colorDefault);
                    m_Nodes.Add(layer[i, j]);
                }
            }
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

    public interface INodeController : IController, IActivable
    {
        void SetLayerRecognize(INode[,] layer);
        void SetLayerAnalyze(INode[,] layer);
        void SetLayerResponse(INode[,] layer);

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