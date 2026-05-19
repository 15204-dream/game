using System;

namespace 糟糕是心动鸭.Dialogue.StateMachine
{
    /// <summary>
    /// 对话状态枚举
    /// 定义对话系统的所有可能状态
    /// </summary>
    public enum DialogueState
    {
        /// <summary>
        /// 空闲状态
        /// </summary>
        Idle,

        /// <summary>
        /// 开始状态
        /// </summary>
        Starting,

        /// <summary>
        /// 等待玩家输入
        /// </summary>
        WaitingForInput,

        /// <summary>
        /// AI生成中
        /// </summary>
        AIGenerating,

        /// <summary>
        /// 显示响应
        /// </summary>
        ShowingResponse,

        /// <summary>
        /// 处理选择
        /// </summary>
        ProcessingSelection,

        /// <summary>
        /// 更新状态
        /// </summary>
        Updating,

        /// <summary>
        /// 恢复中
        /// </summary>
        Resuming,

        /// <summary>
        /// 被打断
        /// </summary>
        Interrupted,

        /// <summary>
        /// 结束中
        /// </summary>
        Ending,

        /// <summary>
        /// 错误状态
        /// </summary>
        Error,

        /// <summary>
        /// 重试状态
        /// </summary>
        Retrying,

        /// <summary>
        /// 保存状态
        /// </summary>
        Saving,

        /// <summary>
        /// 加载状态
        /// </summary>
        Loading
    }

    /// <summary>
    /// 对话状态扩展类
    /// 提供状态相关的辅助方法
    /// </summary>
    public static class DialogueStateExtensions
    {
        /// <summary>
        /// 获取状态描述
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>描述文本</returns>
        public static string GetDescription(this DialogueState state)
        {
            return state switch
            {
                DialogueState.Idle => "空闲中",
                DialogueState.Starting => "对话开始",
                DialogueState.WaitingForInput => "等待选择",
                DialogueState.AIGenerating => "AI思考中",
                DialogueState.ShowingResponse => "显示回复",
                DialogueState.ProcessingSelection => "处理选择",
                DialogueState.Updating => "更新状态",
                DialogueState.Resuming => "继续对话",
                DialogueState.Interrupted => "对话中断",
                DialogueState.Ending => "对话结束",
                DialogueState.Error => "发生错误",
                DialogueState.Retrying => "重试中",
                DialogueState.Saving => "保存中",
                DialogueState.Loading => "加载中",
                _ => "未知状态"
            };
        }

        /// <summary>
        /// 检查是否为活跃状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>是否活跃</returns>
        public static bool IsActive(this DialogueState state)
        {
            return state == DialogueState.WaitingForInput ||
                   state == DialogueState.ShowingResponse ||
                   state == DialogueState.AIGenerating ||
                   state == DialogueState.Starting;
        }

        /// <summary>
        /// 检查是否为可中断状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>是否可中断</returns>
        public static bool IsInterruptible(this DialogueState state)
        {
            return state == DialogueState.WaitingForInput ||
                   state == DialogueState.Idle;
        }

        /// <summary>
        /// 检查是否为错误状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>是否错误</returns>
        public static bool IsError(this DialogueState state)
        {
            return state == DialogueState.Error;
        }

        /// <summary>
        /// 检查是否为结束状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>是否结束</returns>
        public static bool IsTerminal(this DialogueState state)
        {
            return state == DialogueState.Ending ||
                   state == DialogueState.Idle;
        }

        /// <summary>
        /// 获取状态优先级
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>优先级（越高越重要）</returns>
        public static int GetPriority(this DialogueState state)
        {
            return state switch
            {
                DialogueState.Error => 100,
                DialogueState.AIGenerating => 90,
                DialogueState.Ending => 80,
                DialogueState.Interrupted => 70,
                DialogueState.ShowingResponse => 60,
                DialogueState.ProcessingSelection => 50,
                DialogueState.Updating => 40,
                DialogueState.Resuming => 30,
                DialogueState.Starting => 20,
                DialogueState.WaitingForInput => 10,
                DialogueState.Idle => 0,
                _ => 0
            };
        }
    }
}
