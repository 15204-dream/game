using System;
using System.Collections.Generic;

namespace 糟糕是心动鸭.Dialogue.StateMachine
{
    /// <summary>
    /// 状态转换规则管理器
    /// 定义和管理状态之间的转换规则
    /// </summary>
    public class StateTransitions
    {
        /// <summary>
        /// 转换规则字典
        /// </summary>
        private Dictionary<DialogueState, List<DialogueState>> transitionRules;

        /// <summary>
        /// 转换条件
        /// </summary>
        private Dictionary<string, Func<bool>> transitionConditions;

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public StateTransitions()
        {
            transitionRules = new Dictionary<DialogueState, List<DialogueState>>();
            transitionConditions = new Dictionary<string, Func<bool>>();

            InitializeDefaultRules();
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化默认规则
        /// </summary>
        private void InitializeDefaultRules()
        {
            AddRule(DialogueState.Idle, DialogueState.Starting);
            AddRule(DialogueState.Idle, DialogueState.Loading);

            AddRule(DialogueState.Starting, DialogueState.WaitingForInput);
            AddRule(DialogueState.Starting, DialogueState.AIGenerating);
            AddRule(DialogueState.Starting, DialogueState.Ending);
            AddRule(DialogueState.Starting, DialogueState.Error);

            AddRule(DialogueState.WaitingForInput, DialogueState.ProcessingSelection);
            AddRule(DialogueState.WaitingForInput, DialogueState.Interrupted);
            AddRule(DialogueState.WaitingForInput, DialogueState.Ending);

            AddRule(DialogueState.ProcessingSelection, DialogueState.AIGenerating);
            AddRule(DialogueState.ProcessingSelection, DialogueState.Updating);
            AddRule(DialogueState.ProcessingSelection, DialogueState.Error);

            AddRule(DialogueState.AIGenerating, DialogueState.ShowingResponse);
            AddRule(DialogueState.AIGenerating, DialogueState.WaitingForInput);
            AddRule(DialogueState.AIGenerating, DialogueState.Retrying);
            AddRule(DialogueState.AIGenerating, DialogueState.Error);

            AddRule(DialogueState.ShowingResponse, DialogueState.WaitingForInput);
            AddRule(DialogueState.ShowingResponse, DialogueState.Updating);
            AddRule(DialogueState.ShowingResponse, DialogueState.Ending);

            AddRule(DialogueState.Updating, DialogueState.WaitingForInput);
            AddRule(DialogueState.Updating, DialogueState.AIGenerating);
            AddRule(DialogueState.Updating, DialogueState.Ending);

            AddRule(DialogueState.Resuming, DialogueState.WaitingForInput);
            AddRule(DialogueState.Resuming, DialogueState.Ending);

            AddRule(DialogueState.Interrupted, DialogueState.Idle);
            AddRule(DialogueState.Interrupted, DialogueState.Resuming);
            AddRule(DialogueState.Interrupted, DialogueState.Starting);

            AddRule(DialogueState.Retrying, DialogueState.AIGenerating);
            AddRule(DialogueState.Retrying, DialogueState.Error);
            AddRule(DialogueState.Retrying, DialogueState.Ending);

            AddRule(DialogueState.Error, DialogueState.Idle);
            AddRule(DialogueState.Error, DialogueState.Retrying);
            AddRule(DialogueState.Error, DialogueState.Starting);

            AddRule(DialogueState.Ending, DialogueState.Idle);
            AddRule(DialogueState.Ending, DialogueState.Starting);

            AddRule(DialogueState.Saving, DialogueState.Idle);
            AddRule(DialogueState.Saving, DialogueState.Error);

            AddRule(DialogueState.Loading, DialogueState.Idle);
            AddRule(DialogueState.Loading, DialogueState.Starting);
            AddRule(DialogueState.Loading, DialogueState.Error);
        }

        #endregion

        #region 规则管理

        /// <summary>
        /// 添加转换规则
        /// </summary>
        /// <param name="from">源状态</param>
        /// <param name="to">目标状态</param>
        public void AddRule(DialogueState from, DialogueState to)
        {
            if (!transitionRules.ContainsKey(from))
            {
                transitionRules[from] = new List<DialogueState>();
            }

            if (!transitionRules[from].Contains(to))
            {
                transitionRules[from].Add(to);
            }
        }

        /// <summary>
        /// 移除转换规则
        /// </summary>
        /// <param name="from">源状态</param>
        /// <param name="to">目标状态</param>
        public void RemoveRule(DialogueState from, DialogueState to)
        {
            if (transitionRules.ContainsKey(from))
            {
                transitionRules[from].Remove(to);
            }
        }

        /// <summary>
        /// 检查是否存在转换规则
        /// </summary>
        /// <param name="from">源状态</param>
        /// <param name="to">目标状态</param>
        /// <returns>是否存在</returns>
        public bool HasRule(DialogueState from, DialogueState to)
        {
            if (!transitionRules.ContainsKey(from))
            {
                return false;
            }

            return transitionRules[from].Contains(to);
        }

        /// <summary>
        /// 获取指定状态的转换规则
        /// </summary>
        /// <param name="from">源状态</param>
        /// <returns>目标状态列表</returns>
        public List<DialogueState> GetTransitions(DialogueState from)
        {
            if (!transitionRules.ContainsKey(from))
            {
                return new List<DialogueState>();
            }

            return new List<DialogueState>(transitionRules[from]);
        }

        #endregion

        #region 条件转换

        /// <summary>
        /// 注册转换条件
        /// </summary>
        /// <param name="conditionId">条件ID</param>
        /// <param name="condition">条件函数</param>
        public void RegisterCondition(string conditionId, Func<bool> condition)
        {
            transitionConditions[conditionId] = condition;
        }

        /// <summary>
        /// 移除转换条件
        /// </summary>
        /// <param name="conditionId">条件ID</param>
        public void RemoveCondition(string conditionId)
        {
            transitionConditions.Remove(conditionId);
        }

        /// <summary>
        /// 检查条件
        /// </summary>
        /// <param name="conditionId">条件ID</param>
        /// <returns>条件是否满足</returns>
        public bool CheckCondition(string conditionId)
        {
            if (!transitionConditions.ContainsKey(conditionId))
            {
                return false;
            }

            return transitionConditions[conditionId]();
        }

        #endregion

        #region 转换验证

        /// <summary>
        /// 检查转换是否有效
        /// </summary>
        /// <param name="from">源状态</param>
        /// <param name="to">目标状态</param>
        /// <returns>是否有效</returns>
        public bool IsValidTransition(DialogueState from, DialogueState to)
        {
            if (from == to) return false;

            return HasRule(from, to);
        }

        /// <summary>
        /// 获取有效的转换目标
        /// </summary>
        /// <param name="from">源状态</param>
        /// <returns>有效目标状态列表</returns>
        public List<DialogueState> GetValidTargets(DialogueState from)
        {
            return GetTransitions(from);
        }

        /// <summary>
        /// 获取有效的转换目标（带条件检查）
        /// </summary>
        /// <param name="from">源状态</param>
        /// <param name="conditionId">条件ID</param>
        /// <returns>满足条件的有效目标</returns>
        public List<DialogueState> GetValidTargetsWithCondition(DialogueState from, string conditionId)
        {
            List<DialogueState> validTargets = GetTransitions(from);

            if (!string.IsNullOrEmpty(conditionId) && CheckCondition(conditionId))
            {
                return validTargets;
            }

            return new List<DialogueState>();
        }

        #endregion

        #region 高级查询

        /// <summary>
        /// 检查是否可以到达目标状态
        /// </summary>
        /// <param name="from">源状态</param>
        /// <param name="to">目标状态</param>
        /// <param name="maxHops">最大跳数</param>
        /// <returns>是否可以到达</returns>
        public bool CanReach(DialogueState from, DialogueState to, int maxHops = 5)
        {
            if (from == to) return true;
            if (maxHops <= 0) return false;

            List<DialogueState> targets = GetTransitions(from);
            foreach (DialogueState target in targets)
            {
                if (target == to)
                {
                    return true;
                }

                if (CanReach(target, to, maxHops - 1))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取到目标状态的最短路径
        /// </summary>
        /// <param name="from">源状态</param>
        /// <param name="to">目标状态</param>
        /// <returns>状态列表路径</returns>
        public List<DialogueState> GetShortestPath(DialogueState from, DialogueState to)
        {
            if (from == to)
            {
                return new List<DialogueState> { from };
            }

            Queue<List<DialogueState>> queue = new Queue<List<DialogueState>>();
            HashSet<DialogueState> visited = new HashSet<DialogueState>();

            queue.Enqueue(new List<DialogueState> { from });
            visited.Add(from);

            while (queue.Count > 0)
            {
                List<DialogueState> path = queue.Dequeue();
                DialogueState current = path[path.Count - 1];

                List<DialogueState> targets = GetTransitions(current);
                foreach (DialogueState target in targets)
                {
                    if (target == to)
                    {
                        path.Add(target);
                        return path;
                    }

                    if (!visited.Contains(target))
                    {
                        visited.Add(target);
                        List<DialogueState> newPath = new List<DialogueState>(path);
                        newPath.Add(target);
                        queue.Enqueue(newPath);
                    }
                }
            }

            return new List<DialogueState>();
        }

        /// <summary>
        /// 获取所有可到达的状态
        /// </summary>
        /// <param name="from">源状态</param>
        /// <param name="maxHops">最大跳数</param>
        /// <returns>可到达的状态集合</returns>
        public HashSet<DialogueState> GetAllReachable(DialogueState from, int maxHops = 5)
        {
            HashSet<DialogueState> reachable = new HashSet<DialogueState>();
            CollectReachable(from, reachable, maxHops);
            return reachable;
        }

        /// <summary>
        /// 递归收集可到达状态
        /// </summary>
        private void CollectReachable(DialogueState from, HashSet<DialogueState> reachable, int remainingHops)
        {
            if (remainingHops <= 0) return;

            List<DialogueState> targets = GetTransitions(from);
            foreach (DialogueState target in targets)
            {
                if (!reachable.Contains(target))
                {
                    reachable.Add(target);
                    CollectReachable(target, reachable, remainingHops - 1);
                }
            }
        }

        #endregion

        #region 清除和重置

        /// <summary>
        /// 清除所有规则
        /// </summary>
        public void ClearAllRules()
        {
            transitionRules.Clear();
        }

        /// <summary>
        /// 重置为默认规则
        /// </summary>
        public void ResetToDefaults()
        {
            transitionRules.Clear();
            InitializeDefaultRules();
        }

        #endregion
    }
}
