using FD.AI.Node;
using FD.Dev;
using FD.Program.Editer.SO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FD.AI
{

    public class FAED_AI : MonoBehaviour
    {

        [SerializeField] private FAED_DialougeContainer data;
        [SerializeField] private FAED_StateMachine machine; 
        
        public void Setting()
        {

            machine = new FAED_StateMachine(data);

        }

        public void ResetAI()
        {

            machine = null;

        }

        void Update()
        {

            machine.AIActionAll();

        }

        public void ChangeState(string value)
        {

            machine.ChangeState(value);

        }

        public void ChangeBoolState(string name, bool boolState)
        {

            machine.ChangeBoolState(name, boolState);

        }

        public string GetCurrentState()
        {

            return machine.CurrentState;

        }

    }

}


