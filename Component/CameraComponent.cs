using EC.Manager;
using EC.Component;
using UnityEngine;

namespace EC
{
    public class CameraComponent : EComponent
    {
        private const float CAMERA_DEFAULT_PIVOT_Y = -0.35f;
        private const float CAMERA_DEFAULT_PIVOT_Z = -7f;
        #region 控制镜头跟随和水平旋转
        private GameObject cameraRootObject;
        public GameObject CameraRootObject
        {
            get
            {
                if (cameraRootObject == null)
                {
                    var sceneCharacterCamera = GameObject.Find("CharacterCamera");
                    if (sceneCharacterCamera == null)
                    {
                        var prefab = Resources.Load<GameObject>("Common/CharacterCamera");
                        sceneCharacterCamera = GameObject.Instantiate(prefab);
                    }

                    cameraRootObject = sceneCharacterCamera;
                }

                return cameraRootObject;
            }
        }
        
        public float HorizontalAngle
        {
            get;
            set;
        }
        #endregion

        #region 控制Y轴移动和竖直方向旋转
        private GameObject cameraPivotYObject;
        public GameObject CameraPivotYObject
        {
            get
            {
                if (cameraPivotYObject == null)
                {
                    cameraPivotYObject = CameraRootObject.transform.Find("CameraPivotY").gameObject;
                }

                return cameraPivotYObject;
            }
        }
        
        private float VerticalAngle
        {
            get;
            set;
        }
        #endregion

        #region 相机
        private const float CAMERA_COLLIDER_RADIUS = 0.2f;
        public float CameraDefaultZPos
        {
            get;
            private set;
        }

        private Vector3 cameraObjectPos;

        private Camera mainCamera;
        public Camera MainCamera
        {
            get
            {
                if (mainCamera == null)
                {
                    mainCamera = CameraRootObject.transform.Find("CameraPivotY/MainCamera").GetComponent<Camera>();
                }

                return mainCamera;
            }
            private set
            {
                mainCamera = value;
            }
        }
        #endregion

        #region 目标，一般就是主角
        private Transform targetTransform;
        private Transform TargetTransform
        {
            get
            {
                if (targetTransform == null)
                {
                    var goComp = Parent.GetEComponent<GameObjectComponent>(ComponentType.GameObject);
                    if (goComp != null && goComp.EGameObject != null)
                    {
                        targetTransform = goComp.EGameObject.transform;
                    }
                }
                
                return targetTransform;
            }
        }
        
        private Vector3 TargetCenterPosition
        {
            get
            {
                var moveComp = Parent.GetEComponent<CharacterMoveComponent>(ComponentType.CharacterMove);
                if (moveComp != null)
                {
                    return moveComp.GetCharacterColliderCenter();
                }

                return Vector3.zero;
            }
        }
        #endregion
        
        public CameraComponent() : base(ComponentType.Camera)
        {

        }

        public override void Init()
        {
            CameraPivotYObject.transform.localPosition = new Vector3(0, CAMERA_DEFAULT_PIVOT_Y, 0);
            CameraDefaultZPos = CAMERA_DEFAULT_PIVOT_Z;
            cameraObjectPos = new Vector3(0, 0, CameraDefaultZPos);
        }

        public override void Dispose()
        {
               
        }

        public override void Tick(float deltaTime, params object[] paras)
        {
            
        }

        public override void LateTick(float deltaTime, params object[] paras)
        {
            Move();
            
            RotateHorizontal();
            
            RotateVertical();
            
            FixPosition();
        }

        private void Move()
        {
            CameraRootObject.transform.position = TargetCenterPosition;
        }

        private void RotateHorizontal()
        {
            Vector3 r = CameraRootObject.transform.rotation.eulerAngles;
            if (InputManager.Instance.MouseMoveX != 0)
            {
                HorizontalAngle += InputManager.Instance.MouseMoveX * GameConfig.Instance.CAMERA_ROTATE_SPEED *
                                  Time.deltaTime;
                r.y = HorizontalAngle;
            }

            var moveComp = Parent.GetEComponent<CharacterMoveComponent>(ComponentType.CharacterMove);
            if (moveComp != null)
            {
                float angle = moveComp.GetCurHorizontalAngle();
                if (angle % 180 != 0 )
                {
                    var followAngle = angle * GameConfig.Instance.CAMERA_FOLLOW_ROTATE_SPEED * Time.deltaTime;
                    HorizontalAngle += followAngle;
                    r.y = HorizontalAngle;
                }
            }

            var targetRotation = Quaternion.Euler(r);
            CameraRootObject.transform.rotation = Quaternion.Slerp(CameraRootObject.transform.rotation, targetRotation, Time.deltaTime*GameConfig.Instance.CAMERA_ROTATE_LERP_SPEED);
        }

        private void RotateVertical()
        {
            Vector3 r = CameraPivotYObject.transform.localRotation.eulerAngles;
            if (InputManager.Instance.MouseMoveY != 0)
            {
                VerticalAngle -= InputManager.Instance.MouseMoveY * GameConfig.Instance.CAMERA_ROTATE_SPEED *
                                  Time.deltaTime;
                VerticalAngle = Mathf.Clamp(VerticalAngle, GameConfig.Instance.MIN_CAMERA_PIVOT_Y,
                    GameConfig.Instance.MAX_CAMERA_PIVOT_Y);
                r.x = VerticalAngle;
            }

            var targetRotation = Quaternion.Euler(r);
            CameraPivotYObject.transform.localRotation = Quaternion.Slerp(CameraPivotYObject.transform.localRotation, targetRotation, Time.deltaTime*GameConfig.Instance.CAMERA_ROTATE_LERP_SPEED);
        }

        private void FixPosition()
        {
            //处理墙体碰撞:从枢轴Y向相机发出射线，第一个碰撞点就是相机需要移动到的位置
            var zPos = CameraDefaultZPos;
            Vector3 dir = (MainCamera.transform.position - CameraPivotYObject.transform.position).normalized;
            // Debug.DrawRay(CameraPivotYObject.transform.position, dir, Color.green);
            bool isHit = Physics.SphereCast(CameraPivotYObject.transform.position, CAMERA_COLLIDER_RADIUS, dir,
                out RaycastHit hitInfo, Mathf.Abs(zPos), 1 << 3);
            if (isHit)
            {
                var distance = Vector3.Distance(CameraPivotYObject.transform.position, hitInfo.point);
                zPos = -(distance - CAMERA_COLLIDER_RADIUS);
                zPos = Mathf.Abs(zPos) < CAMERA_COLLIDER_RADIUS ? -CAMERA_COLLIDER_RADIUS : zPos;   //todo:为了避免贴墙太近导致看不到人
            }
            
            cameraObjectPos.z = zPos;
            MainCamera.transform.localPosition = cameraObjectPos;
        }
    }
}
