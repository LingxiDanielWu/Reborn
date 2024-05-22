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
        
        public Vector3 JoyStickDir
        {
            get;
            private set;
        }

        private ActionComponent actionComp;

        public ActionComponent ActionComp
        {
            get
            {
                if (actionComp == null)
                {
                    actionComp = Parent.GetEComponent<ActionComponent>(ComponentType.Action);
                }

                return actionComp;
            }
        }

        private ActionType curActionType = ActionType.None;
        
        public ControllerComponent() : base(ComponentType.Controller)
        {

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
            JoyStickDir = Vector3.zero;
            curActionType = ActionType.None;
            
            if (InputManager.Instance.IsTriggerOpt(OptKeyMap[ControlOpt.MoveForward]))
            {
                JoyStickDir += Vector3.forward;
            }
            
            if (InputManager.Instance.IsTriggerOpt(OptKeyMap[ControlOpt.MoveBack]))
            {
                JoyStickDir += Vector3.back;
            }
            
            if (InputManager.Instance.IsTriggerOpt(OptKeyMap[ControlOpt.MoveLeft]))
            {
                JoyStickDir += Vector3.left;
            }
            
            if (InputManager.Instance.IsTriggerOpt(OptKeyMap[ControlOpt.MoveRight]))
            {
                JoyStickDir += Vector3.right;
            }

            float delayTime = 0;
            if (InputManager.Instance.IsTriggerOpt(OptKeyMap[ControlOpt.Jump]))
            {
                curActionType = ActionType.Jump;
                delayTime = GameConfig.Instance.COMMON_ACTION_DELAY_TIME;
            }
            else if (InputManager.Instance.IsTriggerOpt(OptKeyMap[ControlOpt.Attack]))
            {
                curActionType = ActionType.Attack;
            }
            
            if (curActionType != ActionType.None)
            {
                var playerAction = new BufferedAction(Parent, null, JoyStickDir, curActionType, delayTime);
                ActionComp.TryPushAction(playerAction);
            }
            else
            {
                if (JoyStickDir != Vector3.zero)
                {
                    curActionType = ActionType.Run;
                    var playerAction = new BufferedAction(Parent, null, JoyStickDir, curActionType);
                    ActionComp.TryPushAction(playerAction);
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
