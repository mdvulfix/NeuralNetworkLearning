using UnityEngine;

namespace APP
{
    public class AwaiterController : AConfigurableOnScene
    {
        [SerializeField] private Awaiter m_Awaiter;
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
            if (m_SetActive == true && m_Awaiter.IsActive == false)
                Activate();

            else if (m_SetActive == false && m_Awaiter.IsActive == true)
                Deactivate();

            else
                return;
        }


    }
}

