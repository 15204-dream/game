using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 热力图组件 - 用于显示互动频率等数据
    /// </summary>
    public class HeatMap : StatisticsChart
    {
        [Header("热力图配置")]
        [SerializeField] private int rows = 7;
        [SerializeField] private int columns = 24;
        [SerializeField] private Color lowColor = new Color(0.2f, 0.4f, 0.8f, 1f);
        [SerializeField] private Color midColor = new Color(1f, 1f, 0.2f, 1f);
        [SerializeField] private Color highColor = new Color(1f, 0.3f, 0.3f, 1f);
        [SerializeField] private bool showValues = false;
        [SerializeField] private bool showLabels = true;
        [SerializeField] private bool showLegend = true;
        [SerializeField] private float cellSpacing = 2f;
        
        [Header("行标签（星期）")]
        [SerializeField] private string[] rowLabels = new string[] { "周一", "周二", "周三", "周四", "周五", "周六", "周日" };
        
        [Header("列标签（小时）")]
        [SerializeField] private string[] columnLabels = new string[] {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11",
            "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23"
        };
        
        [Header("图例配置")]
        [SerializeField] private RectTransform legendRect;
        [SerializeField] private TextMeshProUGUI legendLowText;
        [SerializeField] private TextMeshProUGUI legendMidText;
        [SerializeField] private TextMeshProUGUI legendHighText;
        
        [Header("交互配置")]
        [SerializeField] private bool enableHover = true;
        [SerializeField] private bool enableClick = true;
        
        private List<List<float>> heatData;
        private float maxHeatValue = 1f;
        private int hoveredRow = -1;
        private int hoveredColumn = -1;
        
        public event Action<int, int, float> OnCellClicked;
        public event Action<int, int, float> OnCellHovered;
        
        protected override void Awake()
        {
            base.Awake();
            InitializeHeatData();
        }
        
        /// <summary>
        /// 初始化热力图数据
        /// </summary>
        private void InitializeHeatData()
        {
            heatData = new List<List<float>>();
            for (int i = 0; i < rows; i++)
            {
                List<float> row = new List<float>();
                for (int j = 0; j < columns; j++)
                {
                    row.Add(0f);
                }
                heatData.Add(row);
            }
        }
        
        /// <summary>
        /// 设置热力图数据
        /// </summary>
        public void SetHeatData(List<List<float>> data)
        {
            if (data == null || data.Count == 0)
                return;
            
            rows = data.Count;
            columns = data.Count > 0 ? data[0].Count : 0;
            
            heatData = new List<List<float>>();
            maxHeatValue = 0f;
            
            foreach (var row in data)
            {
                List<float> newRow = new List<float>();
                foreach (var value in row)
                {
                    newRow.Add(value);
                    if (value > maxHeatValue)
                        maxHeatValue = value;
                }
                heatData.Add(newRow);
            }
            
            List<float> flatValues = new List<float>();
            foreach (var row in heatData)
            {
                flatValues.AddRange(row);
            }
            
            base.SetData(flatValues);
        }
        
        /// <summary>
        /// 设置单个单元格数据
        /// </summary>
        public void SetCellValue(int row, int column, float value)
        {
            if (row < 0 || row >= rows || column < 0 || column >= columns)
                return;
            
            heatData[row][column] = value;
            
            if (value > maxHeatValue)
                maxHeatValue = value;
            
            OnDataChanged();
        }
        
        /// <summary>
        /// 获取单元格数据
        /// </summary>
        public float GetCellValue(int row, int column)
        {
            if (row < 0 || row >= rows || column < 0 || column >= columns)
                return 0f;
            
            return heatData[row][column];
        }
        
        /// <summary>
        /// 获取单元格颜色
        /// </summary>
        protected Color GetCellColor(float value)
        {
            if (maxHeatValue < 0.001f)
                return lowColor;
            
            float normalizedValue = value / maxHeatValue;
            
            if (normalizedValue < 0.5f)
            {
                return Color.Lerp(lowColor, midColor, normalizedValue * 2f);
            }
            else
            {
                return Color.Lerp(midColor, highColor, (normalizedValue - 0.5f) * 2f);
            }
        }
        
        /// <summary>
        /// 获取单元格颜色（带动画）
        /// </summary>
        protected Color GetAnimatedCellColor(float value)
        {
            Color baseColor = GetCellColor(value);
            return Color.Lerp(baseColor * 0.5f, baseColor, animationProgress);
        }
        
        /// <summary>
        /// 获取单元格矩形
        /// </summary>
        protected Rect GetCellRect(int row, int column, Rect chartArea)
        {
            float cellWidth = (chartArea.width - cellSpacing * (columns - 1)) / columns;
            float cellHeight = (chartArea.height - cellSpacing * (rows - 1)) / rows;
            
            float x = chartArea.x + column * (cellWidth + cellSpacing);
            float y = chartArea.y + (rows - 1 - row) * (cellHeight + cellSpacing);
            
            return new Rect(x, y, cellWidth, cellHeight);
        }
        
        /// <summary>
        /// 获取鼠标下的单元格索引
        /// </summary>
        protected (int row, int column) GetCellAtPosition(Vector2 mousePosition, Rect chartArea)
        {
            float cellWidth = (chartArea.width - cellSpacing * (columns - 1)) / columns;
            float cellHeight = (chartArea.height - cellSpacing * (rows - 1)) / rows;
            
            int column = Mathf.FloorToInt((mousePosition.x - chartArea.x) / (cellWidth + cellSpacing));
            int row = rows - 1 - Mathf.FloorToInt((mousePosition.y - chartArea.y) / (cellHeight + cellSpacing));
            
            if (column < 0 || column >= columns || row < 0 || row >= rows)
                return (-1, -1);
            
            return (row, column);
        }
        
        /// <summary>
        /// 处理鼠标进入单元格
        /// </summary>
        public void OnCellMouseEnter(int row, int column)
        {
            if (!enableHover)
                return;
            
            hoveredRow = row;
            hoveredColumn = column;
            
            float value = GetCellValue(row, column);
            OnCellHovered?.Invoke(row, column, value);
        }
        
        /// <summary>
        /// 处理鼠标点击单元格
        /// </summary>
        public void OnCellClicked(int row, int column)
        {
            if (!enableClick)
                return;
            
            float value = GetCellValue(row, column);
            OnCellClicked?.Invoke(row, column, value);
        }
        
        /// <summary>
        /// 处理鼠标离开
        /// </summary>
        public void OnMouseExit()
        {
            hoveredRow = -1;
            hoveredColumn = -1;
        }
        
        /// <summary>
        /// 获取行标签
        /// </summary>
        public string GetRowLabel(int row)
        {
            if (row >= 0 && row < rowLabels.Length)
                return rowLabels[row];
            return "";
        }
        
        /// <summary>
        /// 获取列标签
        /// </summary>
        public string GetColumnLabel(int column)
        {
            if (column >= 0 && column < columnLabels.Length)
                return columnLabels[column];
            return "";
        }
        
        /// <summary>
        /// 设置行标签
        /// </summary>
        public void SetRowLabels(string[] labels)
        {
            if (labels != null && labels.Length > 0)
            {
                rowLabels = labels;
                rows = labels.Length;
                InitializeHeatData();
                OnDataChanged();
            }
        }
        
        /// <summary>
        /// 设置列标签
        /// </summary>
        public void SetColumnLabels(string[] labels)
        {
            if (labels != null && labels.Length > 0)
            {
                columnLabels = labels;
                columns = labels.Length;
                InitializeHeatData();
                OnDataChanged();
            }
        }
        
        /// <summary>
        /// 设置颜色方案
        /// </summary>
        public void SetColorScheme(Color low, Color mid, Color high)
        {
            lowColor = low;
            midColor = mid;
            highColor = high;
            OnDataChanged();
        }
        
        /// <summary>
        /// 获取热点单元格
        /// </summary>
        public List<(int row, int column, float value)> GetHotSpots(float threshold = 0.8f)
        {
            List<(int row, int column, float value)> hotSpots = new List<(int, int, float)>();
            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    float normalizedValue = maxHeatValue > 0 ? heatData[i][j] / maxHeatValue : 0;
                    if (normalizedValue >= threshold)
                    {
                        hotSpots.Add((i, j, heatData[i][j]));
                    }
                }
            }
            
            return hotSpots;
        }
        
        /// <summary>
        /// 获取行统计数据
        /// </summary>
        public List<float> GetRowStatistics()
        {
            List<float> rowStats = new List<float>();
            
            for (int i = 0; i < rows; i++)
            {
                float sum = 0f;
                for (int j = 0; j < columns; j++)
                {
                    sum += heatData[i][j];
                }
                rowStats.Add(sum);
            }
            
            return rowStats;
        }
        
        /// <summary>
        /// 获取列统计数据
        /// </summary>
        public List<float> GetColumnStatistics()
        {
            List<float> colStats = new List<float>();
            
            for (int j = 0; j < columns; j++)
            {
                float sum = 0f;
                for (int i = 0; i < rows; i++)
                {
                    sum += heatData[i][j];
                }
                colStats.Add(sum);
            }
            
            return colStats;
        }
    }
    
    /// <summary>
    /// 热力图数据
    /// </summary>
    [Serializable]
    public class HeatMapData
    {
        public int rows;
        public int columns;
        public List<HeatMapRow> data;
        public float maxValue;
        
        public HeatMapData()
        {
            data = new List<HeatMapRow>();
        }
    }
    
    /// <summary>
    /// 热力图行数据
    /// </summary>
    [Serializable]
    public class HeatMapRow
    {
        public string label;
        public List<float> values;
        
        public HeatMapRow()
        {
            values = new List<float>();
        }
    }
}
