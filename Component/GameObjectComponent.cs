using Cysharp.Text;
using UnityEngine;

namespace EC.Component
{
    public class GameObjectComponent : EComponent
    {
        public GameObject EGameObject
        {
            get;
            private set;
        }

        public GameObjectComponent(EntityType enttType, string resName, GameObject parent = null): base(ComponentType.GameObject)
        {
            LoadAsset(enttType, resName, parent);
        }
        
        public override void Init()
        {
            
        }

        public T BindEntity<T>() where T : Entity
        {
            if (EGameObject.GetComponent<T>() == null)
            {
                return EGameObject.AddComponent<T>();
            }

            return null;
        }

        private void LoadAsset(EntityType enttType, string resName, GameObject parent = null)
        {
            string bundleDir = Utils.GetEnumName(typeof(EntityType), enttType);
            var bundle = AssetBundle.LoadFromFile(ZString.Format("Assets/AssetBundles/prefab/{0}", bundleDir));
            if (bundle != null)
            {
                var prefab = bundle.LoadAsset<GameObject>(resName);
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
