#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using ShaderType = AllIn1SpriteShader.AllIn1Shader.ShaderTypes;

namespace AllIn1SpriteShader
{
    public class AllIn1ShaderWindow : EditorWindow
    {
        private const string versionString = "4.2";
        [MenuItem("Tools/AllIn1/SpriteShaderWindow")]
        public static void ShowAllIn1ShaderWindowWindow()
        {
            GetWindow<AllIn1ShaderWindow>("All In 1 Shader Window");
        }
        
        public static readonly string CUSTOM_EDITOR_HEADER = "AllIn1SpriteShaderEditorImage";
        private static string basePath = "Assets/Plugins/AllIn1SpriteShader";
        public static readonly string materialsSavesRelativePath = "/Materials";
        public static readonly string renderImagesSavesRelativePath = "/Textures";
        public static readonly string normalMapSavesRelativePath = "/Textures/NormalMaps";
        public static readonly string gradientSavesRelativePath = "/Textures/GradientTextures";

        public Vector2 scrollPosition = Vector2.zero;
        private Texture2D imageInspector;
        private DefaultAsset materialTargetFolder = null;
        private GUIStyle style, bigLabel = new GUIStyle(), titleStyle = new GUIStyle();
        private const int bigFontSize = 16;
        
        AllIn1Shader.ShaderTypes shaderTypes = AllIn1Shader.ShaderTypes.Default;
        bool showUrpWarning = false;
        double warningTime = 0f;

        private Texture2D targetNormalImage;
        private float normalStrength = 5f;
        private int normalSmoothing = 1;
        private int isComputingNormals = 0;

        private enum TextureSizes
        {
            _2 = 2,
            _4 = 4,
            _8 = 8,
            _16 = 16,
            _32 = 32,
            _64 = 64,
            _128 = 128,
            _256 = 256,
            _512 = 512,
            _1024 = 1024,
            _2048 = 2048
        }
        private TextureSizes textureSizes = TextureSizes._128;
        [SerializeField] private Gradient gradient = new Gradient();
        private FilterMode gradientFiltering = FilterMode.Bilinear;
    
        private enum ImageType
        {
            ShowImage,
            HideInComponent,
            HideEverywhere
        }
        private ImageType imageType;

