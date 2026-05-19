using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Performance
{
    /// <summary>
    /// 对象池管理器 - 实现游戏对象的复用
    /// 减少对象创建和销毁带来的性能开销
    /// </summary>
    public class ObjectPoolManager : MonoBehaviour
    {
        private static ObjectPoolManager _instance;
        public static ObjectPoolManager Instance
        {
            get { return _instance; }
        }

        private Dictionary<string, ObjectPool> _pools = new Dictionary<string, ObjectPool>();
        private Dictionary<GameObject, string> _poolNames = new Dictionary<GameObject, string>();

        [Header("对象池配置")]
        [SerializeField] private int _defaultPoolSize = 10;
        [SerializeField] private int _maxPoolSize = 100;
        [SerializeField] private bool _autoExpand = true;
        [SerializeField] private bool _enableLogging = false;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        /// <summary>
        /// 创建对象池
        /// </summary>
        public void CreatePool(string poolName, GameObject prefab, int initialSize)
        {
            if (_pools.ContainsKey(poolName))
            {
                Debug.LogWarning($"对象池已存在: {poolName}");
                return;
            }

            var pool = new ObjectPool(prefab, initialSize, _maxPoolSize, _autoExpand, transform);
            _pools.Add(poolName, pool);

            if (_enableLogging)
            {
                Debug.Log($"对象池已创建: {poolName}, 初始大小: {initialSize}");
            }
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        public GameObject GetObject(string poolName)
        {
            if (!_pools.ContainsKey(poolName))
            {
                Debug.LogError($"对象池不存在: {poolName}");
                return null;
            }

            return _pools[poolName].GetObject();
        }

        /// <summary>
        /// 获取对象（带位置和旋转）
        /// </summary>
        public GameObject GetObject(string poolName, Vector3 position, Quaternion rotation)
        {
            var obj = GetObject(poolName);
            if (obj != null)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
            }
            return obj;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        public void ReturnObject(GameObject obj)
        {
            if (_poolNames.ContainsKey(obj))
            {
                string poolName = _poolNames[obj];
                if (_pools.ContainsKey(poolName))
                {
                    _pools[poolName].ReturnObject(obj);
                }
            }
            else
            {
                Destroy(obj);
            }
        }

        /// <summary>
        /// 预热对象池
        /// </summary>
        public void Prewarm(string poolName, int count)
        {
            if (!_pools.ContainsKey(poolName))
            {
                Debug.LogError($"对象池不存在: {poolName}");
                return;
            }

            _pools[poolName].Prewarm(count);
        }

        /// <summary>
        /// 释放对象池
        /// </summary>
        public void ReleasePool(string poolName)
        {
            if (_pools.ContainsKey(poolName))
            {
                _pools[poolName].Clear();
                _pools.Remove(poolName);

                if (_enableLogging)
                {
                    Debug.Log($"对象池已释放: {poolName}");
                }
            }
        }

        /// <summary>
        /// 释放所有对象池
        /// </summary>
        public void ReleaseAllPools()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Clear();
            }
            _pools.Clear();

            if (_enableLogging)
            {
                Debug.Log("所有对象池已释放");
            }
        }

        /// <summary>
        /// 获取对象池信息
        /// </summary>
        public PoolInfo GetPoolInfo(string poolName)
        {
            if (_pools.ContainsKey(poolName))
            {
                return _pools[poolName].GetInfo();
            }
            return null;
        }

        /// <summary>
        /// 获取所有对象池信息
        /// </summary>
        public Dictionary<string, PoolInfo> GetAllPoolInfo()
        {
            var info = new Dictionary<string, PoolInfo>();
            foreach (var kvp in _pools)
            {
                info[kvp.Key] = kvp.Value.GetInfo();
            }
            return info;
        }

        /// <summary>
        /// 设置最大池大小
        /// </summary>
        public void SetMaxPoolSize(string poolName, int maxSize)
        {
            if (_pools.ContainsKey(poolName))
            {
                _pools[poolName].MaxSize = maxSize;
            }
        }
    }

    /// <summary>
    /// 对象池
    /// </summary>
    public class ObjectPool
    {
        private GameObject _prefab;
        private Queue<GameObject> _availableObjects = new Queue<GameObject>();
        private List<GameObject> _activeObjects = new List<GameObject>();
        private int _maxSize;
        private bool _autoExpand;
        private Transform _parent;

        public int MaxSize
        {
            get { return _maxSize; }
            set { _maxSize = value; }
        }

        public ObjectPool(GameObject prefab, int initialSize, int maxSize, bool autoExpand, Transform parent)
        {
            _prefab = prefab;
            _maxSize = maxSize;
            _autoExpand = autoExpand;
            _parent = parent;

            for (int i = 0; i < initialSize; i++)
            {
                CreateNewObject();
            }
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        public GameObject GetObject()
        {
            GameObject obj;
            if (_availableObjects.Count > 0)
            {
                obj = _availableObjects.Dequeue();
            }
            else if (_autoExpand && _activeObjects.Count + _availableObjects.Count < _maxSize)
            {
                obj = CreateNewObject();
            }
            else
            {
                return null;
            }

            obj.SetActive(true);
            _activeObjects.Add(obj);
            return obj;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        public void ReturnObject(GameObject obj)
        {
            if (!_activeObjects.Contains(obj))
            {
                return;
            }

            obj.SetActive(false);
            _activeObjects.Remove(obj);
            _availableObjects.Enqueue(obj);
        }

        /// <summary>
        /// 创建新对象
        /// </summary>
        private GameObject CreateNewObject()
        {
            var obj = UnityEngine.Object.Instantiate(_prefab, _parent);
            obj.SetActive(false);
            _availableObjects.Enqueue(obj);
            return obj;
        }

        /// <summary>
        /// 预热
        /// </summary>
        public void Prewarm(int count)
        {
            for (int i = 0; i < count && _availableObjects.Count + _activeObjects.Count < _maxSize; i++)
            {
                CreateNewObject();
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            foreach (var obj in _availableObjects)
            {
                UnityEngine.Object.Destroy(obj);
            }
            foreach (var obj in _activeObjects)
            {
                UnityEngine.Object.Destroy(obj);
            }
            _availableObjects.Clear();
            _activeObjects.Clear();
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        public PoolInfo GetInfo()
        {
            return new PoolInfo
            {
                ActiveCount = _activeObjects.Count,
                AvailableCount = _availableObjects.Count,
                TotalCount = _activeObjects.Count + _availableObjects.Count,
                MaxSize = _maxSize
            };
        }
    }

    /// <summary>
    /// 对象池信息
    /// </summary>
    [Serializable]
    public class PoolInfo
    {
        public int ActiveCount;
        public int AvailableCount;
        public int TotalCount;
        public int MaxSize;
    }
}
