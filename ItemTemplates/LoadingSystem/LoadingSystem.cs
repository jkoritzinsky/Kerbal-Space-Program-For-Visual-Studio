using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace $rootnamespace$
{
    class $safeitemname$ : LoadingSystem
    {
        public override bool IsReady()
        {
            return false;
        }
        public override string ProgressTitle()
        {
            return nameof($safeitemname$);
        }
        public override float ProgressFraction()
        {
            return 0f;
        }
        public override void StartLoad()
        {
        }
    }

    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    class $safeitemname$Loader : MonoBehaviour
    {
        void Awake()
        {
            List<LoadingSystem> list = LoadingScreen.Instance.loaders;
            if (list != null)
            {
                // Need to create a GameObject so that Unity will correctly initialize the LoadingSystem
                var gameObject = new GameObject("$safeitemname$");
                var loadingSystem = gameObject.AddComponent<$safeitemname$>();
                list.Add(loadingSystem);
            }
        }
    }
}
