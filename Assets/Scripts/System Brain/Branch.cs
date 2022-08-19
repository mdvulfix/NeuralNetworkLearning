using System;
using UnityEngine;

namespace APP.Brain
{
    [Serializable]
    public class Branch: Nerve<Branch>, IConfigurable
    {
        public Branch() { }
        public Branch(Vector3 head, Vector3 tail, float width) =>
            Configure(head, tail, width);

        public override void Configure(params object[] args)
        {
            base.Configure(args);
            

        }

    }
}