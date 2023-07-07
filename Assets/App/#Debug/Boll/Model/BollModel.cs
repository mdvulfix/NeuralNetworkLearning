using System;
using System.Collections;
using UnityEngine;


namespace APP.Test
{
    public abstract class BollModel : ModelCacheable
    {

        public abstract void SetColor(Color color);

        //public IEnumerator SetColorAsync(Color color, float delay)
        public IEnumerator SetColorAsync(Action<bool> callback)
        {

            var delay = UnityEngine.Random.Range(1f, 5f);
            yield return new WaitForSeconds(delay);
            SetColor(Color.yellow);

            callback.Invoke(true);

        }

    }
}