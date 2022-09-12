using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace APP
{

    public class Configurer
    {
        private IConfigurable m_Configurable;
        
        private bool m_IsConfigured;
        private Action m_Configure;
        
        
        public Configurer(IConfigurable configurable, Action configure, ref bool isConfigured)
        {
            m_Configurable = configurable;
            m_Configure = configure;
            m_IsConfigured = isConfigured;

            if(Verify()) 
                configure.Invoke();

            m_IsConfigured = true;
        }


        private bool Verify()
        {
            if (m_IsConfigured == true)
            {
                ($"Instance is already configured.").Send(true, LogFormat.Warning);
                return true;
            }

            return true;
        }
    }
}