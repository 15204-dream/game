using System;
using System.Collections.Generic;

namespace 糟糕是心动鸭.Dialogue.StateMachine
{
    /// <summary>
    /// 对话状态机
    /// 管理对话状态的转换和历史
    /// </summary>
    public class DialogueStateMachine
    {
        /// <summary>
        /// 当前状态
        /// </summary>
        private DialogueState currentState;

        /// <summary>
        /// 前一个状态
        /// </summary>
        private DialogueState previousState;

        /// <summary>
        /// 状态历史
        /// </summary>
        private List<DialogueStateHistoryEntry> stateHistory;

        /// <summary>
        /// 状态转换规则
        /// </summary>
        private StateTransitions transitions;

        /// <summary>
        /// 状态进入时间
        /// </summary>
        private DateTime stateEnterTime;

        /// <summary>
        /// 最大历史记录数
        /// </summary>
        private const int MAX_HISTORY = 20;

        /// <summary>
        /// 状态改变事件
        /// </summary>
        public event Action<DialogueState, DialogueState> OnStateChanged;

        /// <summary>
        /// 状态进入事件
        /// </summary>
        public event Action<DialogueState> OnStateEnter;

        /// <summary>
        /// 状态退出事件
        /// </summary>
        public event Action<DialogueState> OnStateExit;

        #region 属性访问器

        /// <summary>
        /// 获取当前状态
        /// </summary>
        public DialogueState CurrentState => currentState;

        /// <summary>
        /// 获取前一个状态
        /// </summary>
        public DialogueState PreviousState => previousState;

        /// <summary>
        /// 获取状态历史
        /// </summary>
        public IReadOnlyList<DialogueStateHistoryEntry> StateHistory => stateHistory.AsReadOnly();

        /// <summary>
        /// 获取当前状态的持续时间
        /// </summary>
        public TimeSpan CurrentStateDuration => DateTime.Now - stateEnterTime;

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DialogueStateMachine()
        {
            currentState = DialogueState.Idle;
            previousState = DialogueState.Idle;
            stateHistory = new List<DialogueStateHistoryEntry>();
            transitions = new StateTransitions();
            stateEnterTime = DateTime.Now;
        }

        #endregion

        #region 状态转换

        /// <summary>
        /// 转换到指定状态
        /// </summary>
        /// <param name="newState">新状态</param>
        /// <returns>是否转换成功</returns>
        public bool TransitionTo(DialogueState newState)
        {
            if (!transitions.IsValidTransition(currentState, newState))
            {
                return false;
            }

            DialogueState oldState = currentState;

            OnStateExit?.Invoke(oldState);

            previousState = currentState;
            currentState = newState;
            stateEnterTime = DateTime.Now;

            AddToHistory(oldState, newState);

            OnStateEnter?.Invoke(newState);
            OnStateChanged?.Invoke(oldState, newState);

            return true;
        }

        /// <summary>
        /// 强制转换到指定状态
        /// </summary>
        /// <param name="newState">新状态</param>
        public void ForceTransitionTo(DialogueState newState)
        {
            OnStateExit?.Invoke(currentState);

            previousState = currentState;
            currentState = newState;
            stateEnterTime = DateTime.Now;

            AddToHistory(previousState, newState);

            OnStateEnter?.Invoke(newState);
            OnStateChanged?.Invoke(previousState, newState);
        }

        /// <summary>
        /// 返回到前一个状态
        /// </summary>
        /// <returns>是否成功</returns>
        public bool RevertToPrevious()
        {
            if (previousState == currentState)
            {
                return false;
            }

            return TransitionTo(previousState);
        }

        /// <summary>
        /// 添加到历史记录
        /// </summary>
        /// <param name="from">源状态</param>
        /// <param name="to">目标状态</param>
        private void AddToHistory(DialogueState from, DialogueState to)
        {
            DialogueStateHistoryEntry entry = new DialogueStateHistoryEntry
            {
                FromState = from,
                ToState = to,
                Timestamp = DateTime.Now,
                Duration = DateTime.Now - stateEnterTime
            };

            stateHistory.Add(entry);

            if (stateHistory.Count > MAX_HISTORY)
            {
                stateHistory.RemoveAt(0);
            }
        }

        #endregion

        #region 状态查询

