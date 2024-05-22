using System.Collections.Generic;
using UnityEngine;
using EC.Component;

namespace EC.StateGraph
{
    public class SGPlayer : IStateGraph
    {
        public Dictionary<StateEnum, State> CreateStates(Entity entt)
        {
            Dictionary<StateEnum, State> states = new Dictionary<StateEnum, State>();
            var idleState = new State(StateEnum.Idle, entt, OnEnterIdle, OnStayIdle, OnExitIdle);
            idleState.RegisterEvent(EventType.FallDown, (entity, data) =>
            {
                var stateComp = entity.GetEComponent<StateComponent>(ComponentType.State);
                if (stateComp.HasTag("Idle"))
                {
                    stateComp.GotoState(StateEnum.Fall);
                }
            });
            states.Add(StateEnum.Idle, idleState);
            
            states.Add(StateEnum.Run, new State(StateEnum.Run, entt, OnEnterRun, OnStayRun, OnExitRun));
            var jumpState = new State(StateEnum.Jump, entt, OnEnterJump, OnStayJump, OnExitJump, IsJumpBusy);
            jumpState.RegisterEvent(EventType.FallDown, (entity, data) =>
            {
                var stateComp = entity.GetEComponent<StateComponent>(ComponentType.State);
                if (stateComp.HasTag("Jumping"))
                {
                    stateComp.GotoState(StateEnum.Fall);
                }
            });
            states.Add(StateEnum.Jump, jumpState);

            var fallState = new State(StateEnum.Fall, entt, OnEnterFall, OnStayFall, OnExitFall, IsFallBusy);
            fallState.RegisterEvent(EventType.FallToGround, (entity, data) =>
            {
                var stateComp = entity.GetEComponent<StateComponent>(ComponentType.State);
                if (stateComp.HasTag("Falling"))
                {
                    stateComp.GotoState(StateEnum.Idle);
                }
            });
            fallState.RegisterEvent(EventType.AnimOver, (entity, data) =>
            {
                // Debug.Log($"anim {data.ToString()} overrrrr ");
            });
            states.Add(StateEnum.Fall, fallState);

            var comp = entt.GetEComponent<StateComponent>(ComponentType.State);
            if (comp != null)
            {
                comp.RegisterActionHandler(ActionType.Run, () =>
                {
                    comp.GotoState(StateEnum.Run);
                });
                comp.RegisterActionHandler(ActionType.Jump, () =>
                {
                    comp.GotoState(StateEnum.Jump);
                });
            }

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
            
            var animComp = e.GetEComponent<AnimatorComponent>(ComponentType.Animator);
            if (animComp != null)
            {
                animComp.Play("Idle");
            }
            e.GetEComponent<StateComponent>(ComponentType.State)?.AddTag("Idle");
        }

        public void OnStayIdle(Entity e, float deltaTime, object param = null)
        {
            
        }

        public void OnExitIdle(Entity e, object param = null)
        {
            var animComp = e.GetEComponent<AnimatorComponent>(ComponentType.Animator);
            if (animComp != null)
            {
                animComp.Stop("Idle");
            }
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
                int moveDir = Utils.GetOrientation(Vector3.forward, controllerComp.JoyStickDir);
                var animComp = e.GetEComponent<AnimatorComponent>(ComponentType.Animator);
                animComp.Play("Move", moveDir);
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
                var animComp = e.GetEComponent<AnimatorComponent>(ComponentType.Animator);
                animComp.Play("Move", Utils.GetOrientation(Vector3.forward, controllerComp.JoyStickDir));

                if (controllerComp.JoyStickDir == Vector3.zero)
                {
                    e.GetEComponent<StateComponent>(ComponentType.State)?.GotoState(StateEnum.Idle);
                }
            }
        }

        public void OnExitRun(Entity e, object param = null)
        {
            var animComp = e.GetEComponent<AnimatorComponent>(ComponentType.Animator);
            if (animComp != null)
            {
                animComp.Stop("Move");   
            }
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
                var animComp = e.GetEComponent<AnimatorComponent>(ComponentType.Animator);
                animComp.Play("Jump");
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
            var animComp = e.GetEComponent<AnimatorComponent>(ComponentType.Animator);
            animComp?.Stop("Jump");
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
            var animComp = e.GetEComponent<AnimatorComponent>(ComponentType.Animator);
            if (animComp != null)
            {
                animComp.Play("Fall");
            }
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
            var animComp = e.GetEComponent<AnimatorComponent>(ComponentType.Animator);
            animComp?.Stop("Fall");
            e.GetEComponent<StateComponent>(ComponentType.State)?.RemoveTag("Falling");
        }

        public bool IsFallBusy(Entity e)
        {
            return false;
        }

        #endregion
    }
}