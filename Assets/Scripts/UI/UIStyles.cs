using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;
using TMPro;

/// <summary>
/// UI样式定义 - 统一的UI视觉样式配置
/// </summary>
public static class UIStyles
{
    #region 颜色定义
    
    /// <summary>
    /// 主色调
    /// </summary>
    public static class PrimaryColors
    {
        public static Color Pink = new Color(1f, 0.6f, 0.7f, 1f);
        public static Color LightPink = new Color(1f, 0.8f, 0.85f, 1f);
        public static Color DarkPink = new Color(0.9f, 0.4f, 0.5f, 1f);
        public static Color Rose = new Color(1f, 0.5f, 0.6f, 1f);
        public static Color Coral = new Color(1f, 0.7f, 0.6f, 1f);
        public static Color Peach = new Color(1f, 0.9f, 0.8f, 1f);
    }
    
    /// <summary>
    /// 辅助色调
    /// </summary>
    public static class SecondaryColors
    {
        public static Color Lavender = new Color(0.9f, 0.85f, 1f, 1f);
        public static Color Mint = new Color(0.6f, 1f, 0.8f, 1f);
        public static Color SkyBlue = new Color(0.6f, 0.85f, 1f, 1f);
        public static Color Cream = new Color(1f, 0.98f, 0.95f, 1f);
        public static Color PeachCream = new Color(1f, 0.95f, 0.9f, 1f);
    }
    
    /// <summary>
    /// 中性色
    /// </summary>
    public static class NeutralColors
    {
        public static Color White = new Color(1f, 1f, 1f, 1f);
        public static Color OffWhite = new Color(1f, 0.99f, 0.98f, 1f);
        public static Color LightGray = new Color(0.95f, 0.95f, 0.95f, 1f);
        public static Color Gray = new Color(0.6f, 0.6f, 0.6f, 1f);
        public static Color DarkGray = new Color(0.3f, 0.3f, 0.3f, 1f);
        public static Color Black = new Color(0.2f, 0.2f, 0.2f, 1f);
    }
    
    /// <summary>
    /// 状态色
    /// </summary>
    public static class StatusColors
    {
        public static Color Success = new Color(0.4f, 1f, 0.5f, 1f);
        public static Color Warning = new Color(1f, 0.8f, 0.3f, 1f);
        public static Color Error = new Color(1f, 0.4f, 0.4f, 1f);
        public static Color Info = new Color(0.4f, 0.7f, 1f, 1f);
        public static Color Disabled = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }
    
    /// <summary>
    /// 好感度颜色
    /// </summary>
    public static class AffectionColors
    {
        public static Color Cold = new Color(0.7f, 0.8f, 1f, 1f);
        public static Color Normal = new Color(1f, 0.85f, 0.7f, 1f);
        public static Color Warm = new Color(1f, 0.7f, 0.6f, 1f);
        public static Color Hot = new Color(1f, 0.5f, 0.5f, 1f);
        public static Color OnFire = new Color(1f, 0.3f, 0.3f, 1f);
    }
    
    #endregion
    
    #region 字体定义
    
    /// <summary>
    /// 字体配置
    /// </summary>
    public static class Fonts
    {
        public static TMP_FontAsset PrimaryFont;
        public static TMP_FontAsset SecondaryFont;
        public static TMP_FontAsset DisplayFont;
        
        public static void LoadFonts()
        {
            // TODO: 从Resources加载字体资源
            // PrimaryFont = Resources.Load<TMP_FontAsset>("Fonts/PrimaryFont");
            // SecondaryFont = Resources.Load<TMP_FontAsset>("Fonts/SecondaryFont");
            // DisplayFont = Resources.Load<TMP_FontAsset>("Fonts/DisplayFont");
        }
    }
    
    #endregion
    
    #region 按钮样式
    