        /// <summary>
        /// 检查是否可以转换到指定状态
        /// </summary>
        /// <param name="targetState">目标状态</param>
        /// <returns>是否可以转换</returns>
        public bool CanTransitionTo(DialogueState targetState)
        {
            return transitions.IsValidTransition(currentState, targetState);
        }

        /// <summary>
        /// 获取可用的转换目标状态
        /// </summary>
        /// <returns>可用状态列表</returns>
        public List<DialogueState> GetAvailableTransitions()
        {
            return transitions.GetValidTargets(currentState);
        }

        /// <summary>
        /// 检查是否处于指定状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>是否处于该状态</returns>
        public bool IsInState(DialogueState state)
        {
            return currentState == state;
        }

        /// <summary>
        /// 检查是否处于任意一个指定状态
        /// </summary>
        /// <param name="states">状态数组</param>
        /// <returns>是否处于其中之一</returns>
        public bool IsInAnyState(params DialogueState[] states)
        {
            foreach (DialogueState state in states)
            {
                if (currentState == state)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取状态持续时间（秒）
        /// </summary>
        /// <returns>持续时间</returns>
        public double GetStateDurationSeconds()
        {
            return CurrentStateDuration.TotalSeconds;
        }

        /// <summary>
        /// 获取状态持续时间（毫秒）
        /// </summary>
        /// <returns>持续时间</returns>
        public long GetStateDurationMilliseconds()
        {
            return CurrentStateDuration.TotalMilliseconds;
        }

        #endregion

        #region 历史操作

        /// <summary>
        /// 获取状态转换历史
        /// </summary>
        /// <param name="count">数量</param>
        /// <returns>历史记录</returns>
        public List<DialogueStateHistoryEntry> GetHistory(int count = 10)
        {
            if (count <= 0) return new List<DialogueStateHistoryEntry>();

            int start = System.Math.Max(0, stateHistory.Count - count);
            int length = System.Math.Min(count, stateHistory.Count - start);

            return stateHistory.GetRange(start, length);
        }

        /// <summary>
        /// 获取状态统计
        /// </summary>
        /// <returns>状态统计字典</returns>
        public Dictionary<DialogueState, int> GetStateStatistics()
        {
            Dictionary<DialogueState, int> stats = new Dictionary<DialogueState, int>();

            foreach (DialogueStateHistoryEntry entry in stateHistory)
            {
                if (!stats.ContainsKey(entry.ToState))
                {
                    stats[entry.ToState] = 0;
                }
                stats[entry.ToState]++;
            }

            return stats;
        }

        /// <summary>
        /// 获取最常见的状态
        /// </summary>
        /// <returns>最常见状态</returns>
        public DialogueState GetMostCommonState()
        {
            if (stateHistory.Count == 0)
            {
                return currentState;
            }

            Dictionary<DialogueState, int> stats = GetStateStatistics();

            DialogueState mostCommon = currentState;
            int maxCount = 0;

            foreach (var kvp in stats)
            {
                if (kvp.Value > maxCount)
                {
                    maxCount = kvp.Value;
                    mostCommon = kvp.Key;
                }
            }

            return mostCommon;
        }

        /// <summary>
        /// 清除历史
        /// </summary>
        public void ClearHistory()
        {
            stateHistory.Clear();
        }

        #endregion

        #region 重置

        /// <summary>
        /// 重置状态机
        /// </summary>
        public void Reset()
        {
            previousState = currentState;
            currentState = DialogueState.Idle;
            stateEnterTime = DateTime.Now;
            stateHistory.Clear();

            OnStateChanged?.Invoke(previousState, currentState);
            OnStateEnter?.Invoke(currentState);
        }

        #endregion
    }

    /// <summary>
    /// 状态历史记录条目
    /// </summary>
    [Serializable]
    public class DialogueStateHistoryEntry
    {
        /// <summary>
        /// 源状态
        /// </summary>
        public DialogueState FromState;

        /// <summary>
        /// 目标状态
        /// </summary>
        public DialogueState ToState;

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp;

        /// <summary>
        /// 持续时间
        /// </summary>
        public TimeSpan Duration;

        /// <summary>
        /// 获取描述
        /// </summary>
        /// <returns>描述文本</returns>
        public string GetDescription()
        {
            return $"{FromState} -> {ToState} ({Duration.TotalSeconds:F2}s)";
        }
    }
}
