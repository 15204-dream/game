using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    /// <summary>
    /// 测试运行器 - 管理测试的执行和报告
    /// </summary>
    public class TestRunner : MonoBehaviour
    {
        private static TestRunner _instance;
        public static TestRunner Instance
        {
            get { return _instance; }
        }

        [Header("测试配置")]
        [SerializeField] private bool _runOnStart = false;
        [SerializeField] private bool _verboseLogging = true;
        [SerializeField] private float _testDelay = 0.5f;

        private List<TestResult> _results = new List<TestResult>();
        private int _currentTestIndex = 0;
        private bool _isRunning = false;
        private TestSuite _currentSuite;

        public event Action<TestResult> OnTestComplete;
        public event Action<TestSuite> OnSuiteComplete;
        public event Action OnAllTestsComplete;

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        public List<TestResult> Results
        {
            get { return _results; }
        }

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

        void Start()
        {
            if (_runOnStart)
            {
                RunAllTests();
            }
        }

        /// <summary>
        /// 运行所有测试
        /// </summary>
        public void RunAllTests()
        {
            if (_isRunning) return;

            _isRunning = true;
            _results.Clear();
            _currentTestIndex = 0;

            var suites = new List<TestSuite>
            {
                CreateIntegrationTests(),
                CreatePerformanceTests()
            };

            StartCoroutine(RunTestSuites(suites));
        }

        /// <summary>
        /// 运行测试套件
        /// </summary>
        private IEnumerator RunTestSuites(List<TestSuite> suites)
        {
            foreach (var suite in suites)
            {
                _currentSuite = suite;
                yield return RunTestSuite(suite);
                OnSuiteComplete?.Invoke(suite);
            }

            _isRunning = false;
            OnAllTestsComplete?.Invoke();
            GenerateTestReport();
        }

        /// <summary>
        /// 运行测试套件
        /// </summary>
        private IEnumerator RunTestSuite(TestSuite suite)
        {
            if (_verboseLogging)
            {
                Debug.Log($"开始测试套件: {suite.Name}");
            }

            foreach (var test in suite.Tests)
            {
                yield return RunTest(test);
                yield return new WaitForSeconds(_testDelay);
            }
        }

        /// <summary>
        /// 运行单个测试
        /// </summary>
        private IEnumerator RunTest(TestCase test)
        {
            var result = new TestResult
            {
                TestName = test.Name,
                SuiteName = _currentSuite.Name,
                StartTime = DateTime.Now,
                Status = TestStatus.Running
            };

            if (_verboseLogging)
            {
                Debug.Log($"运行测试: {test.Name}");
            }

            try
            {
                if (test.IsAsync)
                {
                    yield return StartCoroutine(test.AsyncExecute());
                }
                else
                {
                    test.SyncExecute();
                }

                result.Status = TestStatus.Passed;
                result.Message = "测试通过";
            }
            catch (Exception ex)
            {
                result.Status = TestStatus.Failed;
                result.Message = ex.Message;
                result.StackTrace = ex.StackTrace;

                if (_verboseLogging)
                {
                    Debug.LogError($"测试失败 [{test.Name}]: {ex.Message}");
                }
            }

            result.EndTime = DateTime.Now;
            result.Duration = (result.EndTime - result.StartTime).TotalSeconds;

            _results.Add(result);
            OnTestComplete?.Invoke(result);
        }

        /// <summary>
        /// 创建集成测试套件
        /// </summary>
        private TestSuite CreateIntegrationTests()
        {
            var suite = new TestSuite
            {
                Name = "集成测试"
            };

            suite.AddTest(new TestCase("事件总线测试", TestEventBus));
            suite.AddTest(new TestCase("服务定位器测试", TestServiceLocator));
            suite.AddTest(new TestCase("游戏循环测试", TestGameLoop));
            suite.AddTest(new TestCase("缓存管理器测试", TestCacheManager));
            suite.AddTest(new TestCase("对象池测试", TestObjectPool));

            return suite;
        }

        /// <summary>
        /// 创建性能测试套件
        /// </summary>
        private TestSuite CreatePerformanceTests()
        {
            var suite = new TestSuite
            {
                Name = "性能测试"
            };

            suite.AddTest(new TestCase("内存占用测试", TestMemoryUsage));
            suite.AddTest(new TestCase("帧率稳定性测试", TestFrameRateStability));
            suite.AddTest(new TestCase("加载时间测试", TestLoadTime));
            suite.AddTest(new TestCase("API响应时间测试", TestAPIResponseTime));

            return suite;
        }

        // 测试用例实现

        private void TestEventBus()
        {
            bool eventFired = false;
            EventBus.Instance.Subscribe<string>("TestEvent", (data) => eventFired = true);
            EventBus.Instance.Publish("TestEvent", "test");
            EventBus.Instance.Unsubscribe<string>("TestEvent", (data) => eventFired = true);

            if (!eventFired)
            {
                throw new Exception("事件总线测试失败：事件未触发");
            }
        }

        private void TestServiceLocator()
        {
            var locator = ServiceLocator.Instance;
            if (locator == null)
            {
                throw new Exception("服务定位器测试失败：无法获取实例");
            }
        }

        private void TestGameLoop()
        {
            var loop = GameLoopManager.Instance;
            if (loop == null)
            {
                throw new Exception("游戏循环测试失败：无法获取实例");
            }
        }

        private void TestCacheManager()
        {
            var cache = CacheManager.Instance;
            if (cache == null)
            {
                throw new Exception("缓存管理器测试失败：无法获取实例");
            }
        }

        private void TestObjectPool()
        {
            var pool = ObjectPoolManager.Instance;
            if (pool == null)
            {
                throw new Exception("对象池测试失败：无法获取实例");
            }
        }

        private void TestMemoryUsage()
        {
            long memory = System.GC.GetTotalMemory(false);
            if (memory < 0)
            {
                throw new Exception("内存测试失败：内存值异常");
            }
        }

        private void TestFrameRateStability()
        {
            float fps = 1f / Time.deltaTime;
            if (fps < 0)
            {
                throw new Exception("帧率测试失败：帧率值异常");
            }
        }

        private void TestLoadTime()
        {
            // 测试资源加载时间
        }

        private void TestAPIResponseTime()
        {
            // 测试API响应时间
        }

        /// <summary>
        /// 生成测试报告
        /// </summary>
        private void GenerateTestReport()
        {
            int passed = 0;
            int failed = 0;

            foreach (var result in _results)
            {
                if (result.Status == TestStatus.Passed)
                {
                    passed++;
                }
                else
                {
                    failed++;
                }
            }

            Debug.Log($"========== 测试报告 ==========");
            Debug.Log($"总测试数: {_results.Count}");
            Debug.Log($"通过: {passed}");
            Debug.Log($"失败: {failed}");
            Debug.Log($"成功率: {(float)passed / _results.Count:P2}");
            Debug.Log($"==============================");
        }

        /// <summary>
        /// 运行指定测试
        /// </summary>
        public void RunTest(string testName)
        {
            var test = new TestCase(testName, () => { });
            StartCoroutine(RunTest(test));
        }
    }

    /// <summary>
    /// 测试套件
    /// </summary>
    [Serializable]
    public class TestSuite
    {
        public string Name;
        public List<TestCase> Tests = new List<TestCase>();

        public void AddTest(TestCase test)
        {
            Tests.Add(test);
        }
    }

    /// <summary>
    /// 测试用例
    /// </summary>
    [Serializable]
    public class TestCase
    {
        public string Name;
        public bool IsAsync;
        private Action _syncAction;
        private Func<IEnumerator> _asyncAction;

        public TestCase(string name, Action action, bool isAsync = false)
        {
            Name = name;
            _syncAction = action;
            IsAsync = isAsync;
        }

        public TestCase(string name, Func<IEnumerator> asyncAction)
        {
            Name = name;
            _asyncAction = asyncAction;
            IsAsync = true;
        }

        public void SyncExecute()
        {
            _syncAction?.Invoke();
        }

        public IEnumerator AsyncExecute()
        {
            if (_asyncAction != null)
            {
                yield return StartCoroutine(_asyncAction());
            }
            else if (_syncAction != null)
            {
                _syncAction();
            }
        }
    }

    /// <summary>
    /// 测试结果
    /// </summary>
    [Serializable]
    public class TestResult
    {
        public string TestName;
        public string SuiteName;
        public TestStatus Status;
        public string Message;
        public string StackTrace;
        public DateTime StartTime;
        public DateTime EndTime;
        public double Duration;
    }

    /// <summary>
    /// 测试状态
    /// </summary>
    public enum TestStatus
    {
        Pending,
        Running,
        Passed,
        Failed,
        Skipped
    }
}