    /// <summary>
    /// 按钮样式配置
    /// </summary>
    public static class ButtonStyles
    {
        /// <summary>
        /// 主按钮样式
        /// </summary>
        public static ButtonStyle Primary => new ButtonStyle
        {
            BackgroundColor = PrimaryColors.Pink,
            TextColor = NeutralColors.White,
            BorderColor = PrimaryColors.DarkPink,
            BorderWidth = 2f,
            FontSize = 24,
            Padding = new RectOffset(20, 20, 12, 12),
            CornerRadius = 15f,
            FontStyle = FontStyles.Bold
        };
        
        /// <summary>
        /// 次要按钮样式
        /// </summary>
        public static ButtonStyle Secondary => new ButtonStyle
        {
            BackgroundColor = SecondaryColors.Cream,
            TextColor = DarkGray,
            BorderColor = PrimaryColors.Pink,
            BorderWidth = 1f,
            FontSize = 22,
            Padding = new RectOffset(18, 18, 10, 10),
            CornerRadius = 12f,
            FontStyle = FontStyles.Normal
        };
        
        /// <summary>
        /// 轮廓按钮样式
        /// </summary>
        public static ButtonStyle Outline => new ButtonStyle
        {
            BackgroundColor = Color.clear,
            TextColor = PrimaryColors.Pink,
            BorderColor = PrimaryColors.Pink,
            BorderWidth = 2f,
            FontSize = 24,
            Padding = new RectOffset(20, 20, 12, 12),
            CornerRadius = 15f,
            FontStyle = FontStyles.Bold
        };
        
        /// <summary>
        /// 文字按钮样式
        /// </summary>
        public static ButtonStyle Text => new ButtonStyle
        {
            BackgroundColor = Color.clear,
            TextColor = PrimaryColors.Pink,
            BorderColor = Color.clear,
            BorderWidth = 0f,
            FontSize = 22,
            Padding = new RectOffset(10, 10, 5, 5),
            CornerRadius = 0f,
            FontStyle = FontStyles.Normal
        };
    }
    
    /// <summary>
    /// 按钮样式类
    /// </summary>
    public static class FontStyles
    {
        public const int Normal = 0;
        public const int Bold = 1;
        public const int Italic = 2;
    }
    
    public class ButtonStyle
    {
        public Color BackgroundColor { get; set; }
        public Color TextColor { get; set; }
        public Color BorderColor { get; set; }
        public float BorderWidth { get; set; }
        public int FontSize { get; set; }
        public RectOffset Padding { get; set; }
        public float CornerRadius { get; set; }
        public int FontStyle { get; set; }
    }
    
    #endregion
    
    #region 面板样式
    
    /// <summary>
    /// 面板样式配置
    /// </summary>
    public static class PanelStyles
    {
        /// <summary>
        /// 普通面板样式
        /// </summary>
        public static PanelStyle Normal => new PanelStyle
        {
            BackgroundColor = new Color(1f, 0.98f, 0.98f, 0.95f),
            BorderColor = PrimaryColors.LightPink,
            BorderWidth = 2f,
            CornerRadius = 20f,
            ShadowEnabled = true,
            ShadowColor = new Color(0f, 0f, 0f, 0.1f),
            ShadowOffset = new Vector2(0f, 4f),
            ShadowBlur = 8f
        };
        
        /// <summary>
        /// 弹窗面板样式
        /// </summary>
        public static PanelStyle Popup => new PanelStyle
        {
            BackgroundColor = new Color(1f, 0.97f, 0.97f, 0.98f),
            BorderColor = PrimaryColors.Pink,
            BorderWidth = 3f,
            CornerRadius = 25f,
            ShadowEnabled = true,
            ShadowColor = new Color(0f, 0f, 0f, 0.2f),
            ShadowOffset = new Vector2(0f, 8f),
            ShadowBlur = 16f
        };
        
        /// <summary>
        /// 半透明面板样式
        /// </summary>
        public static PanelStyle Translucent => new PanelStyle
        {
            BackgroundColor = new Color(1f, 0.95f, 0.95f, 0.8f),
            BorderColor = PrimaryColors.LightPink,
            BorderWidth = 1f,
            CornerRadius = 15f,
            ShadowEnabled = false
        };
    }
    
