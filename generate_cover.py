#!/usr/bin/env python3
import asyncio
import sys
import os

# 添加技能脚本路径
sys.path.append("/data/user/skills/byted-seedream-image-generate/scripts")
from seedream_image_generate import seedream_generate

async def generate_cover():
    print("正在生成游戏封面...")
    
    # 像素风格游戏封面的提示词
    prompt = """pixel art game cover for a dating simulator, title "Oops! I'm in Love!" (糟糕！是心动鸭！), 16-bit retro pixel style, cute characters, pink hearts, vibrant colors, pixelated love hotel background, anime style pixel art, 1024x1024, highly detailed, retro game aesthetic"""
    
    try:
        # 使用5.0-lite版本，PNG格式，无水印
        result = await seedream_generate([
            {
                "prompt": prompt,
                "size": "1024x1024",
                "watermark": False,
                "output_format": "png"
            }
        ], version="5.0")
        
        print("生成成功！")
        print(f"结果: {result}")
        
        # 保存URL到文件
        if isinstance(result, list) and len(result) > 0:
            url = result[0].get('url', '')
            if url:
                with open('/workspace/cover_url.txt', 'w') as f:
                    f.write(url)
                print(f"封面URL已保存到 /workspace/cover_url.txt")
                print(f"封面URL: {url}")
                
                # 尝试下载封面
                import requests
                print("正在下载封面图片...")
                response = requests.get(url)
                if response.status_code == 200:
                    with open('/workspace/assets/images/cover.png', 'wb') as f:
                        f.write(response.content)
                    print("封面图片已保存到 /workspace/assets/images/cover.png")
        
    except Exception as e:
        print(f"生成封面时出错: {e}")
        import traceback
        traceback.print_exc()

if __name__ == "__main__":
    # 确保输出目录存在
    os.makedirs('/workspace/assets/images', exist_ok=True)
    asyncio.run(generate_cover())
