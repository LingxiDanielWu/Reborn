namespace EC
 {
     public enum ComponentType
     {
         None,
         Controller,
         GameObject,
         Move,
         CharacterMove,
         State,
         Animator,
         Camera,
         Event,
         Action,
         Weapon
     }
     
     public abstract class EComponent
     {
         private Entity parent;

         public bool IsActive
         {
             get;
             set;
         }

         protected Entity Parent
         {
             get
             {
                 return parent;
             }
             private set
             {
                 parent = value;
             }
         }
         private ComponentType cType;

         public ComponentType CType
         {
             get
             {
                 return cType;
             }
             protected set
             {
                 cType = value;
             }
         }

         public EComponent(ComponentType type)
         {
             CType = type;
         }

         public virtual void Attach(Entity e)
         {
             parent = e;
             e.AddEComponent(this);
         }

         public abstract void Init();

         public abstract void Dispose();

         public virtual void Tick(float deltaTime, params object[] paras)
         {

         }

         public virtual void LateTick(float deltaTime, params object[] paras)
         {
             
         }

         public virtual void HandleEvent(EventType type, object data)
         {
             
         }

         public void SetActive(bool isActive)
         {
             IsActive = isActive;
         }
     }
 }
