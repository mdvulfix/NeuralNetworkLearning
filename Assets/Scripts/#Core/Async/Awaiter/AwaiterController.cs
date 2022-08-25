using UnityEngine;

namespace APP
{
    public class AwaiterController : MonoBehaviour
    {
        [SerializeField] private Awaiter m_Awaiter;
        [SerializeField] private bool m_SetActive = false;

        public void Activate() =>
            StartCoroutine(m_Awaiter.ActivateAsync(m_Awaiter.Callback));

        public void Deactivate() =>
            StartCoroutine(m_Awaiter.DeactivateAsync(m_Awaiter.Callback));


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

