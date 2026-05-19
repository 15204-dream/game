using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 饼图组件 - 用于显示结局概率等数据
    /// </summary>
    public class PieChart : StatisticsChart
    {
        [Header("饼图配置")]
        [SerializeField] private Color[] segmentColors = new Color[]
        {
            new Color(1f, 0.4f, 0.4f),
            new Color(0.4f, 1f, 0.4f),
            new Color(0.4f, 0.4f, 1f),
            new Color(1f, 1f, 0.4f),
            new Color(1f, 0.4f, 1f),
            new Color(0.4f, 1f, 1f)
        };
        [SerializeField] private bool showPercentage = true;
        [SerializeField] private bool showLabels = true;
        [SerializeField] private bool showLegend = false;
        [SerializeField] private bool drawFromCenter = false;
        [SerializeField] private float innerRadius = 0f;
        [SerializeField] private float startAngle = 0f;
        [SerializeField] private float segmentGap = 2f;
        
        [Header("标签配置")]
        [SerializeField] private Color labelColor = Color.white;
        [SerializeField] private float labelDistance = 1.1f;
        [SerializeField] private bool outsideLabels = true;
        
        [Header("动画配置")]
        [SerializeField] private bool animateExpand = true;
        [SerializeField] private AnimationCurve expandCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float expandedScale = 1.1f;
        
        private int highlightedSegment = -1;
        private List<float> percentages;
        private List<float> angles;
        
        protected override void Awake()
        {
            base.Awake();
            percentages = new List<float>();
            angles = new List<float>();
        }
        
        /// <summary>
        /// 设置数据
        /// </summary>
        public override void SetData(List<string> labels, List<float> values)
        {
            CalculatePercentages(values);
            CalculateAngles();
            base.SetData(labels, values);
        }
        
        /// <summary>
        /// 设置数据
        /// </summary>
        public override void SetData(List<float> values)
        {
            CalculatePercentages(values);
            CalculateAngles();
            base.SetData(values);
        }
        
        /// <summary>
        /// 计算百分比
        /// </summary>
        private void CalculatePercentages(List<float> values)
        {
            percentages.Clear();
            
            if (values == null || values.Count == 0)
                return;
            
            float total = 0f;
            foreach (var value in values)
            {
                total += Mathf.Max(0, value);
            }
            
            if (total < 0.001f)
            {
                foreach (var value in values)
                {
                    percentages.Add(0f);
                }
                return;
            }
            
            foreach (var value in values)
            {
                percentages.Add(Mathf.Max(0, value) / total * 100f);
            }
        }
        
        /// <summary>
        /// 计算角度
        /// </summary>
        private void CalculateAngles()
        {
            angles.Clear();
            
            float currentAngle = startAngle;
            foreach (var percentage in percentages)
            {
                angles.Add(currentAngle);
                currentAngle += percentage / 100f * 360f;
            }
        }
        
        /// <summary>
        /// 高亮指定段
        /// </summary>
        public void HighlightSegment(int index)
        {
            highlightedSegment = index;
            OnDataChanged();
        }
        
        /// <summary>
        /// 清除高亮
        /// </summary>
        public void ClearHighlight()
        {
            highlightedSegment = -1;
            OnDataChanged();
        }
        
        /// <summary>
        /// 获取段颜色
        /// </summary>
        protected Color GetSegmentColor(int index)
        {
            if (index < 0 || index >= segmentColors.Length)
                return Color.gray;
            
            Color color = segmentColors[index];
            
            if (index == highlightedSegment)
            {
                color = Color.Lerp(color, Color.white, 0.3f);
            }
            
            return color;
        }
        
        /// <summary>
        /// 获取段角度范围
        /// </summary>
        protected (float start, float end) GetSegmentAngles(int index)
        {
            if (index < 0 || index >= angles.Count || index >= percentages.Count)
                return (0, 0);
            
            float startAngle = angles[index];
            float sweepAngle = percentages[index] / 100f * 360f;
            float endAngle = startAngle + sweepAngle;
            
            return (startAngle, endAngle);
        }
        
        /// <summary>
        /// 获取段缩放
        /// </summary>
        protected float GetSegmentScale(int index)
        {
            if (!animateExpand || index != highlightedSegment)
                return 1f;
            
            return Mathf.Lerp(1f, expandedScale, animationProgress);
        }
        
        /// <summary>
        /// 获取段的内半径
        /// </summary>
        protected float GetSegmentInnerRadius(float radius)
        {
            return radius * innerRadius;
        }
        
        /// <summary>
        /// 生成扇形顶点
        /// </summary>
        protected List<Vector3> GenerateSegmentVertices(Vector2 center, float radius, float innerRadius, float startAngle, float endAngle, int segments = 32)
        {
            List<Vector3> vertices = new List<Vector3>();
            
            float angleStep = (endAngle - startAngle) / segments;
            
            for (int i = 0; i <= segments; i++)
            {
                float angle = startAngle + i * angleStep;
                float x = center.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                float y = center.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
                vertices.Add(new Vector3(x, y, 0));
            }
            
            if (innerRadius > 0)
            {
                for (int i = segments; i >= 0; i--)
                {
                    float angle = startAngle + i * angleStep;
                    float x = center.x + innerRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
                    float y = center.y + innerRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
                    vertices.Add(new Vector3(x, y, 0));
                }
            }
            
            return vertices;
        }
        
        /// <summary>
        /// 设置段颜色
        /// </summary>
        public void SetSegmentColor(int index, Color color)
        {
            if (index >= 0 && index < segmentColors.Length)
            {
                segmentColors[index] = color;
                OnDataChanged();
            }
        }
        
        /// <summary>
        /// 设置所有段颜色
        /// </summary>
        public void SetSegmentColors(Color[] colors)
        {
            if (colors != null && colors.Length > 0)
            {
                segmentColors = colors;
                OnDataChanged();
            }
        }
        
        /// <summary>
        /// 获取百分比
        /// </summary>
        public List<float> GetPercentages()
        {
            return new List<float>(percentages);
        }
        
        /// <summary>
        /// 获取指定段的百分比
        /// </summary>
        public float GetPercentage(int index)
        {
            if (index >= 0 && index < percentages.Count)
                return percentages[index];
            return 0f;
        }
    }
    
    /// <summary>
    /// 饼图数据
    /// </summary>
    [Serializable]
    public class PieChartData
    {
        public string label;
        public float value;
        public Color color;
        public float percentage;
        public bool isVisible;
        
        public PieChartData()
        {
            color = Color.white;
            isVisible = true;
        }
        
        public PieChartData(string label, float value)
        {
            this.label = label;
            this.value = value;
            this.color = Color.white;
            this.isVisible = true;
        }
        
        public PieChartData(string label, float value, Color color)
        {
            this.label = label;
            this.value = value;
            this.color = color;
            this.isVisible = true;
        }
    }
    
    /// <summary>
    /// 饼图段
    /// </summary>
    [Serializable]
    public class PieChartSegment
    {
        public string label;
        public float value;
        public Color color;
        public float startAngle;
        public float sweepAngle;
        
        public PieChartSegment()
        {
            color = Color.white;
        }
    }
}
