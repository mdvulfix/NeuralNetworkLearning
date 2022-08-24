using System;
using System.Collections;
using UnityEngine;

namespace APP
{
    public class Cache : MonoBehaviour
    {

        [SerializeField] private bool m_IsLoaded;
        [SerializeField] private float m_LoadingTime;
        [SerializeField] private string m_Label;

        public bool IsLoaded => m_IsLoaded;

        public void Setup(string label, int loadingTime)
        {
            m_LoadingTime = loadingTime;
            m_Label = label;
        }

        public void Load()
        {
            gameObject.SetActive(true);
        }

        public IEnumerator LoadAsync(Action callback)
        {
            while (m_LoadingTime > 0)
            {
                m_LoadingTime -= Time.deltaTime;
                Debug.Log($"{m_Label}: waiting for operation complite...");
                yield return new WaitForSeconds(0.01f);
            }

            
            
            gameObject.SetActive(true);
            Debug.Log($"{m_Label} async loading is done");

            callback.Invoke();
        }

        private void OnEnable()
        {

            m_IsLoaded = true;
        }

        private void OnDisable()
        {
            m_IsLoaded = false;
        }

    }

}