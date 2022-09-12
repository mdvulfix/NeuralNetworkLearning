using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using APP.Brain;

namespace APP.Draw
{
    [Serializable]
    public abstract class PictureModel : ModelCacheable
    {
        
        
        private PictureConfig m_Config;
        
        [SerializeField] private int m_Width;
        [SerializeField] private int m_Height;

        private IPixel[,] m_Matrix;
        private List<IPixel> m_Pixels;

        [SerializeField] private Color m_BackgroundColor = Color.black;
        [SerializeField] private Color m_HoverColor = Color.grey;

        private int m_LayerMask;

        public IPicture Instance {get; private set; }
        public IPixel PixelActive => m_Pixels.Where(pixel => pixel.IsActivated == true).First();

        public static readonly string PREFAB_Folder = "Prefab";

        // CONFIGURE //
        public override void Configure(params object[] args)
        {
            if(VerifyOnConfigure())
                return;
            
            m_Config = args.Length > 0 ?
            (PictureConfig)args[PARAMS_Config] :
            default(PictureConfig);

            m_Width = m_Config.Width;
            m_Height = m_Config.Height;
            m_BackgroundColor = m_Config.BackgroundColor;
            m_HoverColor = m_Config.HoverColor;
            
            m_LayerMask = m_Config.LayerMask;

             
            if(m_Config.Parent != null)
                transform.SetParent(m_Config.Parent);
            
            base.Configure(args);
        }

        public override void Init()
        {
            if(VerifyOnInit())
                return;
            
            m_Matrix = new IPixel[m_Width, m_Height];
            m_Pixels = new List<IPixel>();
                        
            for (int x = 0; x < m_Width; x++)
            {
                for (int y = 0; y < m_Height; y++)
                {
                    var position = new Vector3(x - m_Width / 2, y - m_Height / 2);
                    
                    var pixel = GetPixel(position);
                    pixel.Init();

                    m_Pixels.Add(m_Matrix[x, y] = pixel);
                }
            }

            
            base.Init();
        }

        public override void Dispose()
        {
            foreach (var pixel in m_Pixels)
                pixel.Dispose();

            m_Pixels.Clear();

            base.Dispose();
        }


        public override void Activate()
        {
            foreach (var pixel in m_Pixels)
                pixel.Activate(); 

            base.Activate();  
        }

        public override void Deactivate()
        {
            foreach (var pixel in m_Pixels)
                pixel.Deactivate(); 

            base.Deactivate();  
        }

        
        public abstract IPixel GetPixel(Vector3 position);

        
        public IEnumerable<ISensible> GetSensibles() =>
            m_Pixels;

        // FACTORY //
        public static TPicture Get<TPicture>(params object[] args)
        where TPicture: IPicture
        {
            IFactory factoryCustom = null;
            
            if(args.Length > 0)
                try{ factoryCustom = (IFactory)args[PARAMS_Factory]; } catch { Debug.Log("Custom factory not found! The instance will be created by default."); }

            
            var factory = (factoryCustom != null) ? factoryCustom : new PictureFactory();
            var instance = factory.Get<TPicture>(args);
            
            return instance;
        }
    }

    public struct PictureConfig
    {
        public PictureConfig(int width, int height, Color backgroundColor, Color hoverColor, int layerMask, Transform parent)
        {
            Width = width;
            Height = height;
            BackgroundColor = backgroundColor;
            HoverColor = hoverColor;
            LayerMask = layerMask;
            Parent = parent;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Color BackgroundColor { get; private set; }
        public Color HoverColor { get; private set; }
        public int LayerMask { get; }
        public Transform Parent { get; internal set; }
    }

    public interface IPicture: IConfigurable, ICacheable, IActivable, IMessager, IRecognizable
    {
        IPixel PixelActive { get; }
    }

    public partial class PictureFactory : Factory<IPicture>
    {
        private string m_Label = "Picture";
        
        public PictureFactory()
        {
            Set<Picture2D>(Constructor.Get((args) => GetPicture2D(args)));
            Set<Picture3D>(Constructor.Get((args) => GetPicture3D(args)));
        }
    }
}