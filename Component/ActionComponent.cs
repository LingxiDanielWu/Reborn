using System;
using System.Collections;
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
        public float DelayTime;
        
        public BufferedAction(Entity doer, Entity target, Vector3 dir, ActionType type = ActionType.None, float delayTime = 0f)
        {
            Doer = doer;
            Target = target;
            Dir = dir;
            Type = type;
            DelayTime = delayTime;
        }

        public bool IsValid(Entity e)
        {
            if (ActionComponent.ACTION_CONDITION.TryGetValue(Type, out Func<Entity, BufferedAction, bool> condition))
            {
                return condition(e, this);
            }

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
                return comp.HasTag("Idle") || comp.HasTag("Running");
            } },
            { ActionType.Jump, (entity, action) =>
            {
                var comp = entity.GetEComponent<StateComponent>(ComponentType.State);
                return comp.HasTag("Idle") || comp.HasTag("Running");
            } },
            
        };

        private List<EComponent> listenComps = new List<EComponent>();
        private Queue<BufferedAction> actionQueue = new Queue<BufferedAction>();
        private BufferedAction curDoingAction;
        
        public ActionComponent(Entity e) : base(ComponentType.Action)
        {
        }

        public override void Init()
        {

        }

        public override void Dispose()
        {

        }

        public void TryPushAction(BufferedAction action)
        {
            if (IsBusy() || !action.IsValid(Parent))
            {
                return;
            }

            actionQueue.Enqueue(action);
        }

        public override void Tick(float deltaTime, params object[] paras)
        {
            // foreach (var ba in actionQueue)
            // {
            //     Debug.Log($" type:{ba.Type} dir:{ba.Dir.ToString()} time:{ba.DelayTime}");
            // }
            if (actionQueue.Count == 0 || IsBusy())
                return;

            if (curDoingAction == null)
            {
                curDoingAction = actionQueue.Peek();
            }

            if (curDoingAction.DelayTime == 0)
            {
                DoAction(curDoingAction);
            }
            else
            {
                Parent.DoTask(() => { DoAction(curDoingAction); }, curDoingAction.DelayTime);   
            }
        }

        private void DoAction(BufferedAction action)
        {
            if (action.Type == ActionType.None)
            {
                return;
            }

            if (action.IsValid(Parent))
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

            actionQueue.Dequeue();
            curDoingAction = null;
        }

        public bool IsBusy()
        {
            if (curDoingAction != null)
            {
                return true;
            }

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
