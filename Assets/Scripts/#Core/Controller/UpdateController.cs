using System.Collections.Generic;

namespace APP
{
    public class UpdateController
    {
        private UpdateControllerConfig m_Config;
        
        private static List<IUpdateble> m_Updatebles;
        

        public virtual void Configure(params object[] args)
        {
            if(args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if(arg is UpdateControllerConfig)
                    {
                        m_Config = (UpdateControllerConfig)args[0];
                        
                    }
                }
            }

            m_Updatebles = new List<IUpdateble>();
        }


        public virtual void Init()
        {
 
        }

        public virtual void Dispose()
        {

        }


        public static void SetUpdateble(IUpdateble updateble)
        {
            m_Updatebles.Add(updateble);
        }

        public static void RemoveUpdateble(IUpdateble updateble)
        {
            if(m_Updatebles.Contains(updateble))
                m_Updatebles.Remove(updateble);
        }
        
        
        
        public void Update()
        {
            foreach (var instance in m_Updatebles)
            {
                instance.Update();
            }
        }
    }

    public class UpdateControllerConfig
    {

    }

    public interface IUpdateble
    {
        void Update();

    }



}