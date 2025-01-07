using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace AIOFramework
{
    public partial class Entrance : MonoBehaviour
    {
        private void Start()
        {
            InitBuiltinComponents();
            InitCustomComponents();
            
            OpenLoginPage().Forget();
        }

        //打开入口页面
        private async UniTaskVoid OpenLoginPage()
        {
            ResourceRequest request = Resources.LoadAsync<GameObject>("PatchPage");
            var prefab = await request.ToUniTask();
            if (prefab == null)
            {
                Log.Error("Failed to load LoginPage prefab.");
                return;
            }

            GameObject.Instantiate(prefab,UI.transform.Find("Canvas"));
        }
    }
}
