using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 折线图组件 - 用于显示数据趋势
    /// </summary>
    public class LineChart : StatisticsChart
    {
        [Header("折线图配置")]
        [SerializeField] private Color lineColor = new Color(0.3f, 0.6f, 1f, 1f);
        [SerializeField] private Color fillColor = new Color(0.3f, 0.6f, 1f, 0.2f);
        [SerializeField] private Color pointColor = Color.white;
        [SerializeField] private float lineWidth = 3f;
        [SerializeField] private float pointSize = 6f;
        [SerializeField] private bool showPoints = true;
        [SerializeField] private bool showFill = true;
        [SerializeField] private bool showGradient = true;
        
        [Header("渐变配置")]
        [SerializeField] private Color gradientTop = new Color(0.5f, 0.8f, 1f, 0.5f);
        [SerializeField] private Color gradientBottom = new Color(0.1f, 0.3f, 0.6f, 0f);
        
        [Header("多折线配置")]
        [SerializeField] private bool multiLineMode = false;
        [SerializeField] private List<Color> lineColors = new List<Color>
        {
            new Color(1f, 0.4f, 0.4f),
            new Color(0.4f, 1f, 0.4f),
            new Color(0.4f, 0.4f, 1f),
            new Color(1f, 1f, 0.4f),
            new Color(1f, 0.4f, 1f)
        };
        
        private List<List<float>> multiLineData;
        private List<string> multiLineLabels;
        
        protected override void Awake()
        {
            base.Awake();
            multiLineData = new List<List<float>>();
            multiLineLabels = new List<string>();
        }
        
        /// <summary>
        /// 设置单折线数据
        /// </summary>
        public override void SetData(List<float> values)
        {
            multiLineMode = false;
            base.SetData(values);
        }
        
        /// <summary>
        /// 设置多折线数据
        /// </summary>
        public void SetMultiLineData(List<string> labels, List<List<float>> values)
        {
            multiLineMode = true;
            multiLineLabels = new List<string>(labels);
            multiLineData = new List<List<float>>();
            
            foreach (var line in values)
            {
                multiLineData.Add(new List<float>(line));
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
        /// 获取当前线颜色
        /// </summary>
        protected Color GetCurrentLineColor(int lineIndex = 0)
        {
            if (multiLineMode && lineIndex < lineColors.Count)
            {
                return LerpColor(lineColors[lineIndex] * 0.5f, lineColors[lineIndex], animationProgress);
            }
            return LerpColor(lineColor * 0.5f, lineColor, animationProgress);
        }
        
        /// <summary>
        /// 获取当前填充颜色
        /// </summary>
        protected Color GetCurrentFillColor(int lineIndex = 0)
        {
            if (showGradient)
            {
                Color topColor = gradientTop;
                Color bottomColor = gradientBottom;
                
                if (multiLineMode && lineIndex < lineColors.Count)
                {
                    topColor = Color.Lerp(lineColors[lineIndex] * 0.3f, lineColors[lineIndex], 0.5f);
                    topColor.a = 0.5f;
                    bottomColor = lineColors[lineIndex];
                    bottomColor.a = 0f;
                }
                
                return LerpColor(bottomColor, topColor, animationProgress);
            }
            
            Color fill = showFill ? fillColor : Color.clear;
            return LerpColor(fill * 0.5f, fill, animationProgress);
        }
        
        /// <summary>
        /// 获取点颜色
        /// </summary>
        protected Color GetCurrentPointColor()
        {
            return LerpColor(pointColor * 0.5f, pointColor, animationProgress);
        }
        
        /// <summary>
        /// 获取线宽度
        /// </summary>
        protected float GetCurrentLineWidth()
        {
            return lineWidth * animationProgress;
        }
        
        /// <summary>
        /// 获取点大小
        /// </summary>
        protected float GetCurrentPointSize()
        {
            return pointSize * animationProgress;
        }
        
        /// <summary>
        /// 绘制折线图回调
        /// </summary>
        protected override void OnDataChanged()
        {
            base.OnDataChanged();
        }
        
        /// <summary>
        /// 生成顶点数据
        /// </summary>
        protected virtual List<Vector3> GenerateLinePoints(Rect chartArea, List<float> values, float maxValue)
        {
            List<Vector3> points = new List<Vector3>();
            
            if (values.Count == 0)
                return points;
            
            for (int i = 0; i < values.Count; i++)
            {
                float x = GetDataPointX(i, chartArea);
                float y = GetDataPointY(values[i], maxValue, chartArea);
                points.Add(new Vector3(x, y, 0));
            }
            
            return points;
        }
        
        /// <summary>
        /// 生成填充区域顶点
        /// </summary>
        protected virtual List<Vector3> GenerateFillPoints(Rect chartArea, List<Vector3> linePoints)
        {
            List<Vector3> points = new List<Vector3>(linePoints);
            
            if (points.Count > 0)
            {
                points.Add(new Vector3(points[points.Count - 1].x, chartArea.y, 0));
                points.Add(new Vector3(points[0].x, chartArea.y, 0));
            }
            
            return points;
        }
        
        /// <summary>
        /// 设置线条颜色
        /// </summary>
        public void SetLineColor(Color color)
        {
            lineColor = color;
            OnDataChanged();
        }
        
        /// <summary>
        /// 设置填充颜色
        /// </summary>
        public void SetFillColor(Color color)
        {
            fillColor = color;
            OnDataChanged();
        }
        
        /// <summary>
        /// 获取趋势方向
        /// </summary>
        public TrendDirection GetTrend()
        {
            if (dataValues.Count < 2)
                return TrendDirection.Stable;
            
            float recentAvg = (dataValues[dataValues.Count - 1] + (dataValues.Count > 1 ? dataValues[dataValues.Count - 2] : dataValues[dataValues.Count - 1])) / 2f;
            float olderAvg = (dataValues[0] + (dataValues.Count > 1 ? dataValues[1] : dataValues[0])) / 2f;
            
            if (recentAvg - olderAvg > 5f)
                return TrendDirection.Rising;
            else if (olderAvg - recentAvg > 5f)
                return TrendDirection.Falling;
            else
                return TrendDirection.Stable;
        }
        
        /// <summary>
        /// 获取数据变化百分比
        /// </summary>
        public float GetChangePercentage()
        {
            if (dataValues.Count < 2)
                return 0f;
            
            float first = dataValues[0];
            float last = dataValues[dataValues.Count - 1];
            
            if (Mathf.Abs(first) < 0.001f)
                return 0f;
            
            return ((last - first) / first) * 100f;
        }
    }
    
    /// <summary>
    /// 折线图数据
    /// </summary>
    [Serializable]
    public class LineChartData
    {
        public string lineName;
        public Color lineColor;
        public List<float> values;
        public bool isVisible;
        
        public LineChartData()
        {
            values = new List<float>();
            lineColor = Color.white;
            isVisible = true;
        }
        
        public LineChartData(string name, Color color)
        {
            lineName = name;
            lineColor = color;
            values = new List<float>();
            isVisible = true;
        }
    }
}
