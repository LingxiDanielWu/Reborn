using System;
using EC.Manager;
using UnityEditor.UI;
using UnityEngine;

namespace EC.Component
{
    public class AnimatorComponent : EComponent
    {
        public Animator ParentAnimator
        {
            get;
            private set;
        }

        public AnimatorComponent(Animator animator): base(ComponentType.Animator)
        {
            ParentAnimator = animator;
        }

        public override void Attach(Entity e)
        {
            base.Attach(e);
            if (Parent is Player)
            {
                var p = Parent as Player;
                p.InitAnimEvent();
            }
        }

        public override void SetActive(bool isActive)
        {
            base.SetActive(isActive);
            ParentAnimator.enabled = isActive;
        }

        public override void Tick(float deltaTime, params object[] paras)
        {
        }

        public void Play(string key, int value = 1)
        {
            ParentAnimator.SetInteger(key, value);
        }

        public void Stop(string key)
        {
            ParentAnimator.SetInteger(key, 0);
        }

        public override void Init()
        {
            
        }

        public override void Dispose()
        {
            
        }
    }
}
