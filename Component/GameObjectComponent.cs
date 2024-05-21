using UnityEngine;

namespace EC.Component
{
    public class GameObjectComponent : EComponent
    {
        public string ResName
        {
            get;
            private set;
        }

        public GameObject ObjParent
        {
            get;
            private set;
        }

        public GameObject EGameObject
        {
            get;
            private set;
        }

        public GameObjectComponent(Entity e, string resName, GameObject parent = null): base(ComponentType.GameObject, e)
        {
            ResName = resName;
            ObjParent = parent;
            LoadAsset(ObjParent);
        }

        public override void Init()
        {
            
        }

        private void LoadAsset(GameObject parent = null)
        {
            var prefab = Resources.Load<GameObject>(ResName);
            if (parent != null)
            {
                EGameObject = GameObject.Instantiate(prefab, parent.transform);
            }
            else
            {
                EGameObject = GameObject.Instantiate(prefab);
            }
            EGameObject.transform.position = new Vector3(0, 10, 0);
        }
        
        public override void Dispose()
        {
            if (EGameObject != null)
            {
                GameObject.Destroy(EGameObject);
                EGameObject = null;
            }
        }
    }
}
