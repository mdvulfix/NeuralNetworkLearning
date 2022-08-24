using System;
using System.Collections;

namespace APP
{
    public static class HandlerAsync
    {
        public static void Execute(IEnumerator operationAsync)
        {
            
            
            using (var controller = AsyncController.Get(new ConfigAsyncController()))
            {
                var awaiter = controller.GetAwaiter();
                //controller.ExecuteAsync(awaiter, operationAsync((Action) => awaiter.Stop()));


            }


        }
    
    }

  


}