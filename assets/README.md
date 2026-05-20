# 资源说明

本游戏使用**程序化生成**的方式创建所有像素风资源，无需外部图片文件。

## 资源类型
- 人物立绘：128x128像素，通过CharacterSpriteGenerator生成
- 场景背景：通过BackgroundGenerator生成
- UI元素：通过UIElementGenerator生成
- 字体：内置像素字体渲染器，无需外部字体文件

## 技术实现
所有资源均使用Canvas 2D API实时绘制，确保复古像素风格。
