using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace APP.Test
{
    public class TestDelegate : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

            
            var multiply = new Multiply();
            var divide = new Divide();
            
            CalculationAsync(10, 2, multiply.Execute());
            CalculationAsync(10, 2, divide.Execute());

            
            var spawnerIsReady = new Spawn(State.IsReady);
            var spawnerIsBusy= new Spawn(State.IsBusy);
            
            MethodAsync(spawnerIsReady.Create, Callback);
            MethodAsync(spawnerIsBusy.Create, Callback);
        
        
        }

        public void CalculationAsync(int x, int y,  CalculateAsync func)
        {
            int z = func(x, y, (result) => Debug.Log($"Calculation done! Result = { result }"));
        }

        public void MethodAsync(FuncAsync func, Action<bool> callback)
        {
            func(callback);
        }

        public void Callback(bool result)
        {
            Debug.Log($"Callback: Result: { result }");
        }

    }



    public delegate void FuncAsync(Action<bool> callback);
    public delegate int CalculateAsync(int x, int y, Action<int> callback);

    
    public class Spawn
    {
        public Spawn(State state)
        {
            State = state;
        }

        public State State {get; set;}
        
        public void Create(Action<bool> callback)
        {
            switch (State)
            {
                default:
                    break;

                case State.IsReady:
                    Debug.Log($"Metod: Creating is finished...");
                    callback.Invoke(true);
                    break;

                case State.IsBusy:
                    Debug.Log($"Metod: Creating is in progress...");
                    callback.Invoke(false);
                    break;
            
            }
        }
    }

    public enum State
    {
        None,
        IsReady,
        IsBusy
    }




    

    public class Multiply: Operation
    {
        public Multiply()
        {
            CalculateAsync = MultiplyAsync;
        }

        private int MultiplyAsync(int x, int y, Action<int> callback)
        {
            var z = x * y;
            callback.Invoke(z);
            return z;
        
        }

    }
    
    public class Divide: Operation
    {
        public Divide()
        {
            CalculateAsync = DivideAsync;
        }

        private int DivideAsync(int x, int y, Action<int> callback)
        {
            var z = x / y;
            callback.Invoke(z);
            return z;
        }

    }
    
    public abstract class Operation
    {
        public CalculateAsync CalculateAsync {get; protected set; }

        public CalculateAsync Execute() => 
            CalculateAsync;

    }


}


