# 《糟糕！是心动鸭！》恋综模拟器 - Product Requirement Document

## Overview
- **Summary**: 一款像素风恋爱综艺模拟器游戏，提供嘉宾视角和导演视角双玩法，结合AI动态剧情生成，玩家可以扮演嘉宾寻找真爱或友情，或扮演导演打造爆款综艺。
- **Purpose**: 打造沉浸式恋综体验，通过像素风美术和AI剧情生成，让玩家体验恋爱综艺的浪漫与乐趣。
- **Target Users**: 恋爱游戏爱好者、综艺爱好者、像素风游戏爱好者。

## Goals
- 实现完整的双玩法系统（嘉宾视角 + 导演视角）
- 设计12位性格鲜明的嘉宾人设
- 集成DeepSeek API实现动态剧情生成
- 实现任务系统、社交系统、商城系统等核心功能
- 生成完整的像素风美术资源（人物、场景、图标等）
- 提供多结局游戏体验

## Non-Goals (Out of Scope)
- 不实现在线多人功能（纯单机游戏）
- 不实现跨平台同步功能
- 不实现内购付费功能（所有内容免费）

## Background & Context
- 恋综题材近年来非常受欢迎，具有广泛的用户基础
- 像素风美术风格复古可爱，适合这类休闲游戏
- AI大语言模型的发展使得动态剧情生成成为可能
- 使用React + TypeScript技术栈，确保代码质量和可维护性

## Functional Requirements
- **FR-1**: 实现嘉宾视角玩法，玩家可扮演嘉宾与其他11位NPC互动
- **FR-2**: 实现导演视角玩法，玩家可安排活动、管理节目热度
- **FR-3**: 集成DeepSeek API实现AI动态对话和剧情生成
- **FR-4**: 实现好感度系统和关系管理
- **FR-5**: 实现任务系统（日常任务、主线任务、成就）
- **FR-6**: 实现商城系统（服装、道具、装饰）
- **FR-7**: 实现新手引导系统
- **FR-8**: 实现数据统计面板
- **FR-9**: 实现多结局系统（嘉宾视角4种结局，导演视角4种结局）
- **FR-10**: 生成完整的像素风美术资源（16张人物立绘、6张场景、图标、道具等）

## Non-Functional Requirements
- **NFR-1**: 游戏界面美观，像素风格统一
- **NFR-2**: 游戏响应流畅，加载时间<2秒
- **NFR-3**: 游戏支持存档和读档功能
- **NFR-4**: AI对话生成响应时间<5秒
- **NFR-5**: 游戏适配主流浏览器（Chrome、Firefox、Safari）

## Constraints
- **Technical**: React 18 + TypeScript，前端项目
- **Business**: 单机游戏，无需后端服务器
- **Dependencies**: DeepSeek API用于剧情生成

## Assumptions
- DeepSeek API服务稳定可用
- 用户设备支持现代浏览器
- 像素风格参考图（用户上传的两张图）有效

## Acceptance Criteria

### AC-1: 身份选择功能
- **Given**: 用户启动游戏并进入开始界面
- **When**: 用户点击"开始游戏"
- **Then**: 显示身份选择界面，可选择"嘉宾视角"或"导演视角"
- **Verification**: `programmatic`

### AC-2: 嘉宾视角玩法
- **Given**: 用户选择嘉宾视角并完成角色创建
- **When**: 用户进入游戏
- **Then**: 可以与其他嘉宾互动、聊天、约会、发送心动短信
- **Verification**: `programmatic`

### AC-3: 导演视角玩法
- **Given**: 用户选择导演视角并完成角色创建
- **When**: 用户进入游戏
- **Then**: 可以安排活动、配对嘉宾、剪辑内容、管理热度
- **Verification**: `programmatic`

### AC-4: AI剧情生成
- **Given**: 用户与NPC对话或触发剧情
- **When**: 需要生成对话内容
- **Then**: 调用DeepSeek API生成符合人设和场景的回复
- **Verification**: `programmatic`

### AC-5: 任务系统
- **Given**: 用户在游戏中
- **When**: 查看任务面板
- **Then**: 显示日常任务、主线任务、成就，完成可获得奖励
- **Verification**: `programmatic`

### AC-6: 商城系统
- **Given**: 用户在游戏中
- **When**: 打开商城
- **Then**: 可以购买服装、道具、装饰等物品
- **Verification**: `programmatic`

### AC-7: 多结局系统
- **Given**: 用户完成21天游戏流程
- **When**: 到达最终告白日/节目收官
- **Then**: 根据玩家选择和数据结算对应的结局
- **Verification**: `programmatic`

### AC-8: 像素风美术资源
- **Given**: 游戏运行中
- **When**: 显示人物、场景、图标等
- **Then**: 所有美术资源为统一的像素风格，PNG格式
- **Verification**: `human-judgment`

### AC-9: 方舟像素体字体
- **Given**: 游戏运行中
- **When**: 显示文字内容
- **Then**: 使用方舟像素体字体
- **Verification**: `human-judgment`

## Open Questions
- [ ] 像素风资源生成是否需要使用特定的AI绘画工具？
- [ ] 是否需要音效和背景音乐？
