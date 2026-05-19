using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    /// <summary>
    /// 性能测试 - 测试游戏的性能指标
    /// </summary>
    public class PerformanceTests : MonoBehaviour
    {
        private PerformanceTestResults _results = new PerformanceTestResults();

        [Header("性能阈值")]
        public float MinAcceptableFPS = 30f;
        public float MaxAcceptableMemoryMB = 512f;
        public float MaxAcceptableLoadTime = 5f;

        [ContextMenu("运行所有性能测试")]
        public void RunAllTests()
        {
            StartCoroutine(RunAllTestsCoroutine());
        }

        private IEnumerator RunAllTestsCoroutine()
        {
            _results.Clear();

            yield return StartCoroutine(TestMemoryUsage());
            yield return StartCoroutine(TestFrameRateStability());
            yield return StartCoroutine(TestCachePerformance());
            yield return StartCoroutine(TestObjectPoolEfficiency());
            yield return StartCoroutine(TestGCPerformance());
            yield return StartCoroutine(TestResourceLoading());

            PrintResults();
        }

        /// <summary>
        /// 测试内存占用
        /// </summary>
        private IEnumerator TestMemoryUsage()
        {
            var testName = "内存占用测试";
            try
            {
                long memoryBefore = System.GC.GetTotalMemory(false);
                yield return new WaitForSeconds(1f);
                long memoryAfter = System.GC.GetTotalMemory(false);

                float memoryUsedMB = (memoryAfter - memoryBefore) / (1024f * 1024f);
                float totalMemoryMB = memoryAfter / (1024f * 1024f);

                bool passed = totalMemoryMB < MaxAcceptableMemoryMB;

                _results.AddResult(new PerformanceTestResult
                {
                    TestName = testName,
                    Passed = passed,
                    Value = totalMemoryMB,
                    Unit = "MB",
                    Threshold = MaxAcceptableMemoryMB,
                    Message = $"当前内存: {totalMemoryMB:F2}MB / {MaxAcceptableMemoryMB}MB"
                });

                yield return null;
            }
            catch (Exception ex)
            {
                _results.AddResult(new PerformanceTestResult
                {
                    TestName = testName,
                    Passed = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// 测试帧率稳定性
        /// </summary>
        private IEnumerator TestFrameRateStability()
        {
            var testName = "帧率稳定性测试";
            try
            {
                List<float> fpsSamples = new List<float>();
                int sampleCount = 60;

                for (int i = 0; i < sampleCount; i++)
                {
                    float fps = 1f / Time.deltaTime;
                    fpsSamples.Add(fps);
                    yield return new WaitForSeconds(0.1f);
                }

                float averageFPS = 0f;
                float minFPS = float.MaxValue;
                float maxFPS = float.MinValue;

                foreach (var fps in fpsSamples)
                {
                    averageFPS += fps;
                    minFPS = Mathf.Min(minFPS, fps);
                    maxFPS = Mathf.Max(maxFPS, fps);
                }
                averageFPS /= fpsSamples.Count;

                float variance = 0f;
                foreach (var fps in fpsSamples)
                {
                    variance += Mathf.Pow(fps - averageFPS, 2);
                }
                variance /= fpsSamples.Count;
                float stdDev = Mathf.Sqrt(variance);

                bool passed = averageFPS >= MinAcceptableFPS;

                _results.AddResult(new PerformanceTestResult
                {
                    TestName = testName,
                    Passed = passed,
                    Value = averageFPS,
                    Unit = "FPS",
                    Threshold = MinAcceptableFPS,
                    Message = $"平均: {averageFPS:F2}FPS | 最低: {minFPS:F2}FPS | 最高: {maxFPS:F2}FPS | 标准差: {stdDev:F2}"
                });
            }
            catch (Exception ex)
            {
                _results.AddResult(new PerformanceTestResult
                {
                    TestName = testName,
                    Passed = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// 测试缓存性能
        /// </summary>
        private IEnumerator TestCachePerformance()
        {
            var testName = "缓存性能测试";
            try
            {
                var cache = CacheManager.Instance;
                if (cache == null)
                {
                    throw new Exception("CacheManager实例为null");
                }

                int testCount = 10000;
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

                sw.Start();
                for (int i = 0; i < testCount; i++)
                {
                    cache.Set("PerfTest", $"key_{i}", $"value_{i}");
                }
                long setTime = sw.ElapsedMilliseconds;

                sw.Restart();
                for (int i = 0; i < testCount; i++)
                {
                    var value = cache.Get<string>("PerfTest", $"key_{i}");
                }
                long getTime = sw.ElapsedMilliseconds;
                sw.Stop();

                float avgSetTime = (float)setTime / testCount * 1000f;
                float avgGetTime = (float)getTime / testCount * 1000f;

                bool passed = avgGetTime < 1f;

                _results.AddResult(new PerformanceTestResult
                {
                    TestName = testName,
                    Passed = passed,
                    Value = avgGetTime,
                    Unit = "μs",
                    Threshold = 1f,
                    Message = $"设置平均: {avgSetTime:F4}μs | 获取平均: {avgGetTime:F4}μs"
                });

                cache.Clear("PerfTest");
            }
            catch (Exception ex)
            {
                _results.AddResult(new PerformanceTestResult
                {
                    TestName = testName,
                    Passed = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// 测试对象池效率
        /// </summary>
        private IEnumerator TestObjectPoolEfficiency()
        {
            var testName = "对象池效率测试";
            try
            {
                var pool = ObjectPoolManager.Instance;
                if (pool == null)
                {
                    throw new Exception("ObjectPoolManager实例为null");
                }

                var testPrefab = CreateTestPrefab();
                pool.CreatePool("PerfTest", testPrefab, 10);

                int testCount = 1000;
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

                sw.Start();
                for (int i = 0; i < testCount; i++)
                {
                    var obj = pool.GetObject("PerfTest");
                    pool.ReturnObject(obj);
                }
                long pooledTime = sw.ElapsedMilliseconds;

                sw.Restart();
                for (int i = 0; i < testCount; i++)
                {
                    var obj = UnityEngine.Object.Instantiate(testPrefab);
                    UnityEngine.Object.Destroy(obj);
                }
                long normalTime = sw.ElapsedMilliseconds;
                sw.Stop();

                float speedup = (float)normalTime / pooledTime;

                bool passed = speedup > 2f;

                _results.AddResult(new PerformanceTestResult
                {
                    TestName = testName,
                    Passed = passed,
                    Value = speedup,
                    Unit = "x",
                    Threshold = 2f,
                    Message = $"对象池: {pooledTime}ms | 普通: {normalTime}ms | 加速: {speedup:F2}x"
                });

                pool.ReleasePool("PerfTest");
                UnityEngine.Object.Destroy(testPrefab);
            }
            catch (Exception ex)
            {
                _results.AddResult(new PerformanceTestResult
                {
                    TestName = testName,
                    Passed = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// 测试GC性能
        /// </summary>
        private IEnumerator TestGCPerformance()
        {
            var testName = "GC性能测试";
            try
            {
                long memoryBefore = System.GC.GetTotalMemory(false);

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.GC.Collect();

                yield return new WaitForSeconds(1f);

                long memoryAfter = System.GC.GetTotalMemory(false);
                long memoryFreed = memoryBefore - memoryAfter;
                float memoryFreedMB = memoryFreed / (1024f * 1024f);

                bool passed = memoryFreedMB < 100f;

                _results.AddResult(new PerformanceTestResult
                {
                    TestName = testName,
                    Passed = passed,
                    Value = memoryFreedMB,
                    Unit = "MB",
                    Threshold = 100f,
                    Message = $"GC释放内存: {memoryFreedMB:F2}MB"
                });
            }
            catch (Exception ex)
            {
                _results.AddResult(new PerformanceTestResult
                {
                    TestName = testName,
                    Passed = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// 测试资源加载
        /// </summary>
        private IEnumerator TestResourceLoading()
        {
            var testName = "资源加载测试";
            try
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                for (int i = 0; i < 10; i++)
                {
                    Resources.Load<UnityEngine.Object>("non_existent_resource");
                }

                sw.Stop();
                float loadTime = sw.ElapsedMilliseconds / 10f;

                bool passed = loadTime < MaxAcceptableLoadTime * 1000f;

                _results.AddResult(new PerformanceTestResult
                {
                    TestName = testName,
                    Passed = passed,
                    Value = loadTime,
                    Unit = "ms",
                    Threshold = MaxAcceptableLoadTime * 1000f,
                    Message = $"平均加载时间: {loadTime:F2}ms"
                });
            }
            catch (Exception ex)
            {
                _results.AddResult(new PerformanceTestResult
                {
                    TestName = testName,
                    Passed = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// 创建测试用预制体
        /// </summary>
        private GameObject CreateTestPrefab()
        {
            var go = new GameObject("TestPrefab");
            go.AddComponent<RectTransform>();
            return go;
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
    /// 性能测试结果集合
    /// </summary>
    [Serializable]
    public class PerformanceTestResults
    {
        private List<PerformanceTestResult> _results = new List<PerformanceTestResult>();

        public void AddResult(PerformanceTestResult result)
        {
            _results.Add(result);
        }

        public void Clear()
        {
            _results.Clear();
        }

        public void PrintResults()
        {
            Debug.Log("========== 性能测试结果 ==========");

            int passed = 0;
            int failed = 0;

            foreach (var result in _results)
            {
                string status = result.Passed ? "✓ 通过" : "✗ 失败";
                string thresholdInfo = result.Threshold > 0 ? $" | 阈值: {result.Threshold:F2}{result.Unit}" : "";
                Debug.Log($"[{status}] {result.TestName}: {result.Value:F2}{result.Unit}{thresholdInfo}");
                Debug.Log($"         {result.Message}");

                if (result.Passed)
                {
                    passed++;
                }
                else
                {
                    failed++;
                }
            }

            Debug.Log($"==================================");
            Debug.Log($"总计: {_results.Count} | 通过: {passed} | 失败: {failed}");
            Debug.Log($"成功率: {(float)passed / _results.Count:P2}");
            Debug.Log($"==================================");
        }
    }

    /// <summary>
    /// 性能测试结果
    /// </summary>
    [Serializable]
    public class PerformanceTestResult
    {
        public string TestName;
        public bool Passed;
        public float Value;
        public string Unit;
        public float Threshold;
        public string Message;
    }
}
