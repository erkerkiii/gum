#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using Gum.Composer.CodeGen;
using Gum.Core.Utility;
using UnityEditor;
using UnityEngine;

namespace Gum.Composer.Unity.Editor
{
    public class AspectEditor : EditorWindow
    {
        private static readonly Type[] AspectTypes = typeof(IAspect).GetDerivedTypesOfInterface();

        private readonly Dictionary<Type, bool> _foldoutGUIMap = new Dictionary<Type, bool>();

        [MenuItem("Gum/Composition/AspectEditor")]
        public static void ShowWindow()
        {
            GetWindow<AspectEditor>("Aspect Editor");
        }

        private void OnGUI()
        {
            for (int index = 0; index < AspectTypes.Length; index++)
            {
                Type aspectType = AspectTypes[index];
                if (!_foldoutGUIMap.ContainsKey(aspectType))
                {
                    _foldoutGUIMap.Add(aspectType, false);
                }

                _foldoutGUIMap[aspectType] =
                    EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutGUIMap[aspectType], aspectType.Name);

                if (_foldoutGUIMap[aspectType])
                {
                    EditorGUILayout.LabelField("Aspect Type : " + aspectType);

                    FieldInfo[] propertyInfos = aspectType.GetFields();
                    for (int order = 0; order < propertyInfos.Length; order++)
                    {
                        EditorGUILayout.LabelField(propertyInfos[order].FieldType + propertyInfos[order].Name);
                    }
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            if (GUILayout.Button("Run CodeGen"))
            {
                CompositionCodeGenerator.Run();
            }
        }
    }
}
#endif