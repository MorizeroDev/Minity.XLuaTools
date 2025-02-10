#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Minity.XLuaTools.Editor
{
    [CustomPropertyDrawer(typeof(LuaBehaviour.Injection))]
    public class LuaInjectionDrawer : PropertyDrawer
    {
        private DropdownField typeField;
        private PropertyField nameField;
        private HelpBox emptyName, invalidType;

        private SerializedProperty nameProp, typeProp;
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var outerContainer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Column
                }
            };
            
            emptyName = new HelpBox("The injection name can not be empty.", HelpBoxMessageType.Error);
            invalidType = new HelpBox("The injection type is invalid.", HelpBoxMessageType.Error);
            
            nameProp = property.FindPropertyRelative("Name");
            typeProp = property.FindPropertyRelative("Type");
            
            var container = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    justifyContent = Justify.SpaceBetween,
                }
            };

            var objField = new PropertyField(property.FindPropertyRelative("Object"), "")
            {
                style =
                {
                    width = new StyleLength(Length.Percent(30f))
                }
            };
            container.Add(objField);
            
            var obj = (GameObject)property.FindPropertyRelative("Object").objectReferenceValue;
            var components = new List<Component>();
            if (obj)
            {
                obj.GetComponents(components);
            }
            var types = components.Select(x => x.GetType().Name).Distinct().ToList();
            types.Insert(0, "GameObject");
                
            nameField = new PropertyField(nameProp, "")            
            {
                style =
                {
                    width = new StyleLength(Length.Percent(30f))
                }
            };
            typeField = new DropdownField(types, 
                Math.Max(0, types.FindIndex(x => x == typeProp.stringValue)))
            {
                style =
                {
                    width = new StyleLength(Length.Percent(40f))
                }
            };
            typeField.BindProperty(typeProp);
            
            container.Add(nameField);
            container.Add(typeField);

            if (!obj)
            {
                typeField.visible = false;
            }
            
            objField.RegisterValueChangeCallback(OnObjectChanged);
            nameField.RegisterValueChangeCallback((e) =>
            {
                emptyName.style.display = string.IsNullOrEmpty(e.changedProperty.stringValue)
                                                    ? DisplayStyle.Flex 
                                                    : DisplayStyle.None;
            });
            typeField.RegisterValueChangedCallback(e =>
            {
                invalidType.style.display = !typeField.choices.Contains(e.newValue) 
                                                    ? DisplayStyle.Flex 
                                                    : DisplayStyle.None;
            });
            
            outerContainer.Add(container);
            
            outerContainer.Add(emptyName);
            outerContainer.Add(invalidType);
            
            return outerContainer;
        }

        private void Check()
        {
            emptyName.style.display = string.IsNullOrEmpty(nameProp.stringValue)
                                                ? DisplayStyle.Flex 
                                                : DisplayStyle.None;
            invalidType.style.display = !typeField.choices.Contains(typeProp.stringValue) 
                                                ? DisplayStyle.Flex 
                                                : DisplayStyle.None;
        }

        private void OnObjectChanged(SerializedPropertyChangeEvent e)
        {
            if (e.changedProperty.objectReferenceValue)
            {
                var components = new List<Component>();
                ((GameObject)e.changedProperty.objectReferenceValue).GetComponents(components);
         
                var types = components.Select(x => x.GetType().Name).Distinct().ToList();
                types.Insert(0, "GameObject");
                typeField.choices = types;
                
                typeField.visible = true;
            }
            else
            {
                typeField.visible = false;
            }
            Check();
        }
    }
}
#endif
