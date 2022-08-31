using System.Collections.Generic;

namespace APP
{
    public class UpdateController: AConfigurable, IConfigurable
    {
        private static List<IUpdateble> m_Updatebles = new List<IUpdateble>();
    
        public UpdateController() { }
        public UpdateController(params object[] args)
            => Configure(args);
        
        public override void Configure(params object[] args)
        {
            var config = (UpdateControllerConfig)args[PARAM_INDEX_Config];
            
            base.Configure(args);
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