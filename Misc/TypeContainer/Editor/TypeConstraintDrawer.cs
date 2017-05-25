using UnityEngine;
using UnityEditor;

namespace Expanse.Misc
{
    /// <summary>
    /// Custom drawer for TypeConstraintAttribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(TypeConstraintAttribute))]
    public class TypeConstraintDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (fieldInfo.FieldType != typeof(TypeContainer))
            {
                Debug.LogWarning("TypeConstraint attribute can only be applied to TypeContainer fields");
                return;
            }

            Utilities.EditorUtil.ApplyTooltip(fieldInfo, label);

            TypeConstraintAttribute typeConstraintAttribute = (TypeConstraintAttribute)attribute;

            //

            TypeContainerDrawer typeContainerDrawer = new TypeContainerDrawer()
            {
                baseType = typeConstraintAttribute.BaseType,
                nonAbstractOnly = typeConstraintAttribute.NonAbstractOnly
            };

            typeContainerDrawer.OnGUI(position, property, label);
        }
    }
}