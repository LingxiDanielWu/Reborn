using System;
using System.Collections.Generic;
using UnityEngine;
using EC.Component;

namespace EC
{
    public class StateComponent : EComponent, IActionHandler
    {
        private StateEnum curStateEnum;
        
        public StateEnum CurState
        {
            get
            {
                return curStateEnum;
            }
            private set
            {
                curStateEnum = value;
            }
        }

        public bool IsBusy
        {
            get
            {
                return stateList[CurState].IsBusy(Parent);
            }
        }

        private Dictionary<string, bool> stateTags = new Dictionary<string, bool>();

        private Dictionary<StateEnum, State> stateList = new Dictionary<StateEnum, State>();

        private Dictionary<ActionType, Action> actionHandlers = new Dictionary<ActionType, Action>();

        public StateComponent(Entity e) : base(ComponentType.State, e)
        {
            curStateEnum = StateEnum.None;
        }

        public override void Tick(float deltaTime, params object[] paras)
        {
            if (CurState != StateEnum.None)
            {
                var curState = stateList[CurState];
                curState.OnActionStay(Parent, deltaTime, Parent);
            }
        }

        public void GotoState(StateEnum newStateEnum)
        {
            // Debug.Log($"sssssssss   c:{curStateEnum.PlayerToString()}   n:{newStateEnum.PlayerToString()}");
            if (!stateList.ContainsKey(newStateEnum))
            {
                Debug.LogError($"没有状态{newStateEnum.ToString()}，记得添加");
                return;
            }

            InnerChangeState(newStateEnum);
        }

        private void InnerChangeState(StateEnum newStateEnum)
        {
            if (CurState == newStateEnum)
                return;
            
            if (CurState != StateEnum.None)
            {
                var curState = stateList[CurState];
                curState.OnActionExit(Parent);
            }

            var newState = stateList[newStateEnum];
            newState.OnActionEnter(Parent);
            CurState = newStateEnum;
        }

        public bool HasTag(string tagName)
        {
            if (stateTags.TryGetValue(tagName, out bool result))
            {
                return result;
            }

            return false;
        }

        public void AddTag(string tagName)
        {
            stateTags.TryAdd(tagName, true);
        }

        public void RemoveTag(string tagName)
        {
            if (stateTags.ContainsKey(tagName))
            {
                stateTags.Remove(tagName);
            }
        }

        public override void Init()
        {
            stateList = StateFactory.Instance.GetStates(Parent, Parent.EType);
            GotoState(StateEnum.Idle);
        }

        public override void HandleEvent(EventType eventType, object data)
        {
            stateList[CurState].HandleEvent(Parent, eventType, data);
        }

        public void RegisterActionHandler(ActionType type, Action handler)
        {
            actionHandlers.TryAdd(type, handler);
        }

        public void HandleAction(ActionType type)
        {
            if (actionHandlers.TryGetValue(type, out Action handler))
            {
                handler?.Invoke();
            }
        }

        public override void Dispose()
        {
        }
    }
    
    public enum StateEnum
    {
        None,
        //Common
        Attack,
        //Others
        Idle,
        Run,
        Dashing,
        Sprinting,
        Rolling,
        Dodge,
        Jump,
        Fall,
    }

    public class State
    {
        private Func<Entity, bool> isBusyAction;
        private Action<Entity, object> onActionEnterAction;
        private Action<Entity, float, object> onActionStayAction;
        private Action<Entity, object> onActionExitAction;
        private StateEnum stateEnum;
        private Dictionary<EventType, Handler> events = new Dictionary<EventType, Handler>();
        private int timeInState = 0;
        private int timelineIndex = -1;
        private List<TimeEvent> timeline = new List<TimeEvent>();
        
        public State(StateEnum state, Entity entt, 
            Action<Entity, object> enterAction = null, 
            Action<Entity, float, object> stayAction = null, 
            Action<Entity, object> exitAction = null,
            Func<Entity, bool> busyAction = null)
        {
            stateEnum = state;
            onActionEnterAction = enterAction;
            onActionStayAction = stayAction;
            onActionExitAction = exitAction;
            isBusyAction = busyAction;
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

        public void OnActionEnter(Entity entt, object param = null)
        {
            onActionEnterAction?.Invoke(entt, param);
            ResetTimeline();
        }
        
        public void OnActionStay(Entity entt, float deltaTime, object param = null)
        {
            onActionStayAction?.Invoke(entt, deltaTime, param);
            TickState(deltaTime, entt);
        }
        
        public void OnActionExit(Entity entt, object param = null)
        {
            onActionExitAction?.Invoke(entt, param);
            ResetTimeline();
        }

        public bool IsBusy(Entity entt)
        {
            if (isBusyAction != null)
            {
                return isBusyAction(entt);
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
            
        public void AddTimelineEvent(TimeEvent te)
        {
            if (!timeline.Contains(te))
            {
                timeline.Add(te);
            }
        }
    }
}
