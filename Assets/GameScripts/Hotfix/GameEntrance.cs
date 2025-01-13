using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace AIOFramework
{
    public class GameEntrance : MonoBehaviour
    {
        void Start()
        {
            Log.Info("GameEntrance Start");
        }

        public static void Main()
        {
            Log.Info("GameEntrance Main");
        }
    } 
}

