///<summary>
///Monosingleton script to be used as a base class for any singleton script.
///Helps to avoid the need to create a new singleton script for each singleton class.
///</summary>
using UnityEngine;

public abstract class MonoSingleton<Inherited> : MonoBehaviour where Inherited : MonoSingleton<Inherited>
{
    private static readonly object lockObj = new object();

    //Inherited class to take any inheriting class into effect
    private static Inherited _Instance;
    public static Inherited Instance
    {
        get
        {
            lock (lockObj)
            {
                if (_Instance == null)
                {
                    //Does a search for the existing inheriting class
                    _Instance = (Inherited)FindObjectOfType(typeof(Inherited));

                    //Creates a new instance of the inheriting class if none is found
                    if (_Instance == null)
                    {
                        var singletonObject = new GameObject();
                        _Instance = singletonObject.AddComponent<Inherited>();
                        singletonObject.name = $"{typeof(Inherited)} (Singleton)";
                    }
                }
                return _Instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this as Inherited;
        }
        else if (Instance != this)
        {
            Destroy(this);
            throw new System.ComponentModel.WarningException($"'{typeof(Inherited)}' already exists in the scene. This duplicate on '{gameObject.name}' will be destroyed.");
        }
    }
}