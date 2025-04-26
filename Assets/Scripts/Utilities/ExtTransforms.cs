// Enders
// Emobile static class Transforms
using UnityEngine;
public static class TransformExtensions
{
    // 0 references
    public static void DestroyChildren(this Transform t, bool destroyImmediately = false)
    {
        foreach (Transform child in t)
        {
            if (destroyImmediately)
                MonoBehaviour.DestroyImmediate(child.gameObject);
            else
                MonoBehaviour.Destroy(child.gameObject);
        }
    }
}