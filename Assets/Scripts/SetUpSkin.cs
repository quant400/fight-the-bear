using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SetUpSkin : MonoBehaviour
{
    SkinnedMeshRenderer[] _meshRenderer;
    private Transform rootBone;
    //Material[] mat;
    private void Awake()
    {
        _meshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>();
        //mat =smr.material;
    }

    public void SetUpChar(string n)
    {
        GameObject skin = Resources.Load(Path.Combine("SinglePlayerPrefabs/DisplayModels", n)) as GameObject;
        var newMeshRenderers = skin.GetComponentsInChildren<SkinnedMeshRenderer>();
        transform.GetChild(8).gameObject.SetActive(false);
        transform.GetChild(9).gameObject.SetActive(false);
        transform.GetChild(10).gameObject.SetActive(false);

        for (int i = 0; i < newMeshRenderers.Length; i++)
        {
            // update mesh
            //_meshRenderer.sharedMesh = newMeshRenderer.sharedMesh;
            if (newMeshRenderers[i].sharedMaterials.Length > 1)
            {
                _meshRenderer[i].sharedMaterials = newMeshRenderers[i].sharedMaterials;

            }
            else
            {
                _meshRenderer[i].material.mainTexture = newMeshRenderers[i].sharedMaterial.mainTexture;
            }


            _meshRenderer[i].sharedMesh = newMeshRenderers[i].sharedMesh;

            Transform[] childrens = transform.GetComponentsInChildren<Transform>(true);

            // sort bones.
            Transform[] bones = new Transform[newMeshRenderers[i].bones.Length];
            for (int boneOrder = 0; boneOrder < newMeshRenderers[i].bones.Length; boneOrder++)
            {
                bones[boneOrder] = Array.Find<Transform>(childrens, c => c.name == newMeshRenderers[i].bones[boneOrder].name);
            }
            _meshRenderer[i].bones = bones;

            rootBone = _meshRenderer[i].rootBone;

            _meshRenderer[i].gameObject.name = newMeshRenderers[i].gameObject.name;
        }
        // UpdateWearables();

    }

    /* public void UpdateWearables()
     {
         //TODO Suleman: Uncomment Later
         //wearablesWorn = gameplayView.instance.equipedWearables.Split(',');

         //Debug.Log("Update Called: " + wearablesWorn[0]);
         transform.GetChild(8).gameObject.SetActive(false);
         transform.GetChild(9).gameObject.SetActive(false);
         transform.GetChild(10).gameObject.SetActive(false);

         int childIndex;

         GameObject wearable;

         SkinnedMeshRenderer spawnedSkinnedMeshRenderer;

         foreach (string wearableWorn in wearablesWorn)
         {
             var x = wearableWorn.Split('_');

             Debug.Log("WearableModels/" + x[0]);
             GameObject instantiatedWearable = Resources.Load(Path.Combine("WearableModels/" + x[0], x[1])) as GameObject;

             // GameObject  = Instantiate(modelToInstantiate);

             childIndex = GetIndex(x[0]);

             if (childIndex != -1)
             {
                 wearable = instantiatedWearable.transform.GetChild(1).gameObject;
                 foreach (string color in colors)
                 {
                     if (transform.GetChild(childIndex).name.Contains(color))
                     {
                         foreach (Transform child in instantiatedWearable.transform)
                         {
                             if (child.name.Contains(color))
                             {
                                 wearable = child.gameObject;
                             }
                         }
                     }
                 }

                 spawnedSkinnedMeshRenderer = wearable.GetComponent<SkinnedMeshRenderer>();

                 _meshRenderer[GetIndex(x[0]) - 3].sharedMaterial = spawnedSkinnedMeshRenderer.sharedMaterial;

                 if (spawnedSkinnedMeshRenderer.sharedMaterials.Length > 1)
                 {

                     _meshRenderer[GetIndex(x[0]) - 3].sharedMaterials = spawnedSkinnedMeshRenderer.sharedMaterials;

                 }
                 else
                 {
                     _meshRenderer[GetIndex(x[0]) - 3].material.mainTexture = spawnedSkinnedMeshRenderer.sharedMaterial.mainTexture;
                 }


                 _meshRenderer[GetIndex(x[0]) - 3].sharedMesh = spawnedSkinnedMeshRenderer.sharedMesh;
             }

             if (childIndex >= 8)
             {
                 transform.GetChild(childIndex).gameObject.SetActive(true);
             }

             //Destroy(instantiatedWearable);
         }

     }


     private int GetIndex(string wearableType)
     {
         switch (wearableType)
         {
             case "Gloves":
                 return 3;
             case "Shorts":
                 return 5;
             case "Shoes":
                 return 6;
             case "Belts":
                 return 8;
             case "masks":
                 return 9;
             case "glasses":
                 return 10;
         }

         return -1;
     }*/
}
