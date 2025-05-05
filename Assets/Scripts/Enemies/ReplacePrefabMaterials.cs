using UnityEditor;
using UnityEngine;

public static class ReplacePrefabMaterials
{
    //[MenuItem("Tools/Replace Materials In Prefab")]
    //static void ReplaceMaterialsInPrefab()
    //{
    //    // Ruta relativa al Assets de tu prefab y tu material
    //    const string prefabPath    = "Assets/Prefabs/Lobby/armour.prefab";
    //    const string materialPath  = "Assets/Materials/Lobby/shaderBasement.mat";
    //
    //    // Carga assets
    //    var prefab   = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
    //    var newMat   = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
    //    if (prefab == null || newMat == null)
    //    {
    //        Debug.LogError("No se encontró el prefab o el material en las rutas indicadas.");
    //        return;
    //    }
    //
    //    // Abre el prefab para edición
    //    var prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
    //    var renderers  = prefabRoot.GetComponentsInChildren<Renderer>(true);
    //
    //    // Reemplaza todos los materiales
    //    foreach (var rend in renderers)
    //    {
    //        var mats = rend.sharedMaterials;
    //        for (int i = 0; i < mats.Length; i++)
    //            mats[i] = newMat;
    //        rend.sharedMaterials = mats;
    //    }
    //
    //    // Marca cambios y guarda
    //    PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
    //    PrefabUtility.UnloadPrefabContents(prefabRoot);
    //
    //    Debug.Log($"Reemplazados {renderers.Length} Renderers en {prefabPath}");
    //}
}