    /// <summary>
    /// 面板样式类
    /// </summary>
    public class PanelStyle
    {
        public Color BackgroundColor { get; set; }
        public Color BorderColor { get; set; }
        public float BorderWidth { get; set; }
        public float CornerRadius { get; set; }
        public bool ShadowEnabled { get; set; }
        public Color ShadowColor { get; set; }
        public Vector2 ShadowOffset { get; set; }
        public float ShadowBlur { get; set; }
    }
    
    #endregion
    
    #region 文本样式
    
    /// <summary>
    /// 文本样式配置
    /// </summary>
    public static class TextStyles
    {
        /// <summary>
        /// 标题文本样式
        /// </summary>
        public static TextStyle Title => new TextStyle
        {
            FontSize = 36,
            Color = PrimaryColors.DarkPink,
            FontStyle = FontStyles.Bold,
            Alignment = TextAlignmentOptions.Center,
            LineSpacing = 0f
        };
        
        /// <summary>
        /// 副标题文本样式
        /// </summary>
        public static TextStyle Subtitle => new TextStyle
        {
            FontSize = 28,
            Color = NeutralColors.DarkGray,
            FontStyle = FontStyles.Normal,
            Alignment = TextAlignmentOptions.Center,
            LineSpacing = 0f
        };
        
        /// <summary>
        /// 正文文本样式
        /// </summary>
        public static TextStyle Body => new TextStyle
        {
            FontSize = 22,
            Color = NeutralColors.DarkGray,
            FontStyle = FontStyles.Normal,
            Alignment = TextAlignmentOptions.TopLeft,
            LineSpacing = 8f
        };
        
        /// <summary>
        /// 对话文本样式
        /// </summary>
        public static TextStyle Dialogue => new TextStyle
        {
            FontSize = 24,
            Color = new Color(0.3f, 0.2f, 0.25f, 1f),
            FontStyle = FontStyles.Normal,
            Alignment = TextAlignmentOptions.TopLeft,
            LineSpacing = 10f
        };
        
        /// <summary>
        /// 角色名称文本样式
        /// </summary>
        public static TextStyle CharacterName => new TextStyle
        {
            FontSize = 26,
            Color = PrimaryColors.DarkPink,
            FontStyle = FontStyles.Bold,
            Alignment = TextAlignmentOptions.Left,
            LineSpacing = 0f
        };
        
        /// <summary>
        /// 数值文本样式
        /// </summary>
        public static TextStyle Numeric => new TextStyle
        {
            FontSize = 24,
            Color = PrimaryColors.Pink,
            FontStyle = FontStyles.Bold,
            Alignment = TextAlignmentOptions.Center,
            LineSpacing = 0f
        };
    }
    
    /// <summary>
    /// 文本样式类
    /// </summary>
    public class TextStyle
    {
        public int FontSize { get; set; }
        public Color Color { get; set; }
        public int FontStyle { get; set; }
        public TextAlignmentOptions Alignment { get; set; }
        public float LineSpacing { get; set; }
    }
    
    /// <summary>
    /// TextMeshPro对齐选项（简化版）
    /// </summary>
    public enum TextAlignmentOptions
    {
        Left = 0,
        Center = 1,
        Right = 2,
        TopLeft = 3,
        TopCenter = 4,
        TopRight = 5,
        MidLeft = 6,
        MidCenter = 7,
        MidRight = 8,
        BottomLeft = 9,
        BottomCenter = 10,
        BottomRight = 11
    }
    
    #endregion
    
    #region 阴影样式
    
    /// <summary>
    /// 应用阴影效果到Image
    /// </summary>
    public static void ApplyShadow(this Graphic graphic, PanelStyle style)
    {
        if (!style.ShadowEnabled) return;
        
        // 使用Unity的内置Shadow组件
        var shadow = graphic.GetComponent<Shadow>();
        if (shadow == null)
        {
            shadow = graphic.gameObject.AddComponent<Shadow>();
        }
        
        shadow.effectColor = style.ShadowColor;
        shadow.effectDistance = style.ShadowOffset;
        shadow.effectBlur = style.ShadowBlur;
    }
    
    #endregion
}
