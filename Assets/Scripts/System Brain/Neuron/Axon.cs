using System;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Brain
{
    [Serializable]
    public class Axon : Nerve<Axon>
    {
        private List<Branch> m_Branches;
        private int m_BranchNumber = 1;
        
        
        public Axon() { }
        public Axon(params object[] args)
            => Configure(args);

        public override void Configure(params object[] args)
        {
            
            
            base.Configure(args);
        }

        public override void Init()
        {
            m_Branches = new List<Branch>();

            for (int i = 0; i < m_BranchNumber; i++)
            {
                var tailPosition = Tail;

                var branchHead = tailPosition;
                var branchTail = new Vector3(tailPosition.x - 1, tailPosition.y, tailPosition.z);
                var branchWidth = Width / m_BranchNumber;
                
                
                m_Branches.Add(Branch.Get(branchHead, branchTail, branchWidth));
            }
            
            base.Init();
        }
    }
}