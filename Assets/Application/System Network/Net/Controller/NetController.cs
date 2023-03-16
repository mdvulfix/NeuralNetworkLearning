using System;
using System.Collections;
using UnityEngine;


namespace APP.Network
{
    [Serializable]
    public class NetController: ModelController, INetController
    {
       
        [SerializeField] private INet m_Net;

        private Color m_ColorBackground = Color.grey;
        private Color m_ColorActive = Color.green;
        
        public INode NodeActivated { get; private set; }


        public NetController() { }
        public NetController(params object[] args)
            => Configure(args);
        
        public override void Configure(params object[] args)
        {
            
            var config = (NetControllerConfig)args[PARAMS_Config];
            m_Net = config.Net;
            
            base.Configure(args);
        }

        public override void Init()
        {
            m_Net.Init();
            m_Net.Activate();
            base.Init();
        }

        public override void Dispose()
        {
            m_Net.Dispose();
            base.Dispose();
        }

        //public void PixelColorize(Color color) =>
        //    PixelColorize(color, Picture.PixelActive);
        
        //public void Colorize(IPixel pixel, Color color, ColorMode mode = ColorMode.None) =>
        //    pixel.SetColor(color);

        /*
        public IEnumerator AwaitLoadingAsync(IActivable activatable, float awaiting = 5f)
        {
            while (activatable.IsActivated == false && awaiting > 0)
            {
                yield return new WaitForSeconds(1);
                awaiting -= Time.deltaTime;
            }
        }
        */
        public static NetController Get(params object[] args)
            => Get<NetController>(args);


    }

    public interface INetController : IController
    {
        INode NodeActivated { get; }

        
        //void Colorize(IPixel pixel, Color color, ColorMode mode = ColorMode.None);
    }

    public struct NetControllerConfig
    {
        public NetControllerConfig(INet net)
        {
            Net = net;
        }

        public INet Net { get; private set; }

        
    }
}
