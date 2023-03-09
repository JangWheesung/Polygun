using FD.Program.Editer.Runtime.Data;
using FD.Program.Editer.SO;
using FD.Program.Type;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace FD.AI.Node
{

    [System.Serializable]
    internal abstract class FAED_Node
    {

        public abstract List<string> state { get; set; }
        public abstract string name { get; set; }
        public abstract bool able { get; set; }
        public abstract string GUID { get; set; }

        public abstract void NodeAction();
        public abstract void SetState(List<string> value);

        public void ChangeAble(bool value)
        {

            able = value;

        }

    }

    [System.Serializable]
    internal class FAED_AIActionNode : FAED_Node
    {

        [SerializeField] private UnityEvent actionEvent;
        [SerializeField] private string showingName;

        [field:SerializeField] public override List<string> state { get; set; } = new List<string>();
        [field:SerializeField, HideInInspector] public override string name { get; set; }
        [field: SerializeField, HideInInspector] public override string GUID { get; set; }
        [field: SerializeField, HideInInspector] public override bool able { get; set; } = true;


        public override void NodeAction()
        {

            if (!able) return;

            actionEvent?.Invoke();

        }

        public override void SetState(List<string> value)
        {

            value.ForEach(x =>
            {

                state.Add(x);

            });

        }

        public FAED_AIActionNode(string name, string GUID)
        {

            this.name = name;
            this.GUID = GUID;
            showingName = name;

        }

    }

    [System.Serializable]
    internal class FAED_AIBoolNode : FAED_Node
    {

        [HideInInspector] public List<string> tnodes = new List<string>();
        [HideInInspector] public List<string> fnodes = new List<string>();
        [field: SerializeField] public override List<string> state { get; set; } = new List<string>();
        [field:SerializeField] public override string name { get; set; }
        [field:SerializeField, HideInInspector] public override bool able { get; set; } = true;
        [field:SerializeField, HideInInspector] public override string GUID { get; set; }

        public bool boolState;

        public void ChangeState(bool value)
        {

            if (!able) return;

            boolState = value;

        }

        public FAED_AIBoolNode(string name, string GUID)
        {

            this.name = name;
            this.GUID = GUID;

        }

        public override void SetState(List<string> value)
        {
            
            value.ForEach(x =>
            {

                state.Add(x);

            });

        }


        public void SetList(List<string> trueNodes, List<string> falseNodes)
        {


            tnodes = trueNodes;
            fnodes = falseNodes;

        }

        public override void NodeAction()
        {

        }
    }

    [System.Serializable]
    internal class FAED_AICore
    {
        
        //불 노드는 값만 저장 실질적 기능은 Core에서 실행하도록 만들자
        [SerializeField] public List<FAED_AIActionNode> actionNodes = new List<FAED_AIActionNode>();
        [SerializeField] public List<FAED_AIBoolNode> boolNodes = new List<FAED_AIBoolNode>();

        public FAED_AICore(FAED_DialougeContainer data)
        {

            #region 엑션노드 세팅

            data.editorGrahpData.Where(x => x.nodeType == FAED_AINodeType.Action).ToList().ForEach((y) =>
            {

                actionNodes.Add(new FAED_AIActionNode(y.dialogueText, y.Guid));

            });

            #endregion

            #region 불노드 세팅

            data.editorGrahpData.Where(x => x.nodeType == FAED_AINodeType.BoolNode).ToList().ForEach((y) =>
            {

                boolNodes.Add(new FAED_AIBoolNode(y.dialogueText, y.Guid));

            });

            #endregion

            #region 불노드 연결

            boolNodes.ForEach(x =>
            {

                var targetNodeTrue = data.links.Where(y => y.portName == "true" && y.baseNodeGuid == x.GUID).ToList();
                var targetNodeFalse = data.links.Where(y => y.portName == "false" && y.baseNodeGuid == x.GUID).ToList();

                x.SetList(targetNodeTrue.Select(y => y.targetNodeGuid).ToList(), targetNodeFalse.Select(z => z.targetNodeGuid).ToList());

            });

            #endregion


        }

        public void BoolNodeAction(string currentState, FAED_DialougeContainer data)
        {

            boolNodes.Where(x => x.state.Find(y => y == currentState) != null).ToList().ForEach(y =>
            {

                y?.fnodes?.ForEach(z => actionNodes?.Find(k => k.GUID == z)?.ChangeAble(!y.boolState));
                y?.fnodes?.ForEach(z => boolNodes?.Find(k => k.GUID == z)?.ChangeAble(!y.boolState));
                y?.tnodes?.ForEach(z => actionNodes?.Find(k => k.GUID == z)?.ChangeAble(y.boolState));
                y?.tnodes?.ForEach(z => boolNodes?.Find(k => k.GUID == z)?.ChangeAble(y.boolState));

            });

        }

    }

    [System.Serializable]
    internal class FAED_StateMachine
    {

        [SerializeField] private FAED_AICore core;

        [SerializeField, HideInInspector] private string currentState;
        [SerializeField, HideInInspector] private List<string> states; 
        [SerializeField, HideInInspector] private FAED_DialougeContainer data;

        public string CurrentState => currentState;

        public FAED_StateMachine(FAED_DialougeContainer data)
        {

            this.data = data;

            core = new FAED_AICore(data);

            var thisGuid = data.editorGrahpData.Find(x => x.nodeType == FAED_AINodeType.StateMachine).Guid;
            var states = data.links.Where(x => x.baseNodeGuid == thisGuid).ToList();
            this.states = states.Select(x => x.portName).ToList();

            currentState = states.Find(x => x.portCount == 0).portName;

            foreach(var state in states)
            {

                var targetNode = data.links.Where(x => x.portName == state.portName).ToList();

                foreach(var item in targetNode)
                {

                    if(core.actionNodes.Find(x => x.GUID == item.targetNodeGuid) != null)
                    {

                        if (data.editorGrahpData.Find(x => x.Guid == item.targetNodeGuid).nodeType == FAED_AINodeType.Action)
                        {

                            core.actionNodes.Find(x => x.GUID == item.targetNodeGuid).SetState(new List<string> { item.portName });

                        }

                    }
                    else if (core.boolNodes.Find(x => x.GUID == item.targetNodeGuid) != null)
                    {

                        if (data.editorGrahpData.Find(x => x.Guid == item.targetNodeGuid).nodeType == FAED_AINodeType.BoolNode)
                        {

                            core.boolNodes.Find(x => x.GUID == item.targetNodeGuid).SetState(new List<string> { item.portName });

                        }

                    }

                }

            }

            core.boolNodes.ForEach(x =>
            {

                x.tnodes.ForEach(y =>
                {

                    core.actionNodes.Find(z => z.GUID == y).SetState(x.state);

                });

                x.fnodes.ForEach(y =>
                {

                    core.actionNodes.Find(z => z.GUID == y).SetState(x.state);

                });

            });

        }

        public void AIActionAll()
        {

            core.BoolNodeAction(currentState, data);
            core.actionNodes.Where(x => x.state.Find(x => x == currentState) != null).ToList().ForEach(y => y.NodeAction());
            ///
        }

        public void ChangeState(string value)
        {

            if(states.Find(x => x == value) != null)
            {

                currentState = value;

            }

        }

        public void ChangeBoolState(string name, bool boolState)
        {

            core.boolNodes.FindAll(x => x.name == name).ForEach(y => y.boolState = boolState);

        }

    }

}
