using UnityEngine;

namespace APP
{
    public class AwaiterController : AConfigurableOnAwake, IConfigurable
    {
        [SerializeField] private IAwaiter m_Awaiter;
        [SerializeField] private bool m_SetActive = false;

        public override void Init()
        {
            base.Init();
            m_Awaiter.Activate();
            
        }
            

        public override void Dispose()
        {
            m_Awaiter.Deactivate();
            base.Dispose();
        }
            


        public void Update()
        {
            //if (m_SetActive == true && m_Awaiter.IsActivated == false)
            //    Activate();

            //else if (m_SetActive == false && m_Awaiter.IsActivated == true)
            //     Deactivate();

            //else
            //    return;
        }


    }
}

