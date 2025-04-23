# BetterShaders_AllIn1Sprite Shader

This shader was created using Better Shaders, a Unity asset by Jason Booth that simplifies cross-pipeline shader development.

## Usage
- If you own Better Shaders: You can modify the shader's source code: AllIn1SpriteShaderLit_BetterShader.surfshader
- If you don't own Better Shaders: A pre-compiled version matching your current Render Pipeline and Unity version will be automatically set up.

## Why Better Shaders?
Better Shaders was used because Unity doesn't have an easy way to create hand written lit shaders compatible across all render pipelines.

## About Better Shaders
Better Shaders streamlines shader creation by:
- Enabling Unity-like shader writing
- Auto-compiling for various pipelines
- Allowing shader stacking
- Functioning like native Unity shaders

For more information or to purchase Better Shaders, visit:
[Better Shaders on Unity Asset Store](https://assetstore.unity.com/packages/tools/visual-scripting/better-shaders-2022-standard-urp-hdrp-244057)

## What are all the .txt shaders and how to export them?
The .txt shaders are the pipeline-specific source code for the shader. You can export them by:
1. Selecting AllIn1SpriteShaderLit_BetterShader.surfshader
2. Export all shaders as text assets

Doing this will cause some URP and HDRP shaders material inspectors properties to be in the incorrect order. To fix this, you can:
1. Localize the Better Shaders2022 folder
2. Inside you'll see the PipelineTemplates folder
3. There, find your pipeline's template
4. Make sure that "%PROPERTIES%" is right under "Properties{"
