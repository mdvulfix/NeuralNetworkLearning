using System;
using System.Collections;
using UnityEngine;


namespace APP
{


    public abstract class AScenable : MonoBehaviour
    {

        [SerializeField] private bool m_IsActive;

        protected static Transform ROOT;
        protected static Transform ROOT_POOL;

        public GameObject GameObject => gameObject;
        public Transform Transform => gameObject.transform;

        public ICache Cache { get; protected set; }

        public virtual void Configure(params object[] args) { }
        public virtual void Init() { }
        public virtual void Dispose() { }


        public virtual void Activate()
        {
            transform.SetParent(ROOT);
            transform.position = Vector3.zero;

            m_IsActive = true;
            gameObject.SetActive(m_IsActive);

            ($"{this.GetName()} {this.GetHashCode()} active: {m_IsActive}").Send();

        }

        public IEnumerator ActivateAsync(Action<bool> callback)
        {
            Activate();
            callback.Invoke(m_IsActive);
            yield return null;
        }


        public virtual void Deactivate()
        {
            transform.SetParent(ROOT_POOL);
            transform.position = Vector3.zero;

            m_IsActive = false;
            gameObject.SetActive(m_IsActive);

            ($"{this.GetName()} {this.GetHashCode()} active: {m_IsActive}").Send();
        }

        public IEnumerator DeactivateAsync(Action<bool> callback)
        {
            Deactivate();
            callback.Invoke(m_IsActive);
            yield return null;
        }

        // UNITY //
        private void Awake()
            => Configure();

        private void OnEnable()
            => Init();

        private void OnDisable()
            => Dispose();
    }


    public interface IScenable: ICacheable
    {
        GameObject GameObject { get; }
        Transform Transform { get; }
        
        void Activate();
        IEnumerator ActivateAsync(Action<bool> callback);

        void Deactivate();
        IEnumerator DeactivateAsync(Action<bool> callback);
    }


}

