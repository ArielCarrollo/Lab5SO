using UnityEngine;

public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    private static T __instance = null;

    public static T Instance
    {
        get
        {
            if (__instance == null)
            {
                T[] results = Resources.FindObjectsOfTypeAll<T>();
                if (results.Length == 0)
                {
                    Debug.LogError("SingletonScriptableObject -> Instance -> results.length is 0 for type " + typeof(T).ToString() + "-");
                    return null;
                }
                if (results.Length > 1)
                {
                    Debug.LogError("SingletonScriptableObject -> Instance -> results.length is greather than 1 for type " + typeof(T).ToString() + "-");
                    return null;
                }
                __instance = results[0];
            }
            return __instance;
        }
    }
}