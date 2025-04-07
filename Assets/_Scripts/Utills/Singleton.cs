using UnityEngine;

[DefaultExecutionOrder(-1)]
public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType<T>();

            if (instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = typeof(T).Name;
                instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this as T;
        DontDestroyOnLoad(gameObject);
    }
}