        private void OnGUI()
        {
            style = new GUIStyle(EditorStyles.helpBox);
            style.margin = new RectOffset(0, 0, 0, 0);
            bigLabel = new GUIStyle(EditorStyles.boldLabel);
            bigLabel.fontSize = bigFontSize;
            titleStyle.alignment = TextAnchor.MiddleLeft;

            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height)))
            {
                scrollPosition = scrollView.scrollPosition;

                ShowImageAndSetImageEditorPref();

                ShowAssetImageOptionsToggle();

                DefaultAssetShader();
            
                DrawLine(Color.grey, 1, 3);
                GUILayout.Label("Material Save Path", bigLabel);
                GUILayout.Space(20);
                GUILayout.Label("Select the folder where new Materials will be saved when the Save Material To Folder button of the asset component is pressed", EditorStyles.boldLabel);
                HandleSaveFolderEditorPref("All1ShaderMaterials", basePath + materialsSavesRelativePath, "Material");

                DrawLine(Color.grey, 1, 3);
                GUILayout.Label("Render Material to Image Save Path", bigLabel);
                GUILayout.Space(20);
                EditorGUILayout.BeginHorizontal();
                {
                    float scaleSlider = 1;
                    if (PlayerPrefs.HasKey("All1ShaderRenderImagesScale")) scaleSlider = PlayerPrefs.GetFloat("All1ShaderRenderImagesScale");
                    GUILayout.Label("Rendered Image Texture Scale", GUILayout.MaxWidth(190));
                    scaleSlider = EditorGUILayout.Slider(scaleSlider, 0.2f, 5f, GUILayout.MaxWidth(200));
                    if (GUILayout.Button("Default Value", GUILayout.MaxWidth(100))) PlayerPrefs.SetFloat("All1ShaderRenderImagesScale", 1f);
                    else PlayerPrefs.SetFloat("All1ShaderRenderImagesScale", scaleSlider);
                }
                EditorGUILayout.EndVertical();
                GUILayout.Label("Select the folder where new Images will be saved when the Render Material To Image button of the asset component is pressed", EditorStyles.boldLabel);
                HandleSaveFolderEditorPref("All1ShaderRenderImages", basePath + renderImagesSavesRelativePath, "Images");

                DrawLine(Color.grey, 1, 3);
                NormalMapCreator();
            
                DrawLine(Color.grey, 1, 3);
                GradientCreator();
                
                DrawLine(Color.grey, 1, 3);
                GUILayout.Space(10);
                SceneNotificationsToggle();

				DrawLine(Color.grey, 1, 3);
				GUILayout.Space(10);
				RefreshLitShader();

				GUILayout.Space(10);
                DrawLine(Color.grey, 1, 3);
                GUILayout.Label("Current asset version is " + versionString, EditorStyles.boldLabel);
            }
        }

        private void ShowImageAndSetImageEditorPref()
        {
            if(!EditorPrefs.HasKey("allIn1ImageConfig"))
            {
                EditorPrefs.SetInt("allIn1ImageConfig", (int) ImageType.ShowImage);
            }

            imageType = (ImageType) EditorPrefs.GetInt("allIn1ImageConfig");
            if(imageType == ImageType.HideEverywhere) return;
            switch(imageType)
            {
                case ImageType.ShowImage:
                case ImageType.HideInComponent:
                    if(imageInspector == null) imageInspector = GetInspectorImage();
                    break;
            }

            if(imageInspector)
            {
                Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(50));
                GUI.DrawTexture(rect, imageInspector, ScaleMode.ScaleToFit, true);
            }
            DrawLine(Color.grey, 1, 3);
        }

        public static Texture2D GetInspectorImage() => GetImage(CUSTOM_EDITOR_HEADER);

        private static Texture2D GetImage(string textureName)
        {
            string[] guids = AssetDatabase.FindAssets($"{textureName} t:texture");
            if(guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }
            return null;
        }

        private void ShowAssetImageOptionsToggle()
        {
            GUILayout.Label("Asset Image Display Options", bigLabel);
            GUILayout.Space(20);

            int previousImageType = (int) imageType;
            imageType = (ImageType) EditorGUILayout.EnumPopup(imageType, GUILayout.MaxWidth(200));
            if((int) imageType != previousImageType) EditorPrefs.SetInt("allIn1ImageConfig", (int) imageType);

            DrawLine(Color.grey, 1, 3);
        }

        private void DefaultAssetShader()
        {
            GUILayout.Label("Default Asset Shader", bigLabel);
            GUILayout.Space(20);
            GUILayout.Label("This is the shader variant that will be assigned by default to Sprites and UI Images when the asset component is added", EditorStyles.boldLabel);

            bool isUrp = false;
            Shader temp = FindShader("AllIn1Urp2dRenderer");
            if (temp != null) isUrp = true;

            shaderTypes = (AllIn1Shader.ShaderTypes)PlayerPrefs.GetInt("allIn1DefaultShader");
            int previousShaderType = (int)shaderTypes;
            shaderTypes = (AllIn1Shader.ShaderTypes)EditorGUILayout.EnumPopup(shaderTypes, GUILayout.MaxWidth(200));

            if (previousShaderType != (int)shaderTypes)
            {
                if (!isUrp && shaderTypes == AllIn1Shader.ShaderTypes.Urp2dRenderer)
                {
                    showUrpWarning = true;
                    warningTime = EditorApplication.timeSinceStartup + 5;
                }
                else
                {
                    PlayerPrefs.SetInt("allIn1DefaultShader", (int)shaderTypes);
                    showUrpWarning = false;
                }
            }

            if (warningTime < EditorApplication.timeSinceStartup) showUrpWarning = false;
            if (isUrp) showUrpWarning = false;
            if (!isUrp && !showUrpWarning && shaderTypes == AllIn1Shader.ShaderTypes.Urp2dRenderer)
            {
                showUrpWarning = true;
                warningTime = EditorApplication.timeSinceStartup + 5;
                shaderTypes = AllIn1Shader.ShaderTypes.Default;
                PlayerPrefs.SetInt("allIn1DefaultShader", (int)shaderTypes);
            }

            if (showUrpWarning) EditorGUILayout.HelpBox(
                "You can't set the URP 2D Renderer variant since you didn't import the URP package available in the asset root folder (SEE DOCUMENTATION)",
                MessageType.Error,
                true);
        }

        private void NormalMapCreator()
        {
            GUILayout.Label("Normal Map Creator", bigLabel);

            GUILayout.Space(20);
            GUILayout.Label("Select the folder where new Normal Maps will be saved when the Create Normal Map button of the asset component is pressed (URP only)", EditorStyles.boldLabel);
            HandleSaveFolderEditorPref("All1ShaderNormals", basePath + normalMapSavesRelativePath, "Normal Maps");

            GUILayout.Space(20);
            GUILayout.Label("Assign a sprite you want to create a normal map from. Choose the normal map settings and press the 'Create And Save Normal Map' button", EditorStyles.boldLabel);
            targetNormalImage = (Texture2D)EditorGUILayout.ObjectField("Target Image", targetNormalImage, typeof(Texture2D), false, GUILayout.MaxWidth(225));

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Normal Strength:", GUILayout.MaxWidth(150));
                normalStrength = EditorGUILayout.Slider(normalStrength, 1f, 20f, GUILayout.MaxWidth(400));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Normal Smoothing:", GUILayout.MaxWidth(150));
                normalSmoothing = EditorGUILayout.IntSlider(normalSmoothing, 0, 3, GUILayout.MaxWidth(400));
            }
            EditorGUILayout.EndHorizontal();

            if (isComputingNormals == 0)
            {
                if (targetNormalImage != null)
                {
                    if (GUILayout.Button("Create And Save Normal Map"))
                    {
                        isComputingNormals = 1;
                        return;
                    }
                }
                else
                {
                    GUILayout.Label("Add a Target Image to use this feature", EditorStyles.boldLabel);
                }
            }
            else
            {
                GUILayout.Label("Normal Map is currently being created, be patient", EditorStyles.boldLabel, GUILayout.Height(40));
                Repaint();
                isComputingNormals++;
                if (isComputingNormals > 5)
                {
                    string assetPath = AssetDatabase.GetAssetPath(targetNormalImage);
                    var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                    if (tImporter != null)
                    {
                        tImporter.isReadable = true;
                        tImporter.SaveAndReimport();
                    }

                    Texture2D normalToSave = CreateNormalMap(targetNormalImage, normalStrength, normalSmoothing);
                    
                    string prefSavedPath = PlayerPrefs.GetString("All1ShaderNormals") + "/";
                    string path = prefSavedPath + "NormalMap.png";
                    if(System.IO.File.Exists(path)) path = GetNewValidPath(path);
                    string texName = path.Replace(prefSavedPath, "");
                    
                    path = EditorUtility.SaveFilePanel("Save texture as PNG", prefSavedPath, texName, "png");
                    //If you are reading this you might have encountered an error in Unity 2022 Mac builds, if that's the case comment the line above and uncomment the line below
                    //path = prefSavedPath + texName + ".png";
                    
                    if (path.Length != 0)
                    {
                        byte[] pngData = normalToSave.EncodeToPNG();
                        if (pngData != null) File.WriteAllBytes(path, pngData);
                        AssetDatabase.Refresh();

                        if (path.IndexOf("Assets/") >= 0)
                        {
                            string subPath = path.Substring(path.IndexOf("Assets/"));
                            TextureImporter importer = AssetImporter.GetAtPath(subPath) as TextureImporter;
                            if (importer != null)
                            {
                                Debug.Log("Normal Map saved inside the project: " + subPath);
                                importer.filterMode = FilterMode.Bilinear;
                                importer.textureType = TextureImporterType.NormalMap;
                                importer.wrapMode = TextureWrapMode.Repeat;
                                importer.SaveAndReimport();
                                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(subPath, typeof(Texture)));
                            }
                        }
                        else Debug.Log("Normal Map saved outside the project: " + path);
                    }
                    isComputingNormals = 0;
                }
            }
            GUILayout.Label("*This process will freeze the editor for some seconds, larger images will take longer", EditorStyles.boldLabel);
        }

        private void HandleSaveFolderEditorPref(string keyName, string defaultPath, string logsFeatureName)
        {
            if (!PlayerPrefs.HasKey(keyName)) PlayerPrefs.SetString(keyName, defaultPath);
            materialTargetFolder = (DefaultAsset)AssetDatabase.LoadAssetAtPath(PlayerPrefs.GetString(keyName), typeof(DefaultAsset));
            if (materialTargetFolder == null)
            {
                PlayerPrefs.SetString(keyName, defaultPath);
                materialTargetFolder = (DefaultAsset)AssetDatabase.LoadAssetAtPath(PlayerPrefs.GetString(keyName), typeof(DefaultAsset));
                if (materialTargetFolder == null)
                {
                    materialTargetFolder = (DefaultAsset)AssetDatabase.LoadAssetAtPath("Assets/", typeof(DefaultAsset));
                    if(materialTargetFolder == null)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(600));
                        EditorGUILayout.HelpBox("Folder is invalid, please select a valid one", MessageType.Error, true);
                        EditorGUILayout.EndHorizontal();
                    }
                    else PlayerPrefs.SetString("Assets/", defaultPath);
                }
            }
            materialTargetFolder = (DefaultAsset)EditorGUILayout.ObjectField("New " + logsFeatureName + " Folder", 
                materialTargetFolder, typeof(DefaultAsset), false, GUILayout.MaxWidth(500));

            if (materialTargetFolder != null && IsAssetAFolder(materialTargetFolder))
            {
                string path = AssetDatabase.GetAssetPath(materialTargetFolder);
                PlayerPrefs.SetString(keyName, path);
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(600));
                EditorGUILayout.HelpBox("Valid folder! " + logsFeatureName + " save path: " + path, MessageType.Info);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(600));
                EditorGUILayout.HelpBox("Select the new " + logsFeatureName + " Folder", MessageType.Warning, true);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void GradientCreator()
        {
            GUILayout.Label("Gradient Creator", bigLabel);
            GUILayout.Space(20);
            GUILayout.Label("This feature can be used to create textures for the Color Ramp Effect", EditorStyles.boldLabel);

            EditorGUILayout.GradientField("Gradient", gradient, GUILayout.Height(25), GUILayout.MaxWidth(500));

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Texture Size:", GUILayout.MaxWidth(145));
                textureSizes = (TextureSizes)EditorGUILayout.EnumPopup(textureSizes, GUILayout.MaxWidth(200));
            }
            EditorGUILayout.EndHorizontal();

            int textureSize = (int)textureSizes;
            Texture2D gradTex = new Texture2D(textureSize, 1, TextureFormat.RGBA32, false);
            for (int i = 0; i < textureSize; i++) gradTex.SetPixel(i, 0, gradient.Evaluate((float)i / (float)textureSize));
            gradTex.Apply();

            GUILayout.Space(20);
            GUILayout.Label("Select the folder where new Gradient Textures will be saved", EditorStyles.boldLabel);
            HandleSaveFolderEditorPref("All1ShaderGradients", basePath + gradientSavesRelativePath, "Gradient");

            string prefSavedPath = PlayerPrefs.GetString("All1ShaderGradients") + "/";
            if (Directory.Exists(prefSavedPath))
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Gradient Texture Filtering: ", GUILayout.MaxWidth(170));
                    gradientFiltering = (FilterMode)EditorGUILayout.EnumPopup(gradientFiltering, GUILayout.MaxWidth(200));
                }
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Save Gradient Texture", GUILayout.MaxWidth(500)))
                {
                    string path = prefSavedPath + "ColorGradient.png";
                    if(System.IO.File.Exists(path)) path = GetNewValidPath(path);
                    string texName = path.Replace(prefSavedPath, "");

                    path = EditorUtility.SaveFilePanel("Save texture as PNG", prefSavedPath, texName, "png");
                    if (path.Length != 0)
                    {
                        byte[] pngData = gradTex.EncodeToPNG();
                        if (pngData != null) File.WriteAllBytes(path, pngData);
                        AssetDatabase.Refresh();

                        if (path.IndexOf("Assets/") >= 0)
                        {
                            string subPath = path.Substring(path.IndexOf("Assets/"));
                            TextureImporter importer = AssetImporter.GetAtPath(subPath) as TextureImporter;
                            if (importer != null)
                            {
                                Debug.Log("Gradient saved inside the project: " + subPath);
                                importer.filterMode = gradientFiltering;
                                importer.SaveAndReimport();
                                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(subPath, typeof(Texture)));
                            }
                        }
                        else Debug.Log("Gradient saved outside the project: " + path);
                    }
                }
            }
        }

        private static bool IsAssetAFolder(Object obj)
        {
            string path = "";

            if (obj == null) return false;

            path = AssetDatabase.GetAssetPath(obj.GetInstanceID());

            if (path.Length > 0)
            {
                if (Directory.Exists(path)) return true;
                else return false;
            }
            return false;
        }
        
        private static string GetNewValidPath(string path, string extension = ".png", int i = 1)
        {
            int number = i;
            path = path.Replace(extension, "");
            string newPath = path + "_" + number.ToString();
            string fullPath = newPath + extension;
            if(File.Exists(fullPath))
            {
                number++;
                fullPath = GetNewValidPath(path, extension, number);
            }

            return fullPath;
        }

        private void DrawLine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += (padding / 2);
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        public static Texture2D CreateNormalMap(Texture2D t, float normalMult = 5f, int normalSmooth = 0)
        {
            int width = t.width;
            int height = t.height;
            Color[] sourcePixels = t.GetPixels();
            Color[] resultPixels = new Color[width * height];
            Vector3 vScale = new Vector3(0.3333f, 0.3333f, 0.3333f);

            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    int index = x + y * width;
                    Vector3 cSampleNegXNegY = GetPixelClamped(sourcePixels, x - 1, y - 1, width, height);
                    Vector3 cSampleZerXNegY = GetPixelClamped(sourcePixels, x, y - 1, width, height);
                    Vector3 cSamplePosXNegY = GetPixelClamped(sourcePixels, x + 1, y - 1, width, height);
                    Vector3 cSampleNegXZerY = GetPixelClamped(sourcePixels, x - 1, y, width, height);
                    Vector3 cSamplePosXZerY = GetPixelClamped(sourcePixels, x + 1, y, width, height);
                    Vector3 cSampleNegXPosY = GetPixelClamped(sourcePixels, x - 1, y + 1, width, height);
                    Vector3 cSampleZerXPosY = GetPixelClamped(sourcePixels, x, y + 1, width, height);
                    Vector3 cSamplePosXPosY = GetPixelClamped(sourcePixels, x + 1, y + 1, width, height);

                    float fSampleNegXNegY = Vector3.Dot(cSampleNegXNegY, vScale);
                    float fSampleZerXNegY = Vector3.Dot(cSampleZerXNegY, vScale);
                    float fSamplePosXNegY = Vector3.Dot(cSamplePosXNegY, vScale);
                    float fSampleNegXZerY = Vector3.Dot(cSampleNegXZerY, vScale);
                    float fSamplePosXZerY = Vector3.Dot(cSamplePosXZerY, vScale);
                    float fSampleNegXPosY = Vector3.Dot(cSampleNegXPosY, vScale);
                    float fSampleZerXPosY = Vector3.Dot(cSampleZerXPosY, vScale);
                    float fSamplePosXPosY = Vector3.Dot(cSamplePosXPosY, vScale);

                    float edgeX = (fSampleNegXNegY - fSamplePosXNegY) * 0.25f + (fSampleNegXZerY - fSamplePosXZerY) * 0.5f + (fSampleNegXPosY - fSamplePosXPosY) * 0.25f;
                    float edgeY = (fSampleNegXNegY - fSampleNegXPosY) * 0.25f + (fSampleZerXNegY - fSampleZerXPosY) * 0.5f + (fSamplePosXNegY - fSamplePosXPosY) * 0.25f;

                    Vector2 vEdge = new Vector2(edgeX, edgeY) * normalMult;
                    Vector3 norm = new Vector3(vEdge.x, vEdge.y, 1.0f).normalized;
                    resultPixels[index] = new Color(norm.x * 0.5f + 0.5f, norm.y * 0.5f + 0.5f, norm.z * 0.5f + 0.5f, 1);
                }
            }

            if(normalSmooth > 0)
            {
                resultPixels = SmoothNormals(resultPixels, width, height, normalSmooth);
            }

            Texture2D texNormal = new Texture2D(width, height, TextureFormat.RGB24, false, false);
            texNormal.SetPixels(resultPixels);
            texNormal.Apply();
            return texNormal;
        }

        private static Vector3 GetPixelClamped(Color[] pixels, int x, int y, int width, int height)
        {
            x = Mathf.Clamp(x, 0, width - 1);
            y = Mathf.Clamp(y, 0, height - 1);
            Color c = pixels[x + y * width];
            return new Vector3(c.r, c.g, c.b);
        }

        private static Color[] SmoothNormals(Color[] pixels, int width, int height, int normalSmooth)
        {
            Color[] smoothedPixels = new Color[pixels.Length];
            float step = 0.00390625f * normalSmooth;

            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    float pixelsToAverage = 0.0f;
                    Color c = pixels[x + y * width];
                    pixelsToAverage++;

                    for(int offsetY = -normalSmooth; offsetY <= normalSmooth; offsetY++)
                    {
                        for(int offsetX = -normalSmooth; offsetX <= normalSmooth; offsetX++)
                        {
                            if(offsetX == 0 && offsetY == 0) continue;

                            int sampleX = Mathf.Clamp(x + offsetX, 0, width - 1);
                            int sampleY = Mathf.Clamp(y + offsetY, 0, height - 1);

                            c += pixels[sampleX + sampleY * width];
                            pixelsToAverage++;
                        }
                    }

                    smoothedPixels[x + y * width] = c / pixelsToAverage;
                }
            }

            return smoothedPixels;
        }
        
        [MenuItem("Assets/Create/AllIn1Shader Materials/CreateDefaultMaterial")]
        public static void CreateDefaultMaterial()
        {
            CreateMaterial("AllIn1SpriteShader");
        }
        
        [MenuItem("Assets/Create/AllIn1Shader Materials/CreateScaledTimeMaterial")]
        public static void CreateScaledTimeMaterial()
        {
            CreateMaterial("AllIn1SpriteShaderScaledTime");
        }
        
        [MenuItem("Assets/Create/AllIn1Shader Materials/CreateUiMaskMaterial")]
        public static void CreateUiMaskMaterial()
        {
            CreateMaterial("AllIn1SpriteShaderUiMask");
        }

        private static void CreateMaterial(string shaderName)
        {
            string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            if(!string.IsNullOrEmpty(selectedPath) && Directory.Exists(selectedPath))
            {
                Material material = new Material(FindShader(shaderName));
                string fullPath = selectedPath + "/Mat-" + shaderName + ".mat";
                if(File.Exists(fullPath)) fullPath = GetNewValidPath(fullPath, ".mat");
                AssetDatabase.CreateAsset(material, fullPath);
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogWarning("Please select a valid folder in the Project Window.");
            }
        }
        
        private void OnEnable() => GetBasePath();

        private static void GetBasePath()
        {
            string[] guids = AssetDatabase.FindAssets("t:folder AllIn1SpriteShader");
            if(guids.Length > 0)
            {
                basePath = AssetDatabase.GUIDToAssetPath(guids[0]);
            }
            else
            {
                Debug.LogError("AllIn1SpriteShader folder not found in the project.");
                basePath = "Assets/Plugins/AllIn1SpriteShader";
            }
        }
        
        public static string GetMaterialSavePath()
        {
            if(!PlayerPrefs.HasKey("All1ShaderMaterials"))
            {
                GetBasePath();
                return basePath + materialsSavesRelativePath;
            }
            return PlayerPrefs.GetString("All1ShaderMaterials");
        }
        
        public static string GetRenderImageSavePath()
        {
            if(!PlayerPrefs.HasKey("All1ShaderRenderImages"))
            {
                GetBasePath();
                return basePath + renderImagesSavesRelativePath;
            }
            return PlayerPrefs.GetString("All1ShaderRenderImages");
        }
        
        public static string GetNormalMapSavePath()
        {
            if(!PlayerPrefs.HasKey("All1ShaderNormals"))
            {
                GetBasePath();
                return basePath + normalMapSavesRelativePath;
            }
            return PlayerPrefs.GetString("All1ShaderNormals");
        }

        private void SceneNotificationsToggle()
        {
            float previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 200f;
            bool areNotificationsEnabled = EditorPrefs.GetInt("DisplaySceneViewNotifications", 1) == 1;
            areNotificationsEnabled = EditorGUILayout.Toggle("Display Scene View Notifications", areNotificationsEnabled);
            EditorPrefs.SetInt("DisplaySceneViewNotifications", areNotificationsEnabled ? 1 : 0);
            EditorGUIUtility.labelWidth = previousLabelWidth;
        }

		private static void RefreshLitShader()
		{
            GUILayout.Label("Force the Lit Shader to be reconfigured");
            GUILayout.Label("If you are getting some error or have changed the render pipeline press the button below");
			if (GUILayout.Button("Refresh Lit Shader", GUILayout.MaxWidth(500f)))
			{
				AllIn1ShaderImporter.ForceReimport();
			}
		}

		public static void SceneViewNotificationAndLog(string message)
        {
            Debug.Log(message);
            ShowSceneViewNotification(message);
        }

        public static void ShowSceneViewNotification(string message)
        {
            bool showNotification = EditorPrefs.GetInt("DisplaySceneViewNotifications", 1) == 1;
            if(!showNotification) return;
            
            GUIContent content = new GUIContent(message);
            #if UNITY_2019_1_OR_NEWER
            SceneView.lastActiveSceneView.ShowNotification(content, 1.5f);
            #else
            SceneView.lastActiveSceneView.ShowNotification(content);
            #endif
        }
        
        public static Shader FindShader(string shaderName)
        {
            string[] guids = AssetDatabase.FindAssets($"{shaderName} t:shader");
            foreach(string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
                if(shader != null)
                {
                    string fullShaderName = shader.name;
                    string actualShaderName = fullShaderName.Substring(fullShaderName.LastIndexOf('/') + 1);
                    if(actualShaderName.Equals(shaderName)) return shader;
                }
            }
            return null;
        }
    }
}
#endif