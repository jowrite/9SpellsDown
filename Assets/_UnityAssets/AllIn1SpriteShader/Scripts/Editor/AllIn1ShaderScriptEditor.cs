#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ShaderType = AllIn1SpriteShader.AllIn1Shader.ShaderTypes;

namespace AllIn1SpriteShader
{
    [CustomEditor(typeof(AllIn1Shader)), CanEditMultipleObjects]
    public class AllIn1ShaderScriptEditor : UnityEditor.Editor
    {
        private bool showUrpWarning = false;
        private double warningTime = 0f;
        private SerializedProperty m_NormalStrength, m_NormalSmoothing;
        private Texture2D imageInspector;
    
        private enum ImageType
        {
            ShowImage,
            HideInComponent,
            HideEverywhere
        }
        private ImageType imageType;

        private void OnEnable()
        {
            m_NormalStrength = serializedObject.FindProperty("normalStrength");
            m_NormalSmoothing = serializedObject.FindProperty("normalSmoothing");
        }

        public override void OnInspectorGUI()
        {
            ChooseAndDisplayAssetImage();

            AllIn1Shader myScript = (AllIn1Shader)target;

            SetCurrentShaderType(myScript);

            if (GUILayout.Button("Deactivate All Effects"))
            {
                for (int i = 0; i < targets.Length; i++) ((AllIn1Shader)targets[i]).ClearAllKeywords();
                AllIn1ShaderWindow.ShowSceneViewNotification("AllIn1SpriteShader: Deactivated All Effects");
            }

            if (GUILayout.Button("New Clean Material"))
            {
                bool successOperation = true;
                for (int i = 0; i < targets.Length; i++)
                {
                    successOperation &= ((AllIn1Shader)targets[i]).TryCreateNew();
                }
                if(successOperation) AllIn1ShaderWindow.ShowSceneViewNotification("AllIn1SpriteShader: Clean Material");
            }


            if (GUILayout.Button("Create New Material With Same Properties (SEE DOC)"))
            {
                bool successOperation = true;
                for (int i = 0; i < targets.Length; i++)
                {
                    successOperation &= ((AllIn1Shader)targets[i]).MakeCopy();
                }
                if(successOperation) AllIn1ShaderWindow.ShowSceneViewNotification("AllIn1SpriteShader: Copy Created");
            }

            if (GUILayout.Button("Save Material To Folder (SEE DOC)"))
            {
                bool successOperation = true;
                for(int i = 0; i < targets.Length; i++)
                {
                    successOperation &= ((AllIn1Shader) targets[i]).SaveMaterial();
                }
                if(successOperation) AllIn1ShaderWindow.ShowSceneViewNotification("AllIn1SpriteShader: Material Saved");
            }

            if (GUILayout.Button("Apply Material To All Children"))
            {
                bool successOperation = true;
                for(int i = 0; i < targets.Length; i++)
                {
                    successOperation &= ((AllIn1Shader) targets[i]).ApplyMaterialToHierarchy();
                }
                if(successOperation) AllIn1ShaderWindow.ShowSceneViewNotification("AllIn1SpriteShader: Material Applied To Children");
                else EditorUtility.DisplayDialog("No children found", "All In 1 Shader component couldn't find any children to this GameObject (" + targets[0].name + ")", "Ok");
            }

            if (myScript.currentShaderType != AllIn1Shader.ShaderTypes.Urp2dRenderer)
            {
                if (GUILayout.Button("Render Material To Image"))
                {
                    bool successOperation = true;
                    for(int i = 0; i < targets.Length; i++)
                    {
                        successOperation &= ((AllIn1Shader) targets[i]).RenderToImage();
                    }
                    if(successOperation) AllIn1ShaderWindow.ShowSceneViewNotification("AllIn1SpriteShader: Material Rendered To Image");
                }
            }

            bool isUrp = false;
            Shader temp = AllIn1ShaderWindow.FindShader("AllIn1Urp2dRenderer");
            if (temp != null) isUrp = true;
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Change Shader Variant:", GUILayout.MaxWidth(140));
                int previousShaderType = (int)myScript.currentShaderType;
                myScript.currentShaderType = (AllIn1Shader.ShaderTypes)EditorGUILayout.EnumPopup(myScript.currentShaderType);
                if (previousShaderType != (int)myScript.currentShaderType)
                {
                    for (int i = 0; i < targets.Length; i++) ((AllIn1Shader)targets[i]).CheckIfValidTarget();
                    if (myScript == null) return;
                    if (isUrp || myScript.currentShaderType != AllIn1Shader.ShaderTypes.Urp2dRenderer)
                    {
                        AllIn1ShaderWindow.SceneViewNotificationAndLog(myScript.gameObject.name + " shader variant has been changed to: " + myScript.currentShaderType);
                        myScript.SetSceneDirty();

                        Renderer sr = myScript.GetComponent<Renderer>();
                        if (sr != null)
                        {
                            if (sr.sharedMaterial != null)
                            {
                                int renderingQueue = sr.sharedMaterial.renderQueue;
                                if (myScript.currentShaderType == AllIn1Shader.ShaderTypes.Default) sr.sharedMaterial.shader = AllIn1ShaderWindow.FindShader("AllIn1SpriteShader");
                                else if (myScript.currentShaderType == AllIn1Shader.ShaderTypes.ScaledTime) sr.sharedMaterial.shader = AllIn1ShaderWindow.FindShader("AllIn1SpriteShaderScaledTime");
                                else if (myScript.currentShaderType == AllIn1Shader.ShaderTypes.MaskedUI) sr.sharedMaterial.shader = AllIn1ShaderWindow.FindShader("AllIn1SpriteShaderUiMask");
                                else if (myScript.currentShaderType == AllIn1Shader.ShaderTypes.Urp2dRenderer) sr.sharedMaterial.shader = AllIn1ShaderWindow.FindShader("AllIn1Urp2dRenderer");
                                else if (myScript.currentShaderType == AllIn1Shader.ShaderTypes.Lit) sr.sharedMaterial.shader = AllIn1ShaderWindow.FindShader("AllIn1SpriteShaderLit");
								else SetCurrentShaderType(myScript);
                                sr.sharedMaterial.renderQueue = renderingQueue;

								if (myScript.currentShaderType == AllIn1Shader.ShaderTypes.Lit)
								{
									sr.sharedMaterial.SetFloat("_ZWrite", 1.0f);
									sr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
									sr.receiveShadows = true;
                                    sr.sharedMaterial.renderQueue = 2000;
                                }
								else
								{
									sr.sharedMaterial.SetFloat("_ZWrite", 0f);
									sr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                                    sr.receiveShadows = false;
                                    sr.sharedMaterial.renderQueue = 3000;
								}
							}
                        }
                        else
                        {
                            Graphic img = myScript.GetComponent<Graphic>();
                            if (img != null && img.material != null)
                            {
                                int renderingQueue = img.material.renderQueue;
                                if (myScript.currentShaderType == AllIn1Shader.ShaderTypes.Default) img.material.shader = AllIn1ShaderWindow.FindShader("AllIn1SpriteShader");
                                else if (myScript.currentShaderType == AllIn1Shader.ShaderTypes.ScaledTime) img.material.shader = AllIn1ShaderWindow.FindShader("AllIn1SpriteShaderScaledTime");
                                else if (myScript.currentShaderType == AllIn1Shader.ShaderTypes.MaskedUI) img.material.shader = AllIn1ShaderWindow.FindShader("AllIn1SpriteShaderUiMask");
                                else if (myScript.currentShaderType == AllIn1Shader.ShaderTypes.Urp2dRenderer) img.material.shader = AllIn1ShaderWindow.FindShader("AllIn1Urp2dRenderer");
                                else if (myScript.currentShaderType == AllIn1Shader.ShaderTypes.Lit) img.material.shader = AllIn1ShaderWindow.FindShader("AllIn1SpriteShaderLit");
								else SetCurrentShaderType(myScript);
                                img.material.renderQueue = renderingQueue;
                            }
                        }
                    }
                    else if(!isUrp && myScript.currentShaderType == AllIn1Shader.ShaderTypes.Urp2dRenderer)
                    {
                        myScript.currentShaderType = (AllIn1Shader.ShaderTypes) previousShaderType;
                        showUrpWarning = true;
                        warningTime = EditorApplication.timeSinceStartup + 5;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            if (warningTime < EditorApplication.timeSinceStartup) showUrpWarning = false;
            if (isUrp) showUrpWarning = false;
            if (showUrpWarning) EditorGUILayout.HelpBox(
                "You can't set the URP 2D Renderer variant since you didn't import the URP package available in the asset root folder (SEE DOCUMENTATION)",
                MessageType.Error,
                true);

            if ((isUrp && myScript.currentShaderType == AllIn1Shader.ShaderTypes.Urp2dRenderer) || myScript.currentShaderType == AllIn1Shader.ShaderTypes.Lit)
            {
                EditorGUILayout.Space();
                DrawLine(Color.grey, 1, 3);
                EditorGUILayout.Space();

                if (GUILayout.Button("Create And Add Normal Map"))
                {
                    for (int i = 0; i < targets.Length; i++) ((AllIn1Shader)targets[i]).CreateAndAssignNormalMap();
                    AllIn1ShaderWindow.ShowSceneViewNotification("AllIn1SpriteShader: Creating Normal Map");
                    
                }
                serializedObject.Update();
                EditorGUILayout.PropertyField(m_NormalStrength, new GUIContent("Normal Strength"), GUILayout.Height(20));
                EditorGUILayout.PropertyField(m_NormalSmoothing, new GUIContent("Normal Blur"), GUILayout.Height(20));
                if (myScript.computingNormal)
                {
                    EditorGUILayout.LabelField("Normal Map is currently being created, be patient", EditorStyles.boldLabel, GUILayout.Height(40));
                }
                serializedObject.ApplyModifiedProperties();

                EditorGUILayout.Space();
            }

            DrawLine(Color.grey, 1, 3);
            EditorGUILayout.Space();

            if (GUILayout.Button("Sprite Atlas Auto Setup"))
            {
                bool successOperation = true;
                for(int i = 0; i < targets.Length; i++)
                {
                    successOperation &= ((AllIn1Shader) targets[i]).ToggleSetAtlasUvs(true);
                }
                if(successOperation) AllIn1ShaderWindow.ShowSceneViewNotification("AllIn1SpriteShader: Sprite Atlas Auto Setup");
            }
            if (GUILayout.Button("Remove Sprite Atlas Configuration"))
            {
                bool successOperation = true;
                for(int i = 0; i < targets.Length; i++)
                {
                    successOperation &= ((AllIn1Shader) targets[i]).ToggleSetAtlasUvs(false);
                }
                if(successOperation) AllIn1ShaderWindow.ShowSceneViewNotification("AllIn1SpriteShader: Remove Sprite Atlas Configuration");
            }
            
#if LETAI_TRUESHADOW
            if (myScript.GetComponent<LeTai.TrueShadow.TrueShadow>() && !myScript.GetComponent<TrueShadowCompatibility>())
            {
                EditorGUILayout.Space();
                DrawLine(Color.grey, 1, 3);
                if (GUILayout.Button("Add True Shadow Compatibility"))
                {
                    myScript.gameObject.AddComponent<TrueShadowCompatibility>();
                    myScript.SetSceneDirty();
                }
            }
#endif

            EditorGUILayout.Space();
            DrawLine(Color.grey, 1, 3);
            
            if(GUILayout.Button("Remove Component"))
            {
                for(int i = targets.Length - 1; i >= 0; i--)
                {
                    DestroyImmediate(targets[i] as AllIn1Shader);
                    ((AllIn1Shader)targets[i]).SetSceneDirty();
                }
                AllIn1ShaderWindow.ShowSceneViewNotification("AllIn1SpriteShader: Component Removed");
            }

            if (GUILayout.Button("REMOVE COMPONENT AND MATERIAL"))
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    ((AllIn1Shader)targets[i]).CleanMaterial();
                }
                for (int i = targets.Length - 1; i >= 0; i--)
                {
                    DestroyImmediate(targets[i] as AllIn1Shader);
                }
                AllIn1ShaderWindow.ShowSceneViewNotification("AllIn1SpriteShader: Component And Material Removed");
            }
        }

        private void ChooseAndDisplayAssetImage()
        {
            if(!EditorPrefs.HasKey("allIn1ImageConfig"))
            {
                EditorPrefs.SetInt("allIn1ImageConfig", (int) ImageType.ShowImage);
            }

            imageType = (ImageType) EditorPrefs.GetInt("allIn1ImageConfig");
            switch(imageType)
            {
                case ImageType.ShowImage:
                case ImageType.HideInComponent:
                    if(imageInspector == null) imageInspector = AllIn1ShaderWindow.GetInspectorImage();
                    break;
            }

            if(imageInspector && imageType != ImageType.HideInComponent && imageType != ImageType.HideEverywhere && imageInspector)
            {
                Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(40));
                GUI.DrawTexture(rect, imageInspector, ScaleMode.ScaleToFit, true);
            }
        }

        private void SetCurrentShaderType(AllIn1Shader myScript)
        {
            string shaderName = "";
            Renderer sr = myScript.GetComponent<Renderer>();
            if (sr != null)
            {
                if(sr.sharedMaterial == null) return;
                shaderName = sr.sharedMaterial.shader.name;
            }
            else
            {
                Graphic img = myScript.GetComponent<Graphic>();
                if(img.material == null) return;
                if (img != null) shaderName = img.material.shader.name;
            }
            shaderName = shaderName.Replace("AllIn1SpriteShader/", "");

            if (shaderName.Equals("AllIn1SpriteShader")) myScript.currentShaderType = AllIn1Shader.ShaderTypes.Default;
            else if (shaderName.Equals("AllIn1SpriteShaderScaledTime")) myScript.currentShaderType = AllIn1Shader.ShaderTypes.ScaledTime;
            else if (shaderName.Equals("AllIn1SpriteShaderUiMask")) myScript.currentShaderType = AllIn1Shader.ShaderTypes.MaskedUI;
            else if (shaderName.Equals("AllIn1Urp2dRenderer")) myScript.currentShaderType = AllIn1Shader.ShaderTypes.Urp2dRenderer;
			else if(shaderName.Equals("AllIn1SpriteShaderLit")) myScript.currentShaderType = AllIn1Shader.ShaderTypes.Lit;
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
    }
}
#endif