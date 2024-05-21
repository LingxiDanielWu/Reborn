using System.Collections.Generic;
using EC.Component;
using EC.Manager;
using UnityEngine;

namespace EC
{
    public enum ControlOpt
    {
        None,
        //玩家操作
        MoveForward,
        MoveBack,
        MoveLeft,
        MoveRight,
        Jump,
        Attack,
        Defend,
        CameraRotate,
        //系统操作
        ExitGame,
        OpenMap,
    }

    public class ControllerComponent : EComponent
    {
        //操作->键位映射。todo:后续改为键位配置
        public static readonly Dictionary<ControlOpt, List<InputData>> OptKeyMap = new Dictionary<ControlOpt, List<InputData>>
        {
            { ControlOpt.MoveForward , new List<InputData>{new InputData(InputValue.KeyW, InputType.Key, InputState.Hold)}},
            { ControlOpt.MoveBack , new List<InputData>{new InputData(InputValue.KeyS, InputType.Key, InputState.Hold)}},
            { ControlOpt.MoveLeft , new List<InputData>{new InputData(InputValue.KeyA, InputType.Key, InputState.Hold)}},
            { ControlOpt.MoveRight , new List<InputData>{new InputData(InputValue.KeyD, InputType.Key, InputState.Hold)}},
            { ControlOpt.Jump , new List<InputData>{new InputData(InputValue.KeySpace, InputType.Key, InputState.Down)}},
            { ControlOpt.Attack , new List<InputData>{new InputData(InputValue.MouseLeft, InputType.Mouse, InputState.Down)}},
            { ControlOpt.CameraRotate , new List<InputData>{new InputData(InputValue.MouseRight, InputType.Mouse, InputState.Hold)}},
        };
        
        public Vector3 ControllerDir
        {
            get
            {
                return entityAction.Dir;
            }
        }

        private BufferedAction entityAction;
        private ActionComponent actionComp;
        
        public ControllerComponent(Entity e) : base(ComponentType.Controller, e)
        {
            entityAction = new BufferedAction(
                e, 
                null,
                Vector3.zero
                );
            actionComp = e.GetEComponent<ActionComponent>(ComponentType.Action);
        }

        public override void Tick(float deltaTime, params object[] paras)
        {
            // Debug.Log($"输入-----------------");
            // foreach (var mousePair in inputs)
            // {
            //     Debug.Log($" value:{mousePair.Key} state:{mousePair.Value.State}");
            // }
            
            if (!InputManager.Instance.HasInput)
            {
                return;
            }
            
            CreateAction();
        }

        private void CreateAction()
        {
            entityAction.Dir = Vector3.zero;
            if (InputManager.Instance.IsTriggerOpt(OptKeyMap[ControlOpt.MoveForward]))
            {
                entityAction.Dir += Vector3.forward;
            }
            
            if (InputManager.Instance.IsTriggerOpt(OptKeyMap[ControlOpt.MoveBack]))
            {
                entityAction.Dir += Vector3.back;
            }
            
            if (InputManager.Instance.IsTriggerOpt(OptKeyMap[ControlOpt.MoveLeft]))
            {
                entityAction.Dir += Vector3.left;
            }
            
            if (InputManager.Instance.IsTriggerOpt(OptKeyMap[ControlOpt.MoveRight]))
            {
                entityAction.Dir += Vector3.right;
            }
            
            entityAction.Type = ActionType.None;
            if (InputManager.Instance.IsTriggerOpt(OptKeyMap[ControlOpt.Jump]))
            {
                entityAction.Type = ActionType.Jump;
            }
            else if (InputManager.Instance.IsTriggerOpt(OptKeyMap[ControlOpt.Attack]))
            {
                entityAction.Type = ActionType.Attack;
            }
            
            if (entityAction.Type !=ActionType.None)
            {
                actionComp.AddAction(entityAction);
            }
            else
            {
                if (entityAction.Dir != Vector3.one)
                {
                    entityAction.Type = ActionType.Run;
                    actionComp.AddAction(entityAction);
                }
            }
        }

        public override void Init()
        {
            
        }

        public override void Dispose()
        {
            
        }
    }
}
