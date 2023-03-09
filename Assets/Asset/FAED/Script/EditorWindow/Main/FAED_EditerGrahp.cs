#if UNITY_EDITOR
using FD.Program.Runtime;
using FD.Program.Editer.Runtime.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using FD.Program.Type;

namespace FD.Program.UI
{

    public class FAED_AIEditerGrahpWindowModule : EditorWindow
    {

        private FAED_AIGrahpViewModlue grahpView;
        private string fileName = "New Narrative";

        [MenuItem("FAED_AI/FAED_AIGrahp")]
        public static void OpenGrahpViewWindow()
        {

            var window = GetWindow<FAED_AIEditerGrahpWindowModule>();
            window.titleContent = new GUIContent(text: "FAED_AI");

        }

        private void ConstructGrahpView()
        {

            grahpView = new FAED_AIGrahpViewModlue
            {

                name = "FAED_AI"

            };

            grahpView.StretchToParentSize();
            rootVisualElement.Add(grahpView);

        }

        //툴바
        private void GenerateToolbar()
        {

            var toolbar = new Toolbar();

            #region 세이브

            var fileNameTextField = new TextField(label:"FileName");
            fileNameTextField.SetValueWithoutNotify(fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(evt => fileName = evt.newValue);
            toolbar.Add(fileNameTextField);

            toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "SaveData"});
            toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "LoadData" });
            toolbar.Add(new Button(() => grahpView.CreateNode("BoolNode", FAED_AINodeType.BoolNode, false)) { text = "CreateBoolNode" });

            #endregion

            #region 엑션 노드 버튼

            //엑션 노드 생성버튼
            var nodeCreateButton = new Button(clickEvent: () =>
            {

                grahpView.CreateNode("Action");

            });

            nodeCreateButton.text = "CreateActionNode";
            toolbar.Add(nodeCreateButton);

            #endregion

            rootVisualElement.Add(toolbar);

        }

        private void RequestDataOperation(bool save)
        {

            if (string.IsNullOrEmpty(fileName))
            {

                EditorUtility.DisplayDialog("error", "Invalid File Name", "Ok");
                return;

            }

            var grahpSave = FAED_GrahpSave.GetInstance(grahpView);

            if (save)
            {

                grahpSave.SaveGraph(fileName);

            }
            else
            {

                grahpSave.LoadGraph(fileName);

            }

        }

        private void GenerateMiniMap()
        {

            var minimap = new MiniMap { anchored = true };
            minimap.SetPosition(new Rect(10, 30, 200, 140));
            grahpView.Add(minimap);

        }
        private void OnEnable()
        {
            FAED_AIGrahpViewModlue.portNums = new Dictionary<string, int>();
            ConstructGrahpView();
            GenerateToolbar();
            GenerateMiniMap();

        }

        private void OnDisable()
        {

            FAED_AIGrahpViewModlue.portNums = null;
            rootVisualElement.Remove(grahpView);

        }

    }

    public class FAED_AIGrahpViewModlue : GraphView
    {

        private Dictionary<FAED_AIGrahpViewNodeModlue, List<string>> keyPort = new Dictionary<FAED_AIGrahpViewNodeModlue, List<string>>();
        public static Dictionary<string, int> portNums = new Dictionary<string, int>();

        public readonly Vector2 defultNodeSize = new Vector2(x: 150, y: 200);        

        public FAED_AIGrahpViewModlue()
        {

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var guid = new GridBackground();
            Insert(index: 0, guid);
            guid.StretchToParentSize();

            AddElement(GenerateEntryPoint());


        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {

            var compatiblePorts = new List<Port>();

            ports.ForEach(funcCall: (port) =>
            {

                if (startPort != port && startPort.node != port.node)
                {

                    compatiblePorts.Add(port);

                }

            });

            return compatiblePorts;

        }

        private Port GeneratePort(FAED_AIGrahpViewNodeModlue node, Direction protDirection, Port.Capacity capacity = Port.Capacity.Single)
        {

            return node.InstantiatePort(Orientation.Horizontal, protDirection, capacity, typeof(float));

        }

        private FAED_AIGrahpViewNodeModlue GenerateEntryPoint()
        {

            //노드 생성
            #region 시작 노드 생성

            var node = new FAED_AIGrahpViewNodeModlue
            {

                title = "STARTSTATE",
                GUID = Guid.NewGuid().ToString(),
                viewText = "ENTRYPOINT",
                entryPoint = true,
                nodeType = FAED_AINodeType.Start,
                

            };

            var stateMachineNode = CreateDialogNode("MainStateMachine", FAED_AINodeType.StateMachine);

            #endregion

            //줄 생성
            var genratedPoint = GeneratePort(node, Direction.Output);
            genratedPoint.portName = "Next";
            node.outputContainer.Add(genratedPoint);

            stateMachineNode.capabilities &= ~Capabilities.Deletable;

            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;

            portNums.Add(string.Format("{0} : {1}", node.GUID, "Next"), 0);

            stateMachineNode.RefreshExpandedState();
            node.RefreshExpandedState();
            stateMachineNode.RefreshPorts();
            node.RefreshPorts();

            AddElement(stateMachineNode);

            node.SetPosition(new Rect(x: 100, y: 200, width: 100, height: 150));

            return node;

        }

        public FAED_AIGrahpViewNodeModlue CreateDialogNode(string nodeName, FAED_AINodeType nodeType = FAED_AINodeType.Action, bool isStarting = false)
        {

            if(nodeType == FAED_AINodeType.Action)//엑션 노드
            {

                var dialogeNode = new FAED_AIGrahpViewNodeModlue
                {

                    title = nodeName,
                    viewText = nodeName,
                    GUID = Guid.NewGuid().ToString(),
                    entryPoint = false,
                    nodeType = FAED_AINodeType.Action
                    
                };

                var inputPort = GeneratePort(dialogeNode, Direction.Input, Port.Capacity.Multi);
                inputPort.portName = "Input";
                dialogeNode.inputContainer.Add(inputPort);

                var textField = new TextField(string.Empty);
                textField.RegisterValueChangedCallback(evt =>
                {

                    dialogeNode.viewText = evt.newValue;
                    dialogeNode.title = evt.newValue;

                });
                textField.SetValueWithoutNotify(dialogeNode.title);
                dialogeNode.mainContainer.Add(textField);

                dialogeNode.RefreshExpandedState();
                dialogeNode.RefreshPorts();
                dialogeNode.SetPosition(new Rect(position: Vector2.zero, defultNodeSize));

                return dialogeNode;

            }
            else if(nodeType == FAED_AINodeType.StateMachine)//스테이트머신노드
            {

                var dialogeNode = new FAED_AIGrahpViewNodeModlue
                {

                    title = nodeName,
                    viewText = nodeName,
                    GUID = Guid.NewGuid().ToString(),
                    entryPoint = false,
                    nodeType = FAED_AINodeType.StateMachine

                };

                var inputPort = GeneratePort(dialogeNode, Direction.Input, Port.Capacity.Multi);
                inputPort.portName = "Input";
                dialogeNode.inputContainer.Add(inputPort);

                var button = new Button(clickEvent: () => { AddStatePort(dialogeNode); });
                button.text = "New State";
                dialogeNode.titleButtonContainer.Add(button);
                
                dialogeNode.RefreshExpandedState();
                dialogeNode.RefreshPorts();
                dialogeNode.SetPosition(new Rect(position: Vector2.zero, defultNodeSize));

                return dialogeNode;

            }
            else if(nodeType == FAED_AINodeType.BoolNode)
            {

                var dialogeNode = new FAED_AIGrahpViewNodeModlue
                {

                    title = nodeName,
                    viewText = nodeName,
                    GUID = Guid.NewGuid().ToString(),
                    entryPoint = false,
                    nodeType = FAED_AINodeType.BoolNode

                };

                var inputPort = GeneratePort(dialogeNode, Direction.Input, Port.Capacity.Multi);
                inputPort.portName = "Input";
                dialogeNode.inputContainer.Add(inputPort);

                var textField = new TextField(string.Empty);
                textField.RegisterValueChangedCallback(evt =>
                {

                    dialogeNode.viewText = evt.newValue;
                    dialogeNode.title = evt.newValue;

                });

                dialogeNode.Add(textField);

                if(isStarting == false)
                {

                    AddPort(dialogeNode, "true");
                    AddPort(dialogeNode, "false");

                }

                dialogeNode.RefreshExpandedState();
                dialogeNode.RefreshPorts();
                dialogeNode.SetPosition(new Rect(position: Vector2.zero, defultNodeSize));

                return dialogeNode;

            }

            return null;

        }

        public void AddStatePort(FAED_AIGrahpViewNodeModlue dialogeNode, string name = "", string Guid = "")
        {

            if (name != "" && keyPort.ContainsKey(dialogeNode) == true)
            {

                if (keyPort[dialogeNode].Contains(name)) return;
                else keyPort[dialogeNode].Add(name);

            }
            else if (name != "" && keyPort.ContainsKey(dialogeNode) == false)
            {

                keyPort.Add(dialogeNode, new List<string> { name });

            }

            var generatedPort = GeneratePort(dialogeNode, Direction.Output, Port.Capacity.Multi);

            var oldLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.contentContainer.Remove(oldLabel);

            var outputPortCount = dialogeNode.outputContainer.Query(name: "connector").ToList().Count;

            var portName = string.IsNullOrEmpty(name) ? $"State {outputPortCount + 1}" : name;

            if (portNums.ContainsKey(string.Format("{0} : {1}", dialogeNode.GUID, portName)) == false || portNums.ContainsKey(string.Format("{0} : {1}", Guid, portName)) == false)
            {   

                if(Guid == "")
                {

                    portNums.Add(string.Format("{0} : {1}", dialogeNode.GUID, portName), outputPortCount);

                }
                else
                {

                    portNums.Add(string.Format("{0} : {1}", Guid, portName), outputPortCount);

                }

            }

            var textField = new TextField { name = string.Empty, value = portName };
            textField.RegisterValueChangedCallback(evt => 
            {

                int portCount = portNums[string.Format("{0} : {1}", dialogeNode.GUID, generatedPort.portName)];
                portNums.Remove(string.Format("{0} : {1}", dialogeNode.GUID, generatedPort.portName));
                generatedPort.portName = evt.newValue;

                portNums.Add(string.Format("{0} : {1}", dialogeNode.GUID, generatedPort.portName), portCount);

            });

            generatedPort.contentContainer.Add(new Label(" "));
            generatedPort.contentContainer.Add(textField);


            var delButton = new Button(() => RemovePort(dialogeNode, generatedPort)) { text = "X"};
            generatedPort.contentContainer.Add(delButton);

            generatedPort.portName = portName;
            dialogeNode.outputContainer.Add(generatedPort);
            dialogeNode.RefreshPorts();
            dialogeNode.RefreshExpandedState();

        }

        public void AddPort(FAED_AIGrahpViewNodeModlue dialogeNode, string name = "")
        {

            if (name != "" && keyPort.ContainsKey(dialogeNode) == true)
            {

                if (keyPort[dialogeNode].Contains(name)) return;
                else keyPort[dialogeNode].Add(name);

            }
            else if (name != "" && keyPort.ContainsKey(dialogeNode) == false)
            {

                keyPort.Add(dialogeNode, new List<string> { name });

            }

            var generatedPort = GeneratePort(dialogeNode, Direction.Output, Port.Capacity.Multi);

            var outputPortCount = dialogeNode.outputContainer.Query(name: "connector").ToList().Count;

            var portName = string.IsNullOrEmpty(name) ? $"Port {outputPortCount + 1}" : name;

            if (portNums.ContainsKey(string.Format("{0} : {1}", dialogeNode.GUID, portName)) == false)
            {

                portNums.Add(string.Format("{0} : {1}", dialogeNode.GUID, portName), outputPortCount);

            }

            generatedPort.portName = portName;
            dialogeNode.outputContainer.Add(generatedPort);
            dialogeNode.RefreshPorts();
            dialogeNode.RefreshExpandedState();

        }


        private void RemovePort(FAED_AIGrahpViewNodeModlue dialogeNode, Port generatedPort)
        {

            var targetEdge = edges.ToList().Where(x => x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

            if (targetEdge.Any())
            {

                var edge = targetEdge.First();
                edge.input.Disconnect(edge);
                RemoveElement(targetEdge.First());

            }

            portNums.Remove(string.Format("{0} : {1}", dialogeNode.GUID, generatedPort.portName));

            dialogeNode.outputContainer.Remove(generatedPort);
            dialogeNode.RefreshPorts();
            dialogeNode.RefreshExpandedState();

        }

        public void CreateNode(string nodeName, FAED_AINodeType nodeType = FAED_AINodeType.Action, bool isStart = false)
        {

            AddElement(CreateDialogNode(nodeName, nodeType, isStart));

        }

    }

    public class FAED_AIGrahpViewNodeModlue : Node
    {

        public string GUID;
        public string viewText;
        public bool entryPoint = false;
        public FAED_AINodeType nodeType = FAED_AINodeType.Action;

    }

}

#endif