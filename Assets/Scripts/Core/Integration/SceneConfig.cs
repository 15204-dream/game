using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Core.Integration
{
    /// <summary>
    /// 场景配置 - 定义场景加载的配置参数
    /// </summary>
    [Serializable]
    public class SceneConfig
    {
        public string sceneName;
        public string scenePath;
        public LoadSceneMode loadMode;
        public bool enableLoadingScreen;
        public bool preloadDependencies;
        public LoadScenePriority priority;
        public List<string> requiredScenes;
        public Dictionary<string, object> customData;

        public SceneConfig()
        {
            loadMode = LoadSceneMode.Single;
            enableLoadingScreen = true;
            preloadDependencies = false;
            priority = LoadScenePriority.Normal;
            requiredScenes = new List<string>();
            customData = new Dictionary<string, object>();
        }

        public SceneConfig(string name, string path)
        {
            sceneName = name;
            scenePath = path;
            loadMode = LoadSceneMode.Single;
            enableLoadingScreen = true;
            preloadDependencies = false;
            priority = LoadScenePriority.Normal;
            requiredScenes = new List<string>();
            customData = new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// 加载场景优先级
    /// </summary>
    public enum LoadScenePriority
    {
        Low = 0,
        Normal = 1,
        High = 2
    }

    /// <summary>
    /// 场景加载进度数据
    /// </summary>
    [Serializable]
    public class SceneLoadProgress
    {
        public string sceneName;
        public float progress;
        public bool isComplete;
        public string status;
        public float startTime;
        public float endTime;

        public SceneLoadProgress(string name)
        {
            sceneName = name;
            progress = 0f;
            isComplete = false;
            status = "准备中";
            startTime = Time.time;
        }

        public float elapsedTime
        {
            get { return isComplete ? endTime - startTime : Time.time - startTime; }
        }
    }
}
