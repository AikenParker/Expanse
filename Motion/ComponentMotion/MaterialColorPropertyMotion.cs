using System;
using UnityEngine;

namespace Expanse.Motion
{
    /// <summary>
    /// Motion that moves a Material color property value towards a target value.
    /// </summary>
    public class MaterialColorPropertyMotion : ColorMotion
    {
        public Material Material { get; private set; }
        public string PropertyName { get; private set; }

        public MaterialColorPropertyMotion() : base(1, null, null) { }
        public MaterialColorPropertyMotion(float duration) : base(duration, null, null) { }
        public MaterialColorPropertyMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public MaterialColorPropertyMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public MaterialColorPropertyMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(Material material, string propertyName, Color targetColor)
        {
            if (!material.HasProperty(propertyName))
                throw new MissingMemberException("material does not contain the property: " + propertyName);

            Material = material;
            PropertyName = propertyName;
            SetValues(() => this.Material.GetColor(this.PropertyName), targetColor);
        }

        public void SetParameters(Material material, string propertyName, Color startColor, Color targetColor)
        {
            if (!material.HasProperty(propertyName))
                throw new MissingMemberException("material does not contain the property: " + propertyName);

            Material = material;
            PropertyName = propertyName;
            SetValues(startColor, targetColor);
        }

        protected override void OnValueChanged()
        {
            if (Material != null)
            {
                Material.SetColor(PropertyName, CurrentValue);
            }
        }
    }
}
