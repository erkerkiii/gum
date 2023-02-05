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

        private List<string> _availableTypesAsString = new List<string>();
        private List<string> AvailableTypesAsString
        {
            get
            {
                if (_availableTypesAsString.Count <= 0)
                {
                    _availableTypesAsString = TypeFileReader.ReadTypesAsString().ToList();
                }
                
                return _availableTypesAsString;
            }
        }

        private string _attemptedAspectName = string.Empty;
        private string _acceptedAspectName = string.Empty;
        private string _fieldType = string.Empty;
        private string _fieldName = string.Empty;
        private string _typeName;

        private bool _typeDrawerToggle;

        private Vector2 _scrollToggle;

        private int _typeListIndex;
        
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
            if (AvailableTypesAsString.Count <= 0)
            {
                return;
            }

            EditorGUILayout.BeginVertical("Box");
            for (var index = 0; index < AvailableTypesAsString.Count; index++)
            {
                string type = AvailableTypesAsString[index];
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
                AvailableTypesAsString.Remove(type);
                TypeFileWriter.WriteTypes(AvailableTypesAsString);
                Repaint();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void AddNewType()
        {
            _typeName = EditorGUILayout.TextField("Add New Type", _typeName, "TextField");

            if (GUILayout.Button($"Add new type {_typeName}"))
            {
                TypeNameValidationResult typeNameValidationResult = ValidateTypeName(out string fullName);
                if (typeNameValidationResult != TypeNameValidationResult.Success)
                {
                    Debug.LogError(
                        $"Error while trying to add type: {_typeName} Error code: {typeNameValidationResult}");
                    return;
                }

                AvailableTypesAsString.Add(fullName);
                TypeFileWriter.WriteTypes(AvailableTypesAsString);
                _typeName = string.Empty;
                Repaint();
            }
        }

        private TypeNameValidationResult ValidateTypeName(out string fullName)
        {
            fullName = string.Empty;
            if (IsStringEmpty(_typeName))
            {
                return TypeNameValidationResult.InvalidTypeNameError;
            }

            foreach (Type type in AppDomain.CurrentDomain
                         .GetAssemblies()
                         .SelectMany(a => a.GetTypes()))
            {
                if (_typeName == type.Name || _typeName == type.FullName)
                {
                    fullName = type.FullName;
                    return !AvailableTypesAsString.Contains(fullName) 
                        ? TypeNameValidationResult.Success 
                        : TypeNameValidationResult.TypeNameAlreadyExistError;
                }
            }

            return TypeNameValidationResult.InvalidTypeNameError;
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
                        EditorGUILayout.EndVertical();
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

            _typeListIndex = EditorGUILayout.Popup(_typeListIndex, AvailableTypesAsString.ToArray());
            _fieldType = AvailableTypesAsString.Count <= 0
                ? string.Empty
                : AvailableTypesAsString[_typeListIndex];

            _fieldName = EditorGUILayout.TextField("Name", _fieldName, "TextField");

            EditorGUILayout.EndHorizontal();

            if (!IsStringEmpty(_fieldName) && !IsStringEmpty(_fieldType) &&
                GUILayout.Button($"Add field: {_fieldType} {_fieldName}", "MiniButtonLeft"))
            {
                if (_fieldNameTypeMap.ContainsKey(_fieldName))
                {
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

        private enum TypeNameValidationResult
        {
            Success,
            TypeNameAlreadyExistError,
            InvalidTypeNameError
        }
    }
}
#endif