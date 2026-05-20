# 《糟糕！是心动鸭！》UI设计规范

## 一、设计风格总览

### 设计理念
- **风格**：星露谷物语同款像素风 + 玻璃拟态（Glassmorphism）
- **平台**：PC端（1920×1080）+ 移动端（iOS/Android）
- **色彩**：粉色、金色、天蓝色为主，温暖色调
- **字体**：Press Start 2P（像素字）+ Microsoft YaHei（中文）

### 核心设计原则
1. **像素优先**：所有元素都有像素化质感
2. **玻璃拟态**：UI面板采用半透明毛玻璃效果
3. **侧边栏布局**：B端（业务）风格，左侧导航栏
4. **响应式设计**：PC端和移动端完美适配
5. **动画流畅**：60fps像素动画

---

## 二、全局UI系统

### 2.1 色彩系统

```css
:root {
  /* 主色调 */
  --primary-pink: #ff6b9d;
  --dark-pink: #c44569;
  --gold: #ffd700;
  --sky-blue: #87ceeb;
  --cream: #fffdd0;
  
  /* 像素色彩 */
  --pixel-black: #1a1a1a;
  --pixel-gray: #3d4a54;
  --pixel-light: #f5f5f5;
  
  /* 玻璃拟态 */
  --glass-bg: rgba(255, 255, 255, 0.15);
  --glass-border: rgba(255, 255, 255, 0.2);
  --glass-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
}
```

### 2.2 字体系统
```css
/* 像素字体 */
.pixel-font {
  font-family: 'Press Start 2P', cursive;
}

/* 中文像素字体替代 */
.chinese-pixel {
  font-family: 'Microsoft YaHei', sans-serif;
  letter-spacing: 2px;
  image-rendering: pixelated;
}

/* 字号系统 */
.font-title { font-size: 24px; }    /* 标题 */
.font-subtitle { font-size: 16px; } /* 副标题 */
.font-body { font-size: 12px; }     /* 正文 */
.font-small { font-size: 10px; }    /* 小字 */
```

### 2.3 像素边框系统
```css
/* 像素边框1x */
.pixel-border-1 {
  border: 2px solid var(--pixel-black);
  box-shadow: 
    2px 0 0 var(--pixel-black),
    0 2px 0 var(--pixel-black),
    -2px 0 0 var(--pixel-black),
    0 -2px 0 var(--pixel-black);
}

/* 像素边框2x */
.pixel-border-2 {
  border: 4px solid var(--pixel-black);
  box-shadow: 
    4px 0 0 var(--pixel-black),
    0 4px 0 var(--pixel-black),
    -4px 0 0 var(--pixel-black),
    0 -4px 0 var(--pixel-black);
}
```

---

## 三、PC端UI布局 - 侧边栏式（B端风格）

### 3.1 主界面布局

```
┌─────────────────────────────────────────────────────────────┐
│  侧边栏导航  │           主内容区域                    │
│  [280px]    │                                         │
├─────────────┼─────────────────────────────────────────┤
│             │                                         │
│ ● 🏠 主页   │          游戏内容展示               │
│ ● 👥 嘉宾   │                                         │
│ ● 📋 任务   │          像素风格UI                │
│ ● 📊 数据   │                                         │
│ ● 🎁 商城   │                                         │
│ ● ⚙️ 设置   │                                         │
│             │                                         │
└─────────────┴─────────────────────────────────────────┘
```

### 3.2 侧边栏导航详细设计

**侧边栏组件**：
```
┌──────────────────────┐
│ 🦆 糟糕！是心动鸭！  │ ← Logo区域
├──────────────────────┤
│                      │
│  [🏠] 主页           │ ← 当前选中项
│  [👥] 嘉宾列表       │
│  [📋] 任务系统       │
│  [📊] 数据面板       │
│  [🎁] 商城           │
│  [⚙️] 设置           │
│                      │
│  [❤️] 心动值: 9999   │ ← 底部状态栏
│  [💰] 金币: 10000000 │
└──────────────────────┘
```

**侧边栏CSS**：
```css
.sidebar {
  width: 280px;
  background: var(--glass-bg);
  backdrop-filter: blur(20px);
  border-right: 4px solid var(--primary-pink);
  box-shadow: 4px 0 0 rgba(0, 0, 0, 0.1);
  padding: 20px 0;
}

.sidebar-item {
  padding: 16px 24px;
  margin: 8px 16px;
  background: transparent;
  border-radius: 0;
  cursor: pointer;
  transition: all 0.1s;
  font-size: 14px;
}

.sidebar-item:hover {
  background: var(--primary-pink);
  transform: translateX(4px);
}

.sidebar-item.active {
  background: var(--primary-pink);
  color: white;
  border-left: 4px solid var(--gold);
}
```

### 3.3 主内容区域（玻璃拟态面板）

