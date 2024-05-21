using System;

public abstract class Singleton<T> where T : class, new() 
{
    private static T m_instance;

    public static T Instance{
        get
        {
            if (m_instance == null)
            {
                m_instance = Activator.CreateInstance<T>();
            }

            return m_instance;
        }
    }

    public virtual void Init()
    {
    }

    public abstract void Dispose();

    public virtual void Tick(float deltaTime)
    {
    }

    public virtual void LateTick(float deltaTime)
    {
    }
}
