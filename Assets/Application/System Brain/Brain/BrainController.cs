using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace APP.Brain
{
    [Serializable]
    public class BrainController : ModelController, IBrainController
    {
        private BrainControllerConfig m_Config;


        public IRecognizable Recognizable {get; private set;}
        public IBrain Brain {get; private set; }
        
        public BrainController() { }
        public BrainController(params object[] args)
            => Configure(args);


        public override void Configure(params object[] args)
        {
            if(VerifyOnConfigure())
                return;
            
            m_Config = args.Length > 0 ?
            (BrainControllerConfig)args[PARAMS_Config] :
            default(BrainControllerConfig);

            Brain = m_Config.Brain;
            Recognizable = m_Config.Recognizable;

            
            base.Configure(args);
        }

        public override void Init()
        {
            if(VerifyOnInit())
                return;

            Brain.Init();
            Brain.Activate();
            //Brain.Recognize(Recognizable);
            //Brain.Analyze();
            
            
            base.Init();
        }

        public override void Dispose()
        {
            Brain.Dispose();

            base.Dispose();
        }

        public void Update()
        {
            Brain.Update();
            //Brain.Response();
        }
        
        public static BrainController Get(params object[] args)
            => Get<BrainController>(args);

    }

    public interface IBrainController: IController, IUpdateble
    {

    }

    public class BrainControllerConfig
    {
        public BrainControllerConfig(IBrain brain, IRecognizable recognizable)
        {
            Brain = brain;
            Recognizable = recognizable;
        }

        public IBrain Brain { get; private set; }
        public IRecognizable Recognizable { get; private set; }
    }
}
