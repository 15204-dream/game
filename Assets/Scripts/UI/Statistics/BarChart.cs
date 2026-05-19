using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 柱状图组件 - 用于显示嘉宾热度排名等数据
    /// </summary>
    public class BarChart : StatisticsChart
    {
        [Header("柱状图配置")]
        [SerializeField] private Color barColor = new Color(0.3f, 0.7f, 1f, 1f);
        [SerializeField] private Color highlightColor = new Color(1f, 0.6f, 0.2f, 1f);
        [SerializeField] private float barWidth = 0.8f;
        [SerializeField] private float barSpacing = 0.2f;
        [SerializeField] private float barCornerRadius = 4f;
        [SerializeField] private bool horizontalLayout = false;
        [SerializeField] private bool showValues = true;
        [SerializeField] private bool showLabels = true;
        [SerializeField] private bool showRank = false;
        
        [Header("渐变配置")]
        [SerializeField] private bool useGradient = true;
        [SerializeField] private Color gradientTop = new Color(0.5f, 0.9f, 1f, 1f);
        [SerializeField] private Color gradientBottom = new Color(0.2f, 0.5f, 0.9f, 1f);
        
        [Header("排名颜色")]
        [SerializeField] private Color rank1Color = new Color(1f, 0.84f, 0f, 1f);
        [SerializeField] private Color rank2Color = new Color(0.78f, 0.78f, 0.78f, 1f);
        [SerializeField] private Color rank3Color = new Color(0.8f, 0.5f, 0.2f, 1f);
        
        [Header("动画配置")]
        [SerializeField] private bool animateFromZero = true;
        [SerializeField] private AnimationCurve growCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        private int highlightedIndex = -1;
        private float currentAnimationProgress = 1f;
        private List<float> targetValues;
        
        protected override void Awake()
        {
            base.Awake();
            targetValues = new List<float>();
        }
        
        /// <summary>
        /// 设置数据
        /// </summary>
        public override void SetData(List<string> labels, List<float> values)
        {
            targetValues = new List<float>(values);
            base.SetData(labels, values);
        }
        
        /// <summary>
        /// 设置数据
        /// </summary>
        public override void SetData(List<float> values)
        {
            targetValues = new List<float>(values);
            base.SetData(values);
        }
        
        /// <summary>
        /// 高亮指定索引的柱子
        /// </summary>
        public void HighlightBar(int index)
        {
            highlightedIndex = index;
            OnDataChanged();
        }
        
        /// <summary>
        /// 清除高亮
        /// </summary>
        public void ClearHighlight()
        {
            highlightedIndex = -1;
            OnDataChanged();
        }
        
        /// <summary>
        /// 获取当前柱子的颜色
        /// </summary>
        protected Color GetBarColor(int index, int rank = -1)
        {
            Color baseColor = barColor;
            
            if (rank >= 1 && rank <= 3)
            {
                switch (rank)
                {
                    case 1: baseColor = rank1Color; break;
                    case 2: baseColor = rank2Color; break;
                    case 3: baseColor = rank3Color; break;
                }
            }
            
            if (index == highlightedIndex)
            {
                baseColor = highlightColor;
            }
            
            float animProgress = enableAnimation ? animationProgress : 1f;
            return LerpColor(baseColor * 0.5f, baseColor, animProgress);
        }
        
        /// <summary>
        /// 获取渐变色
        /// </summary>
        protected Gradient GetBarGradient(Color baseColor)
        {
            Gradient gradient = new Gradient();
            
            Color topColor = useGradient ? gradientTop : baseColor;
            Color bottomColor = useGradient ? gradientBottom : baseColor;
            
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0] = new GradientColorKey(topColor, 0f);
            colorKeys[1] = new GradientColorKey(bottomColor, 1f);
            
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(baseColor.a, 0f);
            alphaKeys[1] = new GradientAlphaKey(baseColor.a, 1f);
            
            gradient.SetKeys(colorKeys, alphaKeys);
            
            return gradient;
        }
        
        /// <summary>
        /// 获取柱子的宽度
        /// </summary>
        protected float GetBarActualWidth(Rect chartArea)
        {
            int count = dataValues.Count;
            if (count == 0)
                return 0;
            
            float totalWidth = horizontalLayout ? chartArea.height : chartArea.width;
            float barTotalSpace = totalWidth / count;
            return barTotalSpace * barWidth;
        }
        
        /// <summary>
        /// 获取柱子的位置
        /// </summary>
        protected float GetBarPosition(int index, Rect chartArea)
        {
            int count = dataValues.Count;
            if (count == 0)
                return 0;
            
            float totalWidth = horizontalLayout ? chartArea.height : chartArea.width;
            float barTotalSpace = totalWidth / count;
            float barActualWidth = barTotalSpace * barWidth;
            float spacing = barTotalSpace * barSpacing;
            
            return index * barTotalSpace + spacing + barActualWidth / 2;
        }
        
        /// <summary>
        /// 获取柱子的高度
        /// </summary>
        protected float GetBarHeight(float value, float maxValue, Rect chartArea)
        {
            if (Mathf.Abs(maxValue) < 0.001f)
                return 0;
            
            float normalizedValue = value / maxValue;
            float height = normalizedValue * (horizontalLayout ? chartArea.width : chartArea.height);
            
            if (enableAnimation && animateFromZero)
            {
                height *= animationProgress;
            }
            
            return height;
        }
        
        /// <summary>
        /// 计算排名
        /// </summary>
        protected int CalculateRank(int index)
        {
            if (dataValues.Count <= 1 || index < 0 || index >= dataValues.Count)
                return -1;
            
            int rank = 1;
            float currentValue = dataValues[index];
            
            for (int i = 0; i < dataValues.Count; i++)
            {
                if (i != index && dataValues[i] > currentValue)
                {
                    rank++;
                }
            }
            
            return rank;
        }
        
        /// <summary>
        /// 获取动画进度
        /// </summary>
        protected float GetGrowProgress()
        {
            if (!enableAnimation || !animateFromZero)
                return 1f;
            
            return growCurve.Evaluate(animationProgress);
        }
        
        /// <summary>
        /// 设置柱状图颜色
        /// </summary>
        public void SetBarColor(Color color)
        {
            barColor = color;
            OnDataChanged();
        }
        
        /// <summary>
        /// 设置高亮颜色
        /// </summary>
        public void SetHighlightColor(Color color)
        {
            highlightColor = color;
            OnDataChanged();
        }
        
        /// <summary>
        /// 获取最大值及其索引
        /// </summary>
        public (int index, float value) GetMaxValue()
        {
            if (dataValues.Count == 0)
                return (-1, 0f);
            
            int maxIndex = 0;
            float maxValue = dataValues[0];
            
            for (int i = 1; i < dataValues.Count; i++)
            {
                if (dataValues[i] > maxValue)
                {
                    maxValue = dataValues[i];
                    maxIndex = i;
                }
            }
            
            return (maxIndex, maxValue);
        }
        
        /// <summary>
        /// 获取最小值及其索引
        /// </summary>
        public (int index, float value) GetMinValue()
        {
            if (dataValues.Count == 0)
                return (-1, 0f);
            
            int minIndex = 0;
            float minValue = dataValues[0];
            
            for (int i = 1; i < dataValues.Count; i++)
            {
                if (dataValues[i] < minValue)
                {
                    minValue = dataValues[i];
                    minIndex = i;
                }
            }
            
            return (minIndex, minValue);
        }
    }
    
    /// <summary>
    /// 柱状图数据
    /// </summary>
    [Serializable]
    public class BarChartData
    {
        public string label;
        public float value;
        public Color barColor;
        public bool isVisible;
        public int rank;
        
        public BarChartData()
        {
            barColor = Color.white;
            isVisible = true;
        }
        
        public BarChartData(string label, float value)
        {
            this.label = label;
            this.value = value;
            this.barColor = Color.white;
            this.isVisible = true;
        }
        
        public BarChartData(string label, float value, Color color)
        {
            this.label = label;
            this.value = value;
            this.barColor = color;
            this.isVisible = true;
        }
    }
}
