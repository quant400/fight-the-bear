using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class GetAllMaterials : EditorWindow
{
    MenuCommand mc;
    [MenuItem("BonusTools/SelectAllMaterialsAndSetShader")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        GetAllMaterials window = (GetAllMaterials)EditorWindow.GetWindow(typeof(GetAllMaterials));
        window.Show();
    }
    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        if (GUILayout.Button("Change shader"))
        {
            DisplayShaderContext(GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.popup));
        }
    }
    private void DisplayShaderContext(Rect r)
    {
        if (mc == null)
            mc = new MenuCommand(this, 0);
        string tmpStr = "Shader \"Hidden/tmp_shdr\"{SubShader{Pass{}}}";
        // Rebuild shader menu:
        ShaderUtil.GetAllShaderInfo();
      
        // Display shader popup:
        EditorUtility.DisplayPopupMenu(r, "CONTEXT/ShaderPopup", mc);

    }
    private void OnSelectedShaderPopup(string command, Shader shader)
    {
        if (shader != null)
        {
            selectAllMaterials(shader);
        }
    }

    // Start is called before the first frame update
    public static void selectAllMaterials(Shader shader)
    {
        List<GameObject> objects = GetAllObjectsOnlyInScene();
        List<Material> materials = new List<Material>();
        foreach (var mat in objects)
        {
            if (mat.GetComponent<MeshRenderer>() != null)
            {
                Material[] mats = mat.GetComponent<MeshRenderer>().sharedMaterials;
                foreach(var m in mats)
                {
                    materials.Add(m);
                }
            }
        }
        if (materials.Count > 0)
        {
            setAllMaterialsToShader(shader, materials);
        }
    }

     static List<GameObject> GetAllObjectsOnlyInScene()
    {
        List<GameObject> objectsInScene = new List<GameObject>();

        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (!EditorUtility.IsPersistent(go.transform.root.gameObject) && !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave))
                objectsInScene.Add(go);
        }

        return objectsInScene;
    }
    static void setAllMaterialsToShader(Shader shader,List<Material> materials)
    {
        foreach(var m in materials)
        {
            m.shader = shader;
        }
    }
    
}
