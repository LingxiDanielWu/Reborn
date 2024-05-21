using System;
using System.Collections.Generic;
using UnityEngine;

namespace EC.Component
{
    public enum ActionType
    {
        None,
        Attack,
        Run,
        Dash,
        Sprint,
        Roll,
        Dodge,
        Jump,
        Fall,
    }
    
    public class BufferedAction
    {
        public Vector3 Dir;
        public Entity Doer;
        public Entity Target;
        public ActionType Type;
        
        public BufferedAction(Entity doer, Entity target, Vector3 dir, ActionType type = ActionType.None)
        {
            Doer = doer;
            Target = target;
            Dir = dir;
            Type = type;
        }

        public bool IsValid()
        {
            return true;
        }
    }

    public class ActionComponent : EComponent
    {
        public static Dictionary<ActionType, Func<Entity, BufferedAction, bool>> ACTION_CONDITION = new Dictionary<ActionType, Func<Entity, BufferedAction, bool>>
        {
            { ActionType.Run, (entity, action) =>
            {
                var comp = entity.GetEComponent<StateComponent>(ComponentType.State); 
                return comp.HasTag("Idle");
            } },
            { ActionType.Jump, (entity, action) =>
            {
                var comp = entity.GetEComponent<StateComponent>(ComponentType.State);
                return comp.HasTag("Idle") || comp.HasTag("Running");
            } },
            
        };

        private List<EComponent> listenComps = new List<EComponent>();
        
        private Queue<BufferedAction> actionQueue = new Queue<BufferedAction>();

        public ActionComponent(Entity e) : base(ComponentType.Action, e)
        {
        }

        public override void Init()
        {

        }

        public override void Dispose()
        {

        }

        public void AddAction(BufferedAction action)
        {
            actionQueue.Enqueue(action);
        }

        public override void Tick(float deltaTime, params object[] paras)
        {
            HandleAction();
        }

        private void HandleAction()
        {
            if (actionQueue.Count == 0)
                return;

            var action = actionQueue.Peek();
            DoAction(action);
            actionQueue.Dequeue();
        }

        private void DoAction(BufferedAction action)
        {
            if (action.Type == ActionType.None || IsBusy())
            {
                return;
            }

            if (ACTION_CONDITION.ContainsKey(action.Type) && ACTION_CONDITION[action.Type](Parent, action))
            {
                foreach (var c in listenComps)
                {
                    if (c is IActionHandler)
                    {
                        var i = c as IActionHandler;
                        i.HandleAction(action.Type);
                    }
                }
            }
        }

        private bool IsBusy()
        {
            if (Parent.GetEComponent<StateComponent>(ComponentType.State).IsBusy)
            {
                return true;
            }

            return false;
        }

        private void DoJump()
        {

        }

        private void DoRun()
        {

        }

        public void AddDoActionListener(EComponent comp)
        {
            if (listenComps.Contains(comp))
            {
                return;
            }
            
            listenComps.Add(comp);
        }

        public void RemoveDoActionListener(EComponent comp)
        {
            if (!listenComps.Contains(comp))
            {
                return;
            }

            listenComps.Remove(comp);
        }
    }
}
