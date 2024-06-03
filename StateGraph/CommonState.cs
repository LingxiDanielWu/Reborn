using System;
using System.Collections.Generic;
using Cysharp.Text;
using EC.Component;
using UnityEngine;
namespace EC
{
    public class CommonState
    {
        public static void AddAttackState(Entity entt, Dictionary<StateEnum, State> states)
        {
            var attackState = new State(StateEnum.Attack, entt, OnEnterAttack, null, OnExitAttack, IsAttackBusy, "Attack-L1");
            attackState.RegisterEvent(EventType.AnimOver, (entity, data) =>
            {
                var stateComp = entity.GetEComponent<StateComponent>(ComponentType.State);
                stateComp?.GotoState(StateEnum.Idle, 0f);
            });
            states.Add(StateEnum.Attack, attackState);
        }

        public static void OnEnterAttack(Entity e, object param = null)
        {
            var controllerComp = e.GetEComponent<ControllerComponent>(ComponentType.Controller);
            var moveComp = e.GetEComponent<CharacterMoveComponent>(ComponentType.CharacterMove);
            if (moveComp != null)
            {
                moveComp.DoAttackMove(controllerComp.JoyStickDir);
            }
            e.GetEComponent<StateComponent>(ComponentType.State)?.AddTag("Attacking");
        }

        public static void OnExitAttack(Entity e, object param = null)
        {
            var moveComp = e.GetEComponent<CharacterMoveComponent>(ComponentType.CharacterMove);
            if (moveComp != null)
            {
                moveComp.Stop();
            }
            e.GetEComponent<StateComponent>(ComponentType.State)?.RemoveTag("Attacking");
        }

        public static bool IsAttackBusy(Entity e)
        {
            return true;
        }
    }
    
    public class State
    {
        private Func<Entity, bool> isBusyFn;
        private Action<Entity, object> onEnterFn;
        private Action<Entity, float, object> onStayFn;
        private Action<Entity, object> onExitFn;
        private StateEnum stateEnum;
        private Dictionary<EventType, Handler> events = new Dictionary<EventType, Handler>();
        private Dictionary<ActionType, Action<Entity>> actionHandlers = new Dictionary<ActionType, Action<Entity>>();
        private int timeInState = 0;
        private int timelineIndex = -1;
        private List<TimeEvent> timeline = new List<TimeEvent>();

        public string AnimName
        {
            get;
            set;
        }

        public State(StateEnum state, Entity entt, 
            Action<Entity, object> enterAction = null, 
            Action<Entity, float, object> stayAction = null, 
            Action<Entity, object> exitAction = null,
            Func<Entity, bool> busyAction = null,
            string animName = "")
        {
            stateEnum = state;
            onEnterFn = enterAction;
            onStayFn = stayAction;
            onExitFn = exitAction;
            isBusyFn = busyAction;
            AnimName = animName;
        }

        private void TickState(float deltaTime, Entity e)
        {
            timeInState += (int)(deltaTime * 1000);
            while (timelineIndex >= 0 && timelineIndex < timeline.Count && timeInState >= timeline[timelineIndex].Time)
            {
                timeline[timelineIndex++].Execute(e);
                if (timelineIndex >= timeline.Count)
                {
                    timelineIndex = -1;
                }
            }
        }

        private void ResetTimeline()
        {
            timeInState = 0;
            timelineIndex = 0;
        }

        public void OnEnter(Entity entt, float fadeInTime = 0.25f)
        {
            entt.GetEComponent<AnimatorComponent>(ComponentType.Animator)?.Play(AnimName, fadeInTime);
            ResetTimeline();
            onEnterFn?.Invoke(entt, fadeInTime);
        }

        public void OnStay(Entity entt, float deltaTime, object param = null)
        {
            TickState(deltaTime, entt);
            onStayFn?.Invoke(entt, deltaTime, param);
        }
        
        public void OnExit(Entity entt, object param = null)
        {
            ResetTimeline();
            onExitFn?.Invoke(entt, param);
        }

        public bool IsBusy(Entity entt)
        {
            if (isBusyFn != null)
            {
                return isBusyFn(entt);
            }

            return false;
        }

        public void RegisterEvent(EventType type, Handler handler)
        {
            events.TryAdd(type, handler);
        }

        public void UnRegisterEvent(EventType type)
        {
            if (events.ContainsKey(type))
            {
                events.Remove(type);
            }
        }

        public void HandleEvent(Entity entity, EventType type, object data)
        {
            if (events.TryGetValue(type, out Handler handler))
            {
                handler(entity, data);
            }
        }
        
        public void RegisterAction(ActionType type, Action<Entity> handler)
        {
            actionHandlers.TryAdd(type, handler);
        }

        public void HandleAction(Entity entity, BufferedAction data)
        {
            if (actionHandlers.TryGetValue(data.Type, out Action<Entity> handler))
            {
                handler?.Invoke(entity);
            }
        }

        public void AddTimelineEvent(TimeEvent te)
        {
            if (!timeline.Contains(te))
            {
                timeline.Add(te);
            }
        }
    }
}