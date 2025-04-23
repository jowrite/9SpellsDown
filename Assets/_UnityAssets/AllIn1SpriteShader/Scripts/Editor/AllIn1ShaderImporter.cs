#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[InitializeOnLoad]
public static class AllIn1ShaderImporter
{
	public enum UnityVersion
	{
		NONE = 0,
		UNITY_2019 = 1,
		UNITY_2020 = 2,
		UNITY_2021 = 3,
		UNITY_2022 = 4,
		UNITY_6	= 5,
	}

	public enum RenderPipeline
	{
		NONE = -1,
		BIRP = 0,
		URP = 1,
		HDRP = 2,
	}

	private const string LIT_SHADER_PIPELINE_KEY = "AllIn1SpriteShader_LitShader_RenderPipeline";
	private const string LIT_SHADER_UNITY_VERSION_KEY = "AllIn1SpriteShader_LitShader_UnityVersion";
	private const string LIT_SHADER_FIRST_TIME_PROJECT = "AllIn1SpriteShader_LitShader_FirstTimeProject";

	private const string LIT_SHADER_PATH		= "../../Shaders/AllIn1SpriteShaderLit.shader";

	private const string SHADER_PATH_STANDARD	= "../../Shaders/LitShaders/AllIn1SpriteShaderLit_BetterShader_Standard.txt";

	private const string SHADER_PATH_URP_2019	= "../../Shaders/LitShaders/AllIn1SpriteShaderLit_BetterShader_URP2019.txt";
	private const string SHADER_PATH_HDRP_2019	= "../../Shaders/LitShaders/AllIn1SpriteShaderLit_HDRP2019.txt";

	private const string SHADER_PATH_URP_2020	= "../../Shaders/LitShaders/AllIn1SpriteShaderLit_BetterShader_URP2020.txt";
	private const string SHADER_PATH_HDRP_2020	= "../../Shaders/LitShaders/AllIn1SpriteShaderLit_BetterShader_HDRP2020.txt";

	private const string SHADER_PATH_URP_2021	= "../../Shaders/LitShaders/AllIn1SpriteShaderLit_BetterShader_URP2021.txt";
	private const string SHADER_PATH_HDRP_2021	= "../../Shaders/LitShaders/AllIn1SpriteShaderLit_BetterShader_HDRP2021.txt";

	private const string SHADER_PATH_URP_2022	= "../../Shaders/LitShaders/AllIn1SpriteShaderLit_BetterShader_URP2022.txt";
	private const string SHADER_PATH_HDRP_2022	= "../../Shaders/LitShaders/AllIn1SpriteShaderLit_BetterShader_HDRP2022.txt";

	private const string SHADER_PATH_URP_2023	= "../../Shaders/LitShaders/AllIn1SpriteShaderLit_BetterShader_URP2023.txt";
	private const string SHADER_PATH_HDRP_2023	= "../../Shaders/LitShaders/AllIn1SpriteShaderLit_BetterShader_HDRP2023.txt";

	static AllIn1ShaderImporter()
	{
		EditorApplication.quitting += Quit;

		ConfigureShader();
	}

	private static void Quit()
	{
		EditorPrefs.DeleteKey(LIT_SHADER_FIRST_TIME_PROJECT);
	}

	private static void ConfigureShader()
	{
		RenderPipelineChecker.RefreshData();

		string shaderPath = string.Empty;
		
		UnityVersion unityVersion = GetUnityVersion();
		RenderPipeline renderPipeline = GetRenderPipeline();
		
		if (renderPipeline == RenderPipeline.HDRP)
		{
			switch (unityVersion)
			{
				case UnityVersion.UNITY_2019:
					shaderPath = SHADER_PATH_HDRP_2019;
					break;
				case UnityVersion.UNITY_2020:
					shaderPath = SHADER_PATH_HDRP_2020;
					break;
				case UnityVersion.UNITY_2021:
					shaderPath = SHADER_PATH_HDRP_2021;
					break;
				case UnityVersion.UNITY_2022:
					shaderPath = SHADER_PATH_HDRP_2022;
					break;
				case UnityVersion.UNITY_6:
					shaderPath = SHADER_PATH_HDRP_2023;
					break;

			}
		}
		else if (renderPipeline == RenderPipeline.URP)
		{
			switch (unityVersion)
			{
				case UnityVersion.UNITY_2019:
					shaderPath = SHADER_PATH_URP_2019;
					break;
				case UnityVersion.UNITY_2020:
					shaderPath = SHADER_PATH_URP_2020;
					break;
				case UnityVersion.UNITY_2021:
					shaderPath = SHADER_PATH_URP_2021;
					break;
				case UnityVersion.UNITY_2022:
					shaderPath = SHADER_PATH_URP_2022;
					break;
				case UnityVersion.UNITY_6:
					shaderPath = SHADER_PATH_URP_2023;
					break;
			}
		}
		else
		{
			shaderPath = SHADER_PATH_STANDARD;
		}

		RenderPipeline lastRenderPipeline = (RenderPipeline)EditorPrefs.GetInt(LIT_SHADER_PIPELINE_KEY, -1);
		UnityVersion lastUnityVersion = (UnityVersion)EditorPrefs.GetInt(LIT_SHADER_UNITY_VERSION_KEY, 0);
		int firstTimeProject = EditorPrefs.GetInt(LIT_SHADER_FIRST_TIME_PROJECT, -1);

		if (lastRenderPipeline != renderPipeline || lastUnityVersion != unityVersion || firstTimeProject != 1)
		{
			EditorPrefs.SetInt(LIT_SHADER_PIPELINE_KEY, (int)renderPipeline);
			EditorPrefs.SetInt(LIT_SHADER_UNITY_VERSION_KEY, (int)unityVersion);
			EditorPrefs.SetInt(LIT_SHADER_FIRST_TIME_PROJECT, 1);

			try
			{
				var currentFileGUID = AssetDatabase.FindAssets($"t:Script {nameof(AllIn1ShaderImporter)}")[0];
				string currentFolder = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(currentFileGUID));

				string newShaderStr = File.ReadAllText(Path.Combine(currentFolder, shaderPath));
				newShaderStr = newShaderStr.Replace("Shader \"AllIn1SpriteShader/AllIn1SpriteShaderLit_BetterShader\"", "Shader \"AllIn1SpriteShader/AllIn1SpriteShaderLit\"");

				File.WriteAllText(Path.Combine(currentFolder, LIT_SHADER_PATH), newShaderStr);

				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
			catch (Exception e)
			{
				Debug.LogError("Shader not found: " + e);
			}
		}
	}

	private static UnityVersion GetUnityVersion()
	{
		UnityVersion res = UnityVersion.NONE;

		string unityVersion = Application.unityVersion;

		if (unityVersion.Contains("2019"))
		{
			res = UnityVersion.UNITY_2019;
		}
		else if (unityVersion.Contains("2020"))
		{
			res = UnityVersion.UNITY_2020;
		}
		else if (unityVersion.Contains("2021"))
		{
			res = UnityVersion.UNITY_2021;
		}
		else if (unityVersion.Contains("2022"))
		{
			res = UnityVersion.UNITY_2022;
		}
		else if (unityVersion.Contains("6000"))
		{
			res = UnityVersion.UNITY_6;
		}

		return res;
	}

	private static RenderPipeline GetRenderPipeline()
	{
		RenderPipeline res = RenderPipeline.BIRP;

		if (RenderPipelineChecker.IsURP)
		{
			res = RenderPipeline.URP;
		}
		else if (RenderPipelineChecker.IsHDRP)
		{
			res = RenderPipeline.HDRP;
		}

		return res;
	}

	public static void ForceReimport()
	{
		EditorPrefs.DeleteKey(LIT_SHADER_PIPELINE_KEY);
		EditorPrefs.DeleteKey(LIT_SHADER_UNITY_VERSION_KEY);
		EditorPrefs.DeleteKey(LIT_SHADER_FIRST_TIME_PROJECT);

		ConfigureShader();
	}
}
#endif