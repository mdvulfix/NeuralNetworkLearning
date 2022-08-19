using System;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Brain
{
    [Serializable]
    public class Dendrite : Nerve<Dendrite>, IConfigurable
    {

        private List<Branch> m_Branches;
        private int m_BranchNumber = 1;
        
        
        public Dendrite() { }
        public Dendrite(Vector3 head, Vector3 tail, float width) =>
            Configure(head, tail, width);

        public override void Configure(params object[] args)
        {
            base.Configure(args);
            
            m_Branches = new List<Branch>();
            
            for (int i = 0; i < m_BranchNumber; i++)
            {
                var dendriteTailPosition = Tail;

                var branchHead = dendriteTailPosition;
                var branchTail = new Vector3(dendriteTailPosition.x - 1, dendriteTailPosition.y, dendriteTailPosition.z);
                var branchWidth = Width / m_BranchNumber;
                
                
                m_Branches.Add(Branch.Get(branchHead, branchTail, branchWidth));
            }
        }

    }
}