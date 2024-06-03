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

        public StateComponent() : base(ComponentType.State)
        {
            curStateEnum = StateEnum.None;
        }

        public override void Tick(float deltaTime, params object[] paras)
        {
            if (CurState != StateEnum.None)
            {
                var curState = stateList[CurState];
                curState.OnStay(Parent, deltaTime, Parent);
            }
        }

        public void GotoState(StateEnum newStateEnum, float fadeInTime = 0.25f)
        {
            if (!stateList.ContainsKey(newStateEnum))
            {
                Debug.LogError($"没有状态{newStateEnum.ToString()}，记得添加");
                return;
            }
            
            if (CurState == newStateEnum)
                return;
            
            if (CurState != StateEnum.None)
            {
                var curState = stateList[CurState];
                curState.OnExit(Parent);
            }

            var newState = stateList[newStateEnum];
            newState.OnEnter(Parent, fadeInTime);
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
        }

        public override void HandleEvent(EventType eventType, object data)
        {
            stateList[CurState].HandleEvent(Parent, eventType, data);
        }

        public override void Attach(Entity e)
        {
            base.Attach(e);
            var actionComp = e.GetEComponent<ActionComponent>(ComponentType.Action);
            if (actionComp != null)
            {
                actionComp.AddDoActionListener(this);
            }
        }

        public void HandleAction(BufferedAction data)
        {
            stateList[CurState].HandleAction(Parent, data);
        }

        public State GetCurState()
        {
            return stateList[CurState];
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
}
