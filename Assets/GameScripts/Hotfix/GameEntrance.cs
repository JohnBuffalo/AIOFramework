using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityGameFramework.Runtime;
using YooAsset;

namespace AIOFramework
{
    public class GameEntrance : MonoBehaviour
    {
        private async void Start()
        {
            Log.Info("GameEntrance Start");

            await LoadPrefabTest();
            await ListTest();
        }

        private async UniTask ListTest()
        {
            var m_list = new List<GameObject>();
            m_list.Add(new GameObject("1"));
            m_list.Add(new GameObject("2"));
            m_list.Add(new GameObject("3"));
            Log.Info($"List length {m_list.Count}");
        }

        private async UniTask LoadPrefabTest()
        {
            Log.Info("LoadPrefabTest Start");

            var handler = YooAssets.LoadAssetAsync<GameObject>("Effects_CubeTest");
            await handler.ToUniTask();

            var prefab = handler.AssetObject as GameObject;
            Instantiate(prefab,Vector3.left,Quaternion.identity );
            Instantiate(prefab,Vector3.right,Quaternion.identity );
            Log.Info("LoadPrefabTest Done");
        }
    } 
}

