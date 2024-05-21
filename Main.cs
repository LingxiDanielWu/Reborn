using EC.Manager;
using UnityEngine;

namespace EC
{
    public class Main : MonoSingleton<Main>
    {
        void Awake()
        {
        }

        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
            InitManagers();
            GameConfig.Instance.Init();

            InitPlayer();
        }

        private void InitPlayer()
        {
            var player = EntityFactory.Instance.CreateEntity(EntityType.Player, GameObject.Find("Terrain"),
                "Character/Prefab/ModularCharacters/MC01");
            EntityManager.Instance.AddEntity(player);
        }

        void InitManagers()
        {
            EntityManager.Instance.Init();
            InputManager.Instance.Init();
        }

        void DisposeManagers()
        {
            InputManager.Instance.Dispose();
            EntityManager.Instance.Dispose();
        }

        // Update is called once per frame
        void Update()
        {
            TickManagers(Time.deltaTime);
        }

        private void LateUpdate()
        {
            LateTickManagers(Time.deltaTime);
        }

        private void TickManagers(float deltaTime)
        {
            InputManager.Instance.Tick(deltaTime);
            EntityManager.Instance.Tick(deltaTime);
        }

        private void LateTickManagers(float deltaTime)
        {
            InputManager.Instance.LateTick(deltaTime);
            EntityManager.Instance.LateTick(deltaTime);
        }

        private void OnDestroy()
        {
            DisposeManagers();
        }
    }
}
