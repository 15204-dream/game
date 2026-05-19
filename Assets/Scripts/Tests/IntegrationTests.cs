using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    /// <summary>
    /// 集成测试 - 测试各系统间的集成和交互
    /// </summary>
    public class IntegrationTests : MonoBehaviour
    {
        private TestResults _results = new TestResults();

        [ContextMenu("运行所有集成测试")]
        public void RunAllTests()
        {
            StartCoroutine(RunAllTestsCoroutine());
        }

        private IEnumerator RunAllTestsCoroutine()
        {
            _results.Clear();

            yield return StartCoroutine(TestEventBus());
            yield return StartCoroutine(TestServiceLocator());
            yield return StartCoroutine(TestGameLoopManager());
            yield return StartCoroutine(TestSceneLoader());
            yield return StartCoroutine(TestCacheManager());
            yield return StartCoroutine(TestObjectPoolManager());
            yield return StartCoroutine(TestPerformanceOptimizer());
            yield return StartCoroutine(TestResourceManager());
            yield return StartCoroutine(TestAudioManager());
            yield return StartCoroutine(TestLocalizationManager());

            PrintResults();
        }

        /// <summary>
        /// 测试事件总线
        /// </summary>
        private IEnumerator TestEventBus()
        {
            var testName = "事件总线集成测试";
            try
            {
                var eventBus = EventBus.Instance;
                if (eventBus == null)
                {
                    throw new Exception("EventBus实例为null");
                }

                bool eventReceived = false;
                eventBus.Subscribe<int>("TestEvent", (data) => eventReceived = true);
                eventBus.Publish("TestEvent", 42);

                yield return new WaitForSeconds(0.1f);

                if (!eventReceived)
                {
                    throw new Exception("事件未被接收");
                }

                _results.AddResult(testName, true, "事件总线工作正常");
            }
            catch (Exception ex)
            {
                _results.AddResult(testName, false, ex.Message);
            }
        }

        /// <summary>
        /// 测试服务定位器
        /// </summary>
        private IEnumerator TestServiceLocator()
        {
            var testName = "服务定位器集成测试";
            try
            {
                var locator = ServiceLocator.Instance;
                if (locator == null)
                {
                    throw new Exception("ServiceLocator实例为null");
                }

                locator.RegisterService<IntegrationTests>(this);
                var service = locator.GetService<IntegrationTests>();
                if (service == null)
                {
                    throw new Exception("服务注册或获取失败");
                }

                _results.AddResult(testName, true, "服务定位器工作正常");
            }
            catch (Exception ex)
            {
                _results.AddResult(testName, false, ex.Message);
            }
        }

        /// <summary>
        /// 测试游戏循环管理器
        /// </summary>
        private IEnumerator TestGameLoopManager()
        {
            var testName = "游戏循环管理器集成测试";
            try
            {
                var loop = GameLoopManager.Instance;
                if (loop == null)
                {
                    throw new Exception("GameLoopManager实例为null");
                }

                var initialState = loop.CurrentState;
                loop.ChangeState(GameLoopManager.GameState.Playing);

                yield return new WaitForSeconds(0.1f);

                if (loop.CurrentState != GameLoopManager.GameState.Playing)
                {
                    throw new Exception("状态切换失败");
                }

                loop.ChangeState(initialState);
                _results.AddResult(testName, true, "游戏循环管理器工作正常");
            }
            catch (Exception ex)
            {
                _results.AddResult(testName, false, ex.Message);
            }
        }

        /// <summary>
        /// 测试场景加载器
        /// </summary>
        private IEnumerator TestSceneLoader()
        {
            var testName = "场景加载器集成测试";
            try
            {
                var loader = SceneLoader.Instance;
                if (loader == null)
                {
                    throw new Exception("SceneLoader实例为null");
                }

                if (loader.IsLoading)
                {
                    throw new Exception("场景加载器状态异常");
                }

                _results.AddResult(testName, true, "场景加载器工作正常");
            }
            catch (Exception ex)
            {
                _results.AddResult(testName, false, ex.Message);
            }
        }

        /// <summary>
        /// 测试缓存管理器
        /// </summary>
        private IEnumerator TestCacheManager()
        {
            var testName = "缓存管理器集成测试";
            try
            {
                var cache = CacheManager.Instance;
                if (cache == null)
                {
                    throw new Exception("CacheManager实例为null");
                }

                cache.Set("test", "key1", "value1");
                var value = cache.Get<string>("test", "key1");

                if (value != "value1")
                {
                    throw new Exception("缓存存取失败");
                }

                cache.Clear("test");
                _results.AddResult(testName, true, "缓存管理器工作正常");
            }
            catch (Exception ex)
            {
                _results.AddResult(testName, false, ex.Message);
            }
        }

        /// <summary>
        /// 测试对象池管理器
        /// </summary>
        private IEnumerator TestObjectPoolManager()
        {
            var testName = "对象池管理器集成测试";
            try
            {
                var pool = ObjectPoolManager.Instance;
                if (pool == null)
                {
                    throw new Exception("ObjectPoolManager实例为null");
                }

                var info = pool.GetAllPoolInfo();
                if (info == null)
                {
                    throw new Exception("获取池信息失败");
                }

                _results.AddResult(testName, true, "对象池管理器工作正常");
            }
            catch (Exception ex)
            {
                _results.AddResult(testName, false, ex.Message);
            }
        }

        /// <summary>
        /// 测试性能优化器
        /// </summary>
        private IEnumerator TestPerformanceOptimizer()
        {
            var testName = "性能优化器集成测试";
            try
            {
                var optimizer = PerformanceOptimizer.Instance;
                if (optimizer == null)
                {
                    throw new Exception("PerformanceOptimizer实例为null");
                }

                var profile = optimizer.GetCurrentProfile();
                if (profile == null)
                {
                    throw new Exception("获取性能配置失败");
                }

                _results.AddResult(testName, true, "性能优化器工作正常");
            }
            catch (Exception ex)
            {
                _results.AddResult(testName, false, ex.Message);
            }
        }

        /// <summary>
        /// 测试资源管理器
        /// </summary>
        private IEnumerator TestResourceManager()
        {
            var testName = "资源管理器集成测试";
            try
            {
                var resource = ResourceManager.Instance;
                if (resource == null)
                {
                    throw new Exception("ResourceManager实例为null");
                }

                int cachedCount = resource.GetCachedCount();
                _results.AddResult(testName, true, $"资源管理器工作正常，缓存数: {cachedCount}");
            }
            catch (Exception ex)
            {
                _results.AddResult(testName, false, ex.Message);
            }
        }

        /// <summary>
        /// 测试音频管理器
        /// </summary>
        private IEnumerator TestAudioManager()
        {
            var testName = "音频管理器集成测试";
            try
            {
                var audio = AudioManager.Instance;
                if (audio == null)
                {
                    throw new Exception("AudioManager实例为null");
                }

                audio.SetMasterVolume(0.5f);
                _results.AddResult(testName, true, "音频管理器工作正常");
            }
            catch (Exception ex)
            {
                _results.AddResult(testName, false, ex.Message);
            }
        }

        /// <summary>
        /// 测试本地化管理器
        /// </summary>
        private IEnumerator TestLocalizationManager()
        {
            var testName = "本地化管理器集成测试";
            try
            {
                var localization = LocalizationManager.Instance;
                if (localization == null)
                {
                    throw new Exception("LocalizationManager实例为null");
                }

                string text = localization.Get("game_title");
                if (string.IsNullOrEmpty(text))
                {
                    throw new Exception("获取本地化文本失败");
                }

                _results.AddResult(testName, true, $"本地化管理器工作正常，语言: {localization.CurrentLanguage}");
            }
            catch (Exception ex)
            {
                _results.AddResult(testName, false, ex.Message);
            }
        }

        /// <summary>
        /// 打印结果
        /// </summary>
        private void PrintResults()
        {
            _results.PrintResults();
        }
    }

    /// <summary>
    /// 测试结果集合
    /// </summary>
    [Serializable]
    public class TestResults
    {
        private List<TestResultItem> _items = new List<TestResultItem>();

        public void AddResult(string testName, bool passed, string message)
        {
            _items.Add(new TestResultItem
            {
                TestName = testName,
                Passed = passed,
                Message = message,
                Timestamp = DateTime.Now
            });
        }

        public void Clear()
        {
            _items.Clear();
        }

        public void PrintResults()
        {
            Debug.Log("========== 集成测试结果 ==========");

            int passed = 0;
            int failed = 0;

            foreach (var item in _items)
            {
                string status = item.Passed ? "✓ 通过" : "✗ 失败";
                string color = item.Passed ? "green" : "red";
                Debug.Log($"[{status}] {item.TestName}: {item.Message}");

                if (item.Passed)
                {
                    passed++;
                }
                else
                {
                    failed++;
                }
            }

            Debug.Log($"===================================");
            Debug.Log($"总计: {_items.Count} | 通过: {passed} | 失败: {failed}");
            Debug.Log($"成功率: {(float)passed / _items.Count:P2}");
            Debug.Log($"===================================");
        }
    }

    /// <summary>
    /// 测试结果项
    /// </summary>
    [Serializable]
    public class TestResultItem
    {
        public string TestName;
        public bool Passed;
        public string Message;
        public DateTime Timestamp;
    }
}
