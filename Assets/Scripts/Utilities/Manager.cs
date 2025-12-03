using UnityEngine;

public abstract class Manager : MonoBehaviour
{
    public abstract void InitializeManager();
}

public abstract class Manager<T> : Manager where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    [SerializeField]
    private SingletonMode _mode;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetInstance()
    {
        Instance = null;
    }
    
    protected virtual void Awake()
    {
        if (_mode == SingletonMode.None)
        {
            return;
        }

        if (Instance == null)
        {
            Instance = this as T;
            if (_mode == SingletonMode.Persistent)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private enum SingletonMode
    {
        None,
        Singleton,
        Persistent
    }
}