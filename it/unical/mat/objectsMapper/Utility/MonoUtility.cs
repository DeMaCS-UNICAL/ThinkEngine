
using UnityEngine;

[ExecuteInEditMode]
internal class MonoUtility:MonoBehaviour
{
    void OnEnable()
    {
        Utility.prefabsLoaded = false;
    }
}
