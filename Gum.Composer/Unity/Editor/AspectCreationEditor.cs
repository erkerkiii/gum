#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Gum.Composer.CodeGen;
using Gum.Composer.CodeGen.Internal;
using Gum.Core.Utility;
using UnityEditor;
using UnityEngine;

namespace Gum.Composer.Unity.Editor
{
    public class AspectCreationEditor : EditorWindow
    {
        private static readonly Type[] AspectTypes = typeof(IAspect).GetDerivedTypesOfInterface();

        private readonly Dictionary<string, string> _fieldNameTypeMap = new();

        private readonly string[] _availabeTypeArray = GetAvailableTypeArray();

        private string _attemptedAspectName = string.Empty;
        private string _acceptedAspectName = string.Empty;
        private string _fieldType = string.Empty;
        private string _fieldName = string.Empty;

        private ErrorType _errorType = ErrorType.None;

        [MenuItem("Gum/Composition/AspectCreator")]
        public static void ShowWindow()
        {
            AspectCreationEditor aspectCreationEditor = GetWindow<AspectCreationEditor>("Aspect Creator");
            aspectCreationEditor.Show();
        }
        
        private static string[] GetAvailableTypeArray()
        {
            Type[] types = typeof(IAspect).GetAllAvailableTypes();
            string[] typeNames = new string[types.Length];

            for (int index = 0; index < types.Length; index++)
            {
                typeNames[index] = types[index].ToString();
            }
            return typeNames;
        }

        private void OnGUI()
        {
            DrawInfoBar();

            if (IsStringEmpty(_acceptedAspectName))
            {
                TryTakeAspectName();
                return;
            }

            DrawEnteredAspectFields();
            DrawAddFieldDrawer();
            DrawGenerateButton();

            if (_errorType != ErrorType.None)
            {
                DrawErrorPopup();
            }
        }

        private void DrawErrorPopup()
        {
            EditorGUILayout.BeginHorizontal();

            const string fieldAlreadyExist = "Field already exist";
            const string aspectAlreadyExist = "Aspect already exist";

            string error = String.Empty;
            
            switch (_errorType)
            {
                case ErrorType.AlreadyExistingFieldNameError:
                    error = fieldAlreadyExist;
                    break;
                case ErrorType.AlreadyExistingAspectError:
                    error = aspectAlreadyExist;
                    break;
            }

            GUILayout.Label(error, "CN StatusError");
            
            if (GUILayout.Button("Close", "MiniButton"))
            {
                _errorType = ErrorType.None;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawInfoBar()
        {
            EditorGUILayout.BeginVertical("Button");

            if (!IsStringEmpty(_acceptedAspectName))
            {
                GUILayout.Label("First enter type and name for fields.");
                GUILayout.Label("Then run code generator.");
            }
            else
            {
                GUILayout.Label("This is Aspect creator window.");
                GUILayout.Label("Please enter aspect type name for creating new aspect.");
            }

            EditorGUILayout.EndVertical();
        }

        private void TryTakeAspectName()
        {
            EditorGUILayout.BeginVertical();

            _attemptedAspectName = EditorGUILayout.TextField("AspectName", _attemptedAspectName);
            Repaint();

            if (_attemptedAspectName != String.Empty &&
                GUILayout.Button($"Name new aspect : {_attemptedAspectName}Aspect"))
            {
                for (int index = 0; index < AspectTypes.Length; index++)
                {
                    Type aspectType = AspectTypes[index];

                    if ((string.Compare(aspectType.Name, _attemptedAspectName, StringComparison.Ordinal) == 0))
                    {
                        _errorType = ErrorType.AlreadyExistingAspectError;

                        EditorGUILayout.EndVertical();
                        DrawErrorPopup();
                        return;
                    }
                }
                
                _acceptedAspectName = _attemptedAspectName;
                _attemptedAspectName = string.Empty;
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawEnteredAspectFields()
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField(_acceptedAspectName);

            for (int index = 0; index < _fieldNameTypeMap.Count; index++)
            {
                KeyValuePair<string, string> item = _fieldNameTypeMap.ElementAt(index);
                DrawFieldUI(item.Value, item.Key);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawFieldUI(string fieldType, string fieldName)
        {
            EditorGUILayout.BeginHorizontal("BoldLabel");

            EditorGUILayout.LabelField(fieldType);
            EditorGUILayout.LabelField(fieldName);

            if (GUILayout.Button("x", "ToolbarSeachCancelButton"))
            {
                _fieldNameTypeMap.Remove(fieldName);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawAddFieldDrawer()
        {
            EditorGUILayout.BeginHorizontal("ToolbarButton");

            _fieldType = EditorGUILayout.TextField("Type", _fieldType, "MiniTextField");
            _fieldName = EditorGUILayout.TextField("Name", _fieldName, "MiniTextField");

            EditorGUILayout.EndHorizontal();

            if (!IsStringEmpty(_fieldName) && !IsStringEmpty(_fieldType) &&
                GUILayout.Button($"Add field: {_fieldType} {_fieldName}", "MiniButtonLeft"))
            {
                if (_fieldNameTypeMap.ContainsKey(_fieldName))
                {
                    _errorType = ErrorType.AlreadyExistingFieldNameError;
                    return;
                }

                _fieldNameTypeMap.Add(_fieldName, _fieldType);
                _fieldName = string.Empty;
            }
        }

        private void DrawGenerateButton()
        {
            if (GUILayout.Button("Generate Aspect", "Button"))
            {
                if (IsStringEmpty(_acceptedAspectName))
                {
                    return;
                }

                AspectPrototype aspectPrototype = new AspectPrototype(_acceptedAspectName, _fieldNameTypeMap);
                List<AspectPrototype> aspectPrototypes = new List<AspectPrototype>();
                aspectPrototypes.Add(aspectPrototype);

                AspectFileWriter.WriteAspects(aspectPrototypes);
                CompositionCodeGenerator.Run();
            }
        }

        private bool IsStringEmpty(string s) => s == string.Empty;

        private enum ErrorType
        {
            None,
            AlreadyExistingAspectError,
            AlreadyExistingFieldNameError
        }
    }
}
#endif