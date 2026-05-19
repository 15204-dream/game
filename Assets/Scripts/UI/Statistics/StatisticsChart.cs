using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 图表基类 - 所有图表组件的基类
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public abstract class StatisticsChart : MonoBehaviour
    {
        [Header("图表基础配置")]
        [SerializeField] protected Color backgroundColor = new Color(0.1f, 0.1f, 0.15f, 0.9f);
        [SerializeField] protected Color gridColor = new Color(0.3f, 0.3f, 0.35f, 0.5f);
        [SerializeField] protected Color labelColor = Color.white;
        [SerializeField] protected float padding = 20f;
        [SerializeField] protected int maxDataPoints = 50;
        
        [Header("动画配置")]
        [SerializeField] protected bool enableAnimation = true;
        [SerializeField] protected float animationDuration = 0.5f;
        [SerializeField] protected AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        protected RectTransform rectTransform;
        protected List<float> dataValues;
        protected List<string> dataLabels;
        protected bool isInitialized;
        protected float animationProgress;
        
        public List<float> DataValues => new List<float>(dataValues);
        public List<string> DataLabels => new List<string>(dataLabels);
        
        protected virtual void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            dataValues = new List<float>();
            dataLabels = new List<string>();
            isInitialized = false;
        }
        
        protected virtual void Start()
        {
            Initialize();
        }
        
        /// <summary>
        /// 初始化图表
        /// </summary>
        public virtual void Initialize()
        {
            if (isInitialized)
                return;
            
            isInitialized = true;
        }
        
        /// <summary>
        /// 设置数据
        /// </summary>
        public virtual void SetData(List<float> values)
        {
            if (values == null)
                values = new List<float>();
            
            dataValues = new List<float>(values);
            
            if (dataValues.Count > maxDataPoints)
            {
                dataValues.RemoveRange(0, dataValues.Count - maxDataPoints);
            }
            
            if (enableAnimation)
            {
                animationProgress = 0f;
            }
            else
            {
                animationProgress = 1f;
            }
            
            OnDataChanged();
        }
        
        /// <summary>
        /// 设置数据（带标签）
        /// </summary>
        public virtual void SetData(List<string> labels, List<float> values)
        {
            dataLabels = new List<string>(labels);
            SetData(values);
        }
        
        /// <summary>
        /// 添加数据点
        /// </summary>
        public virtual void AddDataPoint(float value, string label = "")
        {
            dataValues.Add(value);
            dataLabels.Add(label);
            
            if (dataValues.Count > maxDataPoints)
            {
                dataValues.RemoveAt(0);
                if (dataLabels.Count > 0)
                {
                    dataLabels.RemoveAt(0);
                }
            }
            
            if (enableAnimation)
            {
                animationProgress = 0f;
            }
            
            OnDataChanged();
        }
        
        /// <summary>
        /// 清除所有数据
        /// </summary>
        public virtual void ClearData()
        {
            dataValues.Clear();
            dataLabels.Clear();
            OnDataChanged();
        }
        
        /// <summary>
        /// 数据变化回调
        /// </summary>
        protected virtual void OnDataChanged()
        {
        }
        
        protected virtual void Update()
        {
            if (enableAnimation && animationProgress < 1f)
            {
                animationProgress += Time.deltaTime / animationDuration;
                animationProgress = Mathf.Clamp01(animationProgress);
                OnAnimationUpdate();
            }
        }
        
        /// <summary>
        /// 动画更新
        /// </summary>
        protected virtual void OnAnimationUpdate()
        {
        }
        
        /// <summary>
        /// 获取图表区域
        /// </summary>
        protected Rect GetChartArea()
        {
            float width = rectTransform.rect.width - padding * 2;
            float height = rectTransform.rect.height - padding * 2;
            return new Rect(padding, padding, width, height);
        }
        
        /// <summary>
        /// 获取数据点的X坐标
        /// </summary>
        protected float GetDataPointX(int index, Rect chartArea)
        {
            if (dataValues.Count <= 1)
                return chartArea.x + chartArea.width / 2;
            
            float step = chartArea.width / (dataValues.Count - 1);
            return chartArea.x + index * step;
        }
        
        /// <summary>
        /// 获取数据点的Y坐标
        /// </summary>
        protected float GetDataPointY(float value, float maxValue, Rect chartArea)
        {
            if (Mathf.Abs(maxValue) < 0.001f)
                return chartArea.y + chartArea.height / 2;
            
            float normalizedValue = Mathf.Clamp01(value / maxValue);
            return chartArea.y + normalizedValue * chartArea.height;
        }
        
        /// <summary>
        /// 计算数据的最大值
        /// </summary>
        protected float CalculateMaxValue()
        {
            if (dataValues.Count == 0)
                return 1f;
            
            float max = float.MinValue;
            foreach (var value in dataValues)
            {
                if (value > max)
                    max = value;
            }
            
            return max > 0 ? max : 1f;
        }
        
        /// <summary>
        /// 计算数据的最小值
        /// </summary>
        protected float CalculateMinValue()
        {
            if (dataValues.Count == 0)
                return 0f;
            
            float min = float.MaxValue;
            foreach (var value in dataValues)
            {
                if (value < min)
                    min = value;
            }
            
            return min < 0 ? min : 0f;
        }
        
        /// <summary>
        /// 获取当前动画值
        /// </summary>
        protected float GetAnimatedValue(float targetValue)
        {
            if (!enableAnimation)
                return targetValue;
            
            float curveValue = animationCurve.Evaluate(animationProgress);
            return targetValue * curveValue;
        }
        
        /// <summary>
        /// 获取插值颜色
        /// </summary>
        protected Color LerpColor(Color start, Color end, float t)
        {
            return Color.Lerp(start, end, animationCurve.Evaluate(t));
        }
        
        /// <summary>
        /// 刷新图表
        /// </summary>
        public virtual void Refresh()
        {
            OnDataChanged();
        }
    }
    
    /// <summary>
    /// 图表数据点
    /// </summary>
    [Serializable]
    public class ChartDataPoint
    {
        public float value;
        public string label;
        public Color color;
        public bool isHighlighted;
        
        public ChartDataPoint()
        {
            value = 0f;
            label = "";
            color = Color.white;
            isHighlighted = false;
        }
        
        public ChartDataPoint(float value, string label = "", Color? color = null)
        {
            this.value = value;
            this.label = label;
            this.color = color ?? Color.white;
            this.isHighlighted = false;
        }
    }
    
    /// <summary>
    /// 图表配置
    /// </summary>
    [Serializable]
    public class ChartConfig
    {
        public string title;
        public Color titleColor = Color.white;
        public bool showGrid = true;
        public bool showLabels = true;
        public bool showLegend = false;
        public int gridLinesX = 5;
        public int gridLinesY = 5;
        public float lineWidth = 2f;
        public bool fillArea = true;
        public Color fillColor = new Color(0.3f, 0.5f, 1f, 0.3f);
    }
    
    /// <summary>
    /// 图表样式
    /// </summary>
    [Serializable]
    public class ChartStyle
    {
        public Color backgroundColor = new Color(0.1f, 0.1f, 0.15f, 0.9f);
        public Color gridColor = new Color(0.3f, 0.3f, 0.35f, 0.5f);
        public Color labelColor = Color.white;
        public Color axisColor = Color.white;
        public float axisWidth = 2f;
        public float labelFontSize = 12f;
        public string fontName = "Arial";
    }
}
