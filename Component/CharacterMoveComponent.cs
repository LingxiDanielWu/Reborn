using EC.Component;
using UnityEngine;

namespace EC
{
    public class CharacterMoveComponent : EComponent
    {
        private const float GroundedVerticalSpeedFactor = 0.1f;
        private CharacterController ctrlerComp;
        public CharacterController CtrlerComp
        {
            get
            {
                if (ctrlerComp == null)
                {
                    var resComp = Parent.GetEComponent<GameObjectComponent>(ComponentType.GameObject);
                    ctrlerComp = resComp.EGameObject.GetComponent<CharacterController>();
                }

                return ctrlerComp;
            }
        }

        public Vector3 HorizontalForwardDir
        {
            get;
            private set;
        }

        public float HorizontalMoveSpeed
        {
            get;
            private set;
        }

        public Vector3 VerticalForwardDir
        {
            get;
            private set;
        }

        public float VerticalMoveSpeed
        {
            get;
            private set;
        }

        public float HorizontalMoveDragFactor
        {
            get;
            private set;
        }

        public CharacterMoveComponent() : base(ComponentType.CharacterMove)
        {
        }

        private Vector3 turnVelocity;
        
        public override void Tick(float deltaTime, params object[] paras)
        {
            if (IsStandStill())
            {
                return;
            }

            if (CtrlerComp.isGrounded)
            {
                if (VerticalMoveSpeed < 0f && Mathf.Abs(VerticalMoveSpeed - GameConfig.Instance.GRAVITY_ACCELATION * GroundedVerticalSpeedFactor) > 0.001f)
                {
                    //落地时保持一个向下的速度，保证角色能被按在地板上，不发生抖动
                    VerticalMoveSpeed = GameConfig.Instance.GRAVITY_ACCELATION * GroundedVerticalSpeedFactor;
                    BackToGround();
                }
            }
            else
            {
                float tempVSpeed = VerticalMoveSpeed;
                VerticalMoveSpeed += GameConfig.Instance.GRAVITY_ACCELATION * deltaTime;
                if (VerticalMoveSpeed < 0 && tempVSpeed >= 0)
                {
                    Fall();
                }
                VerticalForwardDir = Vector3.up;
            }

            var forward = GetCurForward();
            var gameObj = Parent.GetEComponent<GameObjectComponent>(ComponentType.GameObject);
            gameObj.EGameObject.transform.forward = Vector3.Lerp(gameObj.EGameObject.transform.forward, forward, GameConfig.Instance.CHARACTER_TURN_DIR_SPEED);
            
            Vector3 hMotion = HorizontalMoveSpeed * HorizontalMoveDragFactor * deltaTime * forward;
            Vector3 vMotion = VerticalMoveSpeed * deltaTime * VerticalForwardDir;
            CtrlerComp.Move(hMotion + vMotion);
        }

        public Vector3 GetCharacterColliderCenter()
        {
            return CtrlerComp.gameObject.transform.TransformPoint(CtrlerComp.center);
        }

        private Vector3 GetCurForward()
        {
            //先基于相机朝向获取行走方向
            var cameraComp = Parent.GetEComponent<CameraComponent>(ComponentType.Camera);
            if (cameraComp != null)
            {
                var angle = GetCurHorizontalAngle();
                Vector3 f = cameraComp.CameraRootObject.transform.forward;
                f.y = 0;
                var newForward = Quaternion.Euler(0, angle, 0) * f;
                return newForward;
            }
            
            return Vector3.zero;
        }

        public float GetCurHorizontalAngle()
        {
            return Utils.GetVecAngle(Vector3.forward, HorizontalForwardDir);
        }

        private bool IsStandStill()
        {
            bool isOnGround = Mathf.Abs(VerticalMoveSpeed - GameConfig.Instance.GRAVITY_ACCELATION * GroundedVerticalSpeedFactor) <= 0.001f;
            return CtrlerComp.isGrounded && isOnGround && HorizontalMoveSpeed == 0;
        }
        
        public void BackToGround()
        {
            HorizontalMoveDragFactor = 1f;
            Parent.Publish(Parent, EventType.FallToGround);
        }

        private void Fall()
        {
            Parent.Publish(Parent, EventType.FallDown);
        }

        public void Stop()
        {
            HorizontalMoveSpeed = 0;
            HorizontalForwardDir = Vector3.zero;
        }

        public void Move(Vector3 dir, float speed = 0)
        {
            if (dir != Vector3.zero && (dir.x != 0 || dir.z != 0))
            {
                speed = speed == 0 ? GameConfig.Instance.NORMAL_RUNNING_SPEED : speed;
                HorizontalMoveSpeed = speed;
                HorizontalForwardDir = dir;
            }
        }
        
        public void ChangeHorizontalDir(Vector3 dir)
        {
            if (Mathf.Abs(Vector3.Angle(HorizontalForwardDir, dir)) <= 0.1f)
                return;
            
            if (dir.x != 0 || dir.z != 0 && HorizontalForwardDir != Vector3.zero)
            {
                HorizontalForwardDir += dir;
            }
        }

        public void DoAttackMove(Vector3 dir)
        {
            HorizontalForwardDir = dir;
            HorizontalMoveSpeed = GameConfig.Instance.SPEED_ATTACK_MOVE;
        }

        public void ForceChangeFaceDir(Vector3 dir)
        {
            
        }

        public void Jump()
        {
            VerticalForwardDir = Vector3.up;
            VerticalMoveSpeed = GameConfig.Instance.JUMP_SPEED;
            HorizontalMoveDragFactor = GameConfig.Instance.HORIZONTAL_MOVE_DRAG_DURING_JUMP;
        }

        public override void Init()
        {
            HorizontalMoveDragFactor = 1f;
        }
        
        public override void Dispose()
        {
            
        }
    }
}
