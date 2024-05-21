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

        public AnimatorComponent(Entity e, Animator animator): base(ComponentType.Animator, e)
        {
            ParentAnimator = animator;
        }

        public override void SetActive(bool isActive)
        {
            base.SetActive(isActive);
            ParentAnimator.enabled = isActive;
        }

        public override void Tick(float deltaTime, params object[] paras)
        {
            
        }

        public void ExitState(StateEnum stateEnum)
        {
            
        }

        public void SetTrigger(string triggerName)
        {
            ParentAnimator.SetTrigger(triggerName);
        }

        public void ResetTrigger(string triggerName)
        {
            ParentAnimator.ResetTrigger(triggerName);
        }

        public void SetInt(string param, int value = 0)
        {
            ParentAnimator.SetInteger(param, value);
        }

        public override void Init()
        {
            
        }

        public override void Dispose()
        {
            
        }
    }
}