**主面板设计**：
```
┌──────────────────────────────────────────────────┐
│  面板标题                           [X] [-] [□] │ ← 窗口控件
├──────────────────────────────────────────────────┤
│                                                  │
│         像素风格内容展示区域                     │
│                                                  │
│                                                  │
│                                                  │
│                                                  │
└──────────────────────────────────────────────────┘
```

**玻璃拟态CSS**：
```css
.glass-panel {
  background: var(--glass-bg);
  backdrop-filter: blur(20px);
  border: 2px solid var(--glass-border);
  box-shadow: var(--glass-shadow);
  border-radius: 0;
  padding: 24px;
}

.glass-panel::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 40px;
  background: var(--primary-pink);
  border-bottom: 2px solid var(--pixel-black);
}
```

### 3.4 嘉宾列表页设计

```
┌─────────────────────────────────────────────────────────┐
│  👥 嘉宾列表                                   [筛选]  │
├─────────────────────────────────────────────────────────┤
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐  │
│  │  头像   │  │  头像   │  │  头像   │  │  头像   │  │
│  │  ❄️    │  │  ☀️    │  │  💼    │  │  🎹    │  │
│  │  冷冷  │  │  森森  │  │  琛哥  │  │  小季  │  │
│  │ Lv.80  │  │ Lv.95  │  │ Lv.70  │  │ Lv.65  │  │
│  │ ❤️95   │  │ ❤️85   │  │ ❤️75   │  │ ❤️60   │  │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘  │
│                                                         │
│  女嘉宾 ───────────────────────────────────────────────  │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐  │
│  │  头像   │  │  头像   │  │  头像   │  │  头像   │  │
│  │  🍬    │  │  ⚖️    │  │  📸    │  │  🌟    │  │
│  │  念念  │  │  晚宁  │  │  知意  │  │ 星辰  │  │
│  │ Lv.90  │  │ Lv.75  │  │ Lv.85  │  │ Lv.80  │  │
│  │ ❤️88   │  │ ❤️65   │  │ ❤️75   │  │ ❤️82   │  │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘  │
└─────────────────────────────────────────────────────────┘
```

### 3.5 对话界面设计

```
┌─────────────────────────────────────────────────────────┐
│  🎬 第7天 - 下午 - 客厅                               │
├─────────────────────────────────────────────────────────┤
│ ┌─────────────────────────────────────────────────────┐ │
│ │  场景背景（像素风客厅）                          │ │
│ └─────────────────────────────────────────────────────┘ │
│                                                         │
│  ┌─────────────────────────────────────────────────┐  │
│  │  [头像]  冷冷                                  │  │
│  │  "今天天气不错，要不要一起去散步？"             │  │
│  └─────────────────────────────────────────────────┘  │
│                                                         │
│  ┌─────────────────────────────────────────────────┐  │
│  │  [玩家头像]  你                                │  │
│  │  "好啊，我正好想出去走走！"                    │  │
│  └─────────────────────────────────────────────────┘  │
│                                                         │
│  选择: [1] 一起去      [2] 还是算了      [3] 换话题  │
└─────────────────────────────────────────────────────────┘
```

---

## 四、移动端UI设计

### 4.1 移动端布局适配

```
┌─────────────────┐
│ 🦆 标题        │ ← 顶部标题栏
├─────────────────┤
│                 │
│   主内容区域   │
│    像素UI      │
│                 │
│                 │
│                 │
│                 │
│                 │
├─────────────────┤
│ [🏠][👥][📋]   │ ← 底部导航栏
└─────────────────┘
```

### 4.2 顶部导航栏

```css
.mobile-topbar {
  height: 60px;
  background: var(--glass-bg);
  backdrop-filter: blur(10px);
  border-bottom: 2px solid var(--primary-pink);
  padding: 0 16px;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.mobile-title {
  font-size: 16px;
  color: var(--gold);
  text-shadow: 2px 2px var(--pixel-black);
}
```

### 4.3 底部导航栏

```css
.mobile-bottombar {
  height: 64px;
  background: var(--glass-bg);
  backdrop-filter: blur(10px);
  border-top: 2px solid var(--primary-pink);
  display: flex;
  justify-content: space-around;
  align-items: center;
}

.mobile-nav-item {
  flex: 1;
  padding: 12px;
  text-align: center;
  font-size: 10px;
  cursor: pointer;
}

.mobile-nav-item.active {
  background: var(--primary-pink);
  color: white;
}
```

---

## 五、像素风UI组件库

### 5.1 像素按钮

```css
.pixel-button {
  padding: 12px 24px;
  background: var(--primary-pink);
  color: white;
  border: none;
  font-family: 'Press Start 2P', cursive;
  font-size: 12px;
  cursor: pointer;
  position: relative;
  transition: all 0.1s;
  image-rendering: pixelated;
}

.pixel-button::before {
  content: '';
  position: absolute;
  top: 4px;
  left: 4px;
  right: -4px;
  bottom: -4px;
  background: var(--pixel-black);
  z-index: -1;
}

.pixel-button:hover {
  transform: translate(-2px, -2px);
  background: var(--gold);
  color: var(--pixel-black);
}

.pixel-button:active {
  transform: translate(2px, 2px);
}
```

### 5.2 像素进度条

```css
.pixel-progress {
  height: 16px;
  background: var(--pixel-gray);
  border: 2px solid var(--pixel-black);
  position: relative;
}

.pixel-progress-bar {
  height: 100%;
  background: var(--primary-pink);
  transition: width 0.3s;
}
```

### 5.3 像素文本框

```css
.pixel-input {
  padding: 12px 16px;
  background: var(--cream);
  border: 4px solid var(--pixel-black);
  font-family: 'Press Start 2P', cursive;
  font-size: 12px;
  outline: none;
  image-rendering: pixelated;
}

.pixel-input:focus {
  border-color: var(--primary-pink);
  box-shadow: 0 0 0 4px var(--gold);
}
```

---

## 六、导演模式专用UI

### 6.1 导演控制台

```
┌─────────────────────────────────────────────────────────┐
│ 🎬 导演控制台 - 第7天  [实时]  [加速]  [暂停]      │
├─────────────────────────────────────────────────────────┤
│  ┌───────────────────────────────────────────────────┐  │
│  │  🔥 节目热度: 85/100 [████████░░░░]          │  │
│  │  👁️ 收视率: 1.23% [+0.15%]                   │  │
│  │  💬 弹幕数: 128,000 [+8.3%]                    │  │
│  │  📈 微博话题: #糟糕是心动鸭# 阅读量2.3亿       │  │
│  └───────────────────────────────────────────────────┘  │
│                                                         │
│  嘉宾关系图谱:                                           │
│  ┌───────────────────────────────────────────────────┐  │
│  │  冷冷───────念念              洲洲──────酒酒     │  │
│  │    ╲╱                          ╲╱              │  │
│  │    ╱╲                          ╱╲              │  │
│  │  小季    琛哥                森森              │  │
│  │          ╲                   ╱                │  │
│  │          晚宁   知意   星辰   幼薇              │  │
│  └───────────────────────────────────────────────────┘  │
│                                                         │
│  [安排活动] [制造话题] [危机处理] [私下谈话]            │
└─────────────────────────────────────────────────────────┘
```

### 6.2 数据面板（玻璃拟态）

```css
.stats-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 16px;
}

.stat-card {
  background: var(--glass-bg);
  backdrop-filter: blur(10px);
  border: 2px solid var(--glass-border);
  padding: 20px;
  text-align: center;
}

.stat-number {
  font-size: 24px;
  color: var(--primary-pink);
  font-weight: bold;
}

.stat-label {
  font-size: 10px;
  color: var(--pixel-gray);
}
```

---

## 七、动画效果库

### 7.1 像素动画基础

```css
/* 像素跳动 */
@keyframes pixel-bounce {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-8px); }
}

/* 爱心闪烁 */
@keyframes heart-beat {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.2); }
}

/* 像素位移 */
@keyframes pixel-shake {
  0%, 100% { transform: translateX(0); }
  25% { transform: translateX(-4px); }
  75% { transform: translateX(4px); }
}
```

### 7.2 入场动画

```css
@keyframes slide-in-left {
  from {
    opacity: 0;
    transform: translateX(-32px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

@keyframes slide-in-bottom {
  from {
    opacity: 0;
    transform: translateY(32px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
```

---

## 八、UI设计检查清单

### 8.1 PC端检查项
- [x] 侧边栏导航完整
- [x] 玻璃拟态效果
- [x] 像素风格按钮
- [x] 响应式布局
- [x] 1920×1080适配

### 8.2 移动端检查项
- [x] 底部导航栏
- [x] 顶部标题栏
- [x] 触摸友好的按钮
- [x] 适配常见屏幕尺寸

### 8.3 像素风格检查项
- [x] Press Start 2P字体
- [x] 像素边框
- [x] 像素化渲染
- [x] 星露谷物语风格
- [x] 像素动画60fps

---

## 九、UI文件结构

```
Assets/UI/
├── Components/
│   ├── PixelButton.cs
│   ├── PixelProgressBar.cs
│   └── PixelInput.cs
├── Panels/
│   ├── Sidebar.cs
│   ├── GlassPanel.cs
│   └── DialogBox.cs
├── Layouts/
│   ├── PCLayout.cs
│   └── MobileLayout.cs
└── Themes/
    ├── PinkTheme.cs
    └── GoldTheme.cs
```

---

**最后更新**：2026-05-20  
**版本**：1.0  
**设计团队**：糟糕！是心动鸭！UI组
