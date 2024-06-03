using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;
using EC.Component;

namespace EC.StateGraph
{
    public class SGPlayer : IStateGraph
    {
        public Dictionary<StateEnum, State> CreateStates(Entity entt)
        {
            Dictionary<StateEnum, State> states = new Dictionary<StateEnum, State>();
            
            var idleState = new State(StateEnum.Idle, entt, OnEnterIdle, OnStayIdle, OnExitIdle, null, "Idle");
            idleState.RegisterEvent(EventType.FallDown, (entity, data) =>
            {
                var stateComp = entity.GetEComponent<StateComponent>(ComponentType.State);
                if (stateComp.HasTag("Idle"))
                {
                    stateComp.GotoState(StateEnum.Fall);
                }
            });
            idleState.RegisterAction(ActionType.Run, (e) =>
            {
                e.GetEComponent<StateComponent>(ComponentType.State)?.GotoState(StateEnum.Run);
            });
            idleState.RegisterAction(ActionType.Jump, (e) =>
            {
                e.GetEComponent<StateComponent>(ComponentType.State)?.GotoState(StateEnum.Jump);
            });
            idleState.RegisterAction(ActionType.Attack, (e) =>
            {
                e.GetEComponent<StateComponent>(ComponentType.State)?.GotoState(StateEnum.Attack);
            });
            states.Add(StateEnum.Idle, idleState);

            var runState = new State(StateEnum.Run, entt, OnEnterRun, OnStayRun, OnExitRun, null, "Run-Forward");
            runState.RegisterAction(ActionType.Jump, (e) =>
            {
                e.GetEComponent<StateComponent>(ComponentType.State)?.GotoState(StateEnum.Jump);
            });
            runState.RegisterAction(ActionType.Attack, (e) =>
            {
                e.GetEComponent<StateComponent>(ComponentType.State)?.GotoState(StateEnum.Attack);
            });
            states.Add(StateEnum.Run, runState);

            var jumpState = new State(StateEnum.Jump, entt, OnEnterJump, OnStayJump, OnExitJump, IsJumpBusy, "Jump");
            jumpState.RegisterEvent(EventType.FallDown, (entity, data) =>
            {
                var stateComp = entity.GetEComponent<StateComponent>(ComponentType.State);
                if (stateComp.HasTag("Jumping"))
                {
                    stateComp.GotoState(StateEnum.Fall, 0.1f);
                }
            });
            jumpState.RegisterAction(ActionType.Attack, (e) =>
            {
                e.GetEComponent<StateComponent>(ComponentType.State)?.GotoState(StateEnum.Attack);
            });
            states.Add(StateEnum.Jump, jumpState);

            var fallState = new State(StateEnum.Fall, entt, OnEnterFall, OnStayFall, OnExitFall, IsFallBusy, "Fall");
            fallState.RegisterEvent(EventType.FallToGround, (entity, data) =>
            {
                var stateComp = entity.GetEComponent<StateComponent>(ComponentType.State);
                if (stateComp.HasTag("Falling"))
                {
                    stateComp.GotoState(StateEnum.Idle);
                }
            });
            fallState.RegisterAction(ActionType.Attack, (e) =>
            {
                e.GetEComponent<StateComponent>(ComponentType.State)?.GotoState(StateEnum.Attack);
            });
            states.Add(StateEnum.Fall, fallState);
            //todo:攻击动作似乎总是能打断其他state，需要针对状态优先级进行设计
            return states;
        }

        #region Idle
        public void OnEnterIdle(Entity e, object param = null)
        {
            var moveComp = e.GetEComponent<CharacterMoveComponent>(ComponentType.CharacterMove);
            if (moveComp != null)
            {
                moveComp.Stop();
            }
            
            e.GetEComponent<StateComponent>(ComponentType.State)?.AddTag("Idle");
        }

        public void OnStayIdle(Entity e, float deltaTime, object param = null)
        {
            
        }

        public void OnExitIdle(Entity e, object param = null)
        {
            e.GetEComponent<StateComponent>(ComponentType.State)?.RemoveTag("Idle");
        }

        #endregion

        
        
        #region Run
        
        public void OnEnterRun(Entity e, object param = null)
        {
            var controllerComp = e.GetEComponent<ControllerComponent>(ComponentType.Controller);
            var moveComp = e.GetEComponent<CharacterMoveComponent>(ComponentType.CharacterMove);
            if (moveComp != null)
            {
                moveComp.Move(controllerComp.JoyStickDir);
            }
            e.GetEComponent<StateComponent>(ComponentType.State)?.AddTag("Running");
        }
        
        public void OnStayRun(Entity e, float deltaTime, object param = null)
        {
            var controllerComp = e.GetEComponent<ControllerComponent>(ComponentType.Controller);
            var moveComp = e.GetEComponent<CharacterMoveComponent>(ComponentType.CharacterMove);
            if (moveComp != null)
            {
                moveComp.Move(controllerComp.JoyStickDir);
                if (controllerComp.JoyStickDir == Vector3.zero)
                {
                    e.GetEComponent<StateComponent>(ComponentType.State)?.GotoState(StateEnum.Idle);
                }
            }
        }

        public void OnExitRun(Entity e, object param = null)
        {
            e.GetEComponent<StateComponent>(ComponentType.State)?.RemoveTag("Running");
        }

        #endregion

        
        
        #region Jump

        public void OnEnterJump(Entity e, object param = null)
        {
            var moveComp = e.GetEComponent<CharacterMoveComponent>(ComponentType.CharacterMove);
            if (moveComp != null)
            {
                moveComp.Jump();
            }
            e.GetEComponent<StateComponent>(ComponentType.State)?.AddTag("Jumping");
        }
        
        public void OnStayJump(Entity e, float deltaTime, object param = null)
        {
            var controllerComp = e.GetEComponent<ControllerComponent>(ComponentType.Controller);
            var moveComp = e.GetEComponent<CharacterMoveComponent>(ComponentType.CharacterMove);
            if (moveComp != null)
            {
                Vector3 dir = controllerComp.JoyStickDir;
                dir *= GameConfig.Instance.JUMPING_CHANGE_HORIZONTAL_DIR_FACTOR;
                moveComp.ChangeHorizontalDir(deltaTime * dir);
            }
        }

        public void OnExitJump(Entity e, object param = null)
        {
            e.GetEComponent<StateComponent>(ComponentType.State)?.RemoveTag("Jumping");
        }

        public bool IsJumpBusy(Entity e)
        {
            return false;
        }

        #endregion

        
        
        #region Fall

        public void OnEnterFall(Entity e, object param = null)
        {
            e.GetEComponent<StateComponent>(ComponentType.State)?.AddTag("Falling");
        }
        
        public void OnStayFall(Entity e, float deltaTime, object param = null)
        {
            var controllerComp = e.GetEComponent<ControllerComponent>(ComponentType.Controller);
            var moveComp = e.GetEComponent<CharacterMoveComponent>(ComponentType.CharacterMove);
            if (moveComp != null)
            {
                Vector3 dir = controllerComp.JoyStickDir;
                dir *= GameConfig.Instance.JUMPING_CHANGE_HORIZONTAL_DIR_FACTOR;
                moveComp.ChangeHorizontalDir(deltaTime * dir);
            }
        }
        
        public void OnExitFall(Entity e, object param = null)
        {
            e.GetEComponent<StateComponent>(ComponentType.State)?.RemoveTag("Falling");
        }

        public bool IsFallBusy(Entity e)
        {
            return false;
        }

        #endregion
    }
}