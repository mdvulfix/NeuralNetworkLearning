using System.Collections.Generic;
using UnityEngine;

namespace APP.Brain
{
    public class Сharge : MonoBehaviour
    {

        [SerializeField] private float m_Energy;
        private float m_EnergyDefault = 100f;

        public float Energy => m_Energy;
        //private float m_Lifetime = 100;
        //private List<Vector3> m_Path;

        public void SetEnergy() =>
            SetEnergy(m_EnergyDefault);

        public void SetEnergy(float energy)
        {
            m_Energy = energy;
        }

        public static Сharge Get()
        {
            var obj = new GameObject("Сharge");
            return obj.AddComponent<Сharge>();
        }

    }

}