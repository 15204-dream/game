# 《糟糕！是心动鸭！》游戏打包指南

## 📦 如何将Unity项目打包成EXE文件

### 环境准备

1. **安装Unity编辑器**
   - 下载并安装 Unity Hub：https://unity.com/download
   - 在 Unity Hub 中安装 Unity 2022.3 LTS 或更高版本
   - 确保安装了 Windows Build Support 模块（用于打包 Windows EXE）

2. **打开项目**
   - 打开 Unity Hub
   - 点击 "Add" 按钮
   - 选择项目根目录 `/workspace/`
   - 等待项目导入完成

---

## 🛠️ 项目配置步骤

### 步骤1：检查场景设置

1. 在 Unity 编辑器中，打开 `File > Build Settings`
2. 确保以下场景已添加到 "Scenes In Build" 列表中：
   - `Assets/Scenes/MainMenu.unity`（主菜单场景，索引0）
   - `Assets/Scenes/GuestMode.unity`（嘉宾模式场景）
   - `Assets/Scenes/DirectorMode.unity`（导演模式场景）
   - `Assets/Scenes/EndingScene.unity`（结局场景）

   *如果场景不存在，请先创建或导入所需场景*

### 步骤2：选择平台

1. 在 Build Settings 窗口中，选择 "PC, Mac & Linux Standalone"
2. 选择 "Windows" 作为目标平台
3. 点击 "Switch Platform" 按钮（如果当前不是该平台）

### 步骤3：配置Player Settings

1. 在 Build Settings 窗口中，点击 "Player Settings..." 按钮
2. 在 Inspector 中配置以下选项：

#### Product Name & Company
- **Product Name**: 糟糕！是心动鸭！
- **Company Name**: YourCompany
- **Version**: 1.0.0

#### Default Icon
- 设置游戏图标（可选）

#### Resolution and Presentation
- **Default Screen Width**: 1920
- **Default Screen Height**: 1080
- **Fullscreen Mode**: Fullscreen Window
- **Aspect Ratio**: 16:9

#### Other Settings
- **Scripting Backend**: Mono
- **API Compatibility Level**: .NET Framework
- **Strip Engine Code**: Enabled（减少包体积）

#### Publishing Settings
- **Compression Method**: LZ4HC（较高压缩率）

### 步骤4：配置API密钥

确保在打包前已配置好DeepSeek API密钥：

1. 打开文件：`Assets/Scripts/Core/API/APIClient.cs`
2. 确认API密钥已正确设置：
```csharp
private const string API_KEY = "sk-fe8fdf06158c44b5b6304a8e30dce5a8";
```

---

## 📦 开始打包

### 方法一：使用Build Settings窗口

1. 打开 `File > Build Settings`
2. 确认所有配置正确
3. 选择输出文件夹（建议创建一个单独的 `Build/` 文件夹）
4. 点击 "Build" 或 "Build And Run" 按钮
5. 等待打包完成

### 方法二：使用命令行（可选）

如果你想使用命令行打包，可以使用以下命令：

```bash
# Windows PowerShell 或 CMD
"C:\Program Files\Unity\Hub\Editor\2022.3.x\Editor\Unity.exe" ^
  -quit ^
  -batchmode ^
  -nographics ^
  -projectPath "C:\path\to\your\project" ^
  -buildWindowsPlayer "C:\path\to\output\糟糕！是心动鸭！.exe"
```

---

## 📁 打包后文件结构

打包成功后，你会得到以下文件：

```
Build/
├── 糟糕！是心动鸭！.exe          # 游戏主程序
├── 糟糕！是心动鸭！_Data/       # 游戏数据文件夹
│   ├── globalgamemanagers
│   ├── level0
│   ├── resources.assets
│   ├── Managed/
│   └── ...
├── UnityCrashHandler64.exe       # 崩溃处理程序
├── UnityPlayer.dll               # Unity运行时
└── MonoBleedingEdge/            # Mono运行时
```

**重要提示**：所有文件必须放在同一目录下才能正常运行！

---

## 🚀 发布准备

### 创建发布包

为了方便用户，建议创建一个包含以下内容的压缩包：

```
糟糕！是心动鸭！_v1.0_Windows.zip
├── 糟糕！是心动鸭！.exe
├── 糟糕！是心动鸭！_Data/
├── UnityPlayer.dll
├── UnityCrashHandler64.exe
├── MonoBleedingEdge/
├── README.txt           # 游戏说明
└── LICENSE.txt          # 许可证
```

### 创建README.txt示例

```
《糟糕！是心动鸭！》v1.0

系统要求：
- Windows 10/11 (64位)
- 4GB RAM 以上
- 2GB 可用磁盘空间

操作说明：
1. 解压所有文件到同一文件夹
2. 双击运行 "糟糕！是心动鸭！.exe"
3. 享受游戏！

游戏模式：
- 嘉宾模式：作为第13位嘉宾参与恋综
- 导演模式：作为导演打造爆款节目

技术支持：如有问题请联系开发团队
```

---

## 🔧 常见问题

### Q: 打包时提示缺少场景？
A: 确保在 Build Settings 中添加了所有必需的场景。

### Q: 打包后API无法连接？
A: 检查防火墙设置，确保游戏可以访问网络。

### Q: 游戏启动时崩溃？
A: 检查是否满足系统要求，确保所有文件完整。

### Q: 如何减小打包体积？
A: 
- 启用 "Strip Engine Code"
- 使用 LZ4HC 压缩
- 移除未使用的资源
- 使用 Sprite Atlas 合并贴图

### Q: 如何为其他平台打包？
A: 在 Build Settings 中选择对应的平台（macOS、Linux等），重复上述步骤即可。

---

## ✅ 打包检查清单

打包前请确认：

- [ ] Unity版本为 2022.3 LTS 或更高
- [ ] 所有必需场景已添加到 Build Settings
- [ ] API密钥已正确配置
- [ ] Player Settings 配置完成
- [ ] 目标平台选择正确
- [ ] 输出文件夹有足够空间
- [ ] 测试过游戏在编辑器中能正常运行

---

## 📞 技术支持

如遇到打包问题，请：
1. 查看 Unity Console 中的错误信息
2. 检查 Unity 编辑器日志文件
3. 参考 Unity 官方文档：https://docs.unity3d.com/Manual/PublishingBuilds.html

---

**文档版本**: 1.0  
**最后更新**: 2026-05-20
