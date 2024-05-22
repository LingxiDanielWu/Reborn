using System;
using UnityEngine;
using EC.Component;
using EC.Manager;

namespace EC
{
    public class Player : Entity
    {
        public Player() : base(EntityType.Player)
        {
            
        }
        
        public void InitAnimEvent()
        {
            var animComp = GetEComponent<AnimatorComponent>(ComponentType.Animator);
            if (animComp == null || animComp.ParentAnimator == null)
                return;

            var clips = animComp.ParentAnimator.runtimeAnimatorController.animationClips;
            for (int i = 0; i < clips.Length; i++)
            {
                AnimationEvent e = new AnimationEvent();
                e.stringParameter = clips[i].name;
                e.functionName = "AnimOverEvent";
                e.time = clips[i].length;
                clips[i].AddEvent(e);
            }
            GameEventManager.Instance.RegisterListener(EventType.AnimOver, this);
        }
        
        public void AnimOverEvent(string clipName)
        {
            GameEventManager.Instance.PublishToEntity(EventType.AnimOver, null, clipName);
        }
    }
}