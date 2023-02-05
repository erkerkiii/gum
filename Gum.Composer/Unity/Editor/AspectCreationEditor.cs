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

        private readonly List<string> _availableTypesAsString = TypeFileReader.ReadTypesAsString().ToList();

        private string _attemptedAspectName = string.Empty;
        private string _acceptedAspectName = string.Empty;
        private string _fieldType = string.Empty;
        private string _fieldName = string.Empty;
        private string _typeName;

        private bool _typeDrawerToggle;
        
        private Vector2 _scrollToggle;

        private int _typeListIndex;

        private ErrorType _errorType = ErrorType.None;

        [MenuItem("Gum/Composition/AspectCreator")]
        public static void ShowWindow()
        {
            AspectCreationEditor aspectCreationEditor = GetWindow<AspectCreationEditor>("Aspect Creator");
            aspectCreationEditor.Show();

            aspectCreationEditor.minSize = new Vector2(450, 200);
            aspectCreationEditor.maxSize = new Vector2(1920, 720);
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
            DrawTypeDrawer();

            if (_errorType != ErrorType.None)
            {
                DrawErrorPopup();
            }
        }

        private void DrawTypeDrawer()
        {
            EditorGUILayout.BeginVertical();
            _typeDrawerToggle = EditorGUILayout.BeginFoldoutHeaderGroup(_typeDrawerToggle, "Edit Available Types");
            if (_typeDrawerToggle)
            {
                _scrollToggle = EditorGUILayout.BeginScrollView(_scrollToggle, false, true);

                ListAllAvailableTypes();
                AddNewType();

                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();
        }

        private void ListAllAvailableTypes()
        {
            if (_availableTypesAsString.Count <= 0)
            {
                return;
            }
            
            EditorGUILayout.BeginVertical("Box");
            for (var index = 0; index < _availableTypesAsString.Count; index++)
            {
                string type = _availableTypesAsString[index];
                DrawTypeUI(type);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawTypeUI(string type)
        {
            EditorGUILayout.BeginHorizontal("BoldLabel");
            EditorGUILayout.LabelField(type);

            if (GUILayout.Button("x", "ToolbarSeachCancelButton"))
            {
                _availableTypesAsString.Remove(type);
                TypeFileWriter.WriteTypes(_availableTypesAsString);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void AddNewType()
        {
            _typeName = EditorGUILayout.TextField("Add New Type", _typeName, "TextField");

            if (!IsStringEmpty(_typeName) && GUILayout.Button($"Add new type {_typeName}"))
            {
                if (_availableTypesAsString.Contains(_typeName))
                {
                    return;
                }
                
                _availableTypesAsString.Add(_typeName);
                TypeFileWriter.WriteTypes(_availableTypesAsString);
                _typeName = string.Empty;
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

            _typeListIndex = EditorGUILayout.Popup(_typeListIndex, _availableTypesAsString.ToArray());
            _fieldType = _availableTypesAsString.Count <= 0 
                ? string.Empty 
                : _availableTypesAsString[_typeListIndex];
            
            _fieldName = EditorGUILayout.TextField("Name", _fieldName, "TextField");

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