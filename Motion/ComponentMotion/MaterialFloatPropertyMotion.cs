using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Motion that moves a Material float property value towards a target value.
    /// </summary>
    public class MaterialFloatPropertyMotion : FloatMotion
    {
        public Material Material { get; private set; }
        public string PropertyName { get; private set; }

        public MaterialFloatPropertyMotion() : base(1, null, null) { }
        public MaterialFloatPropertyMotion(float duration) : base(duration, null, null) { }
        public MaterialFloatPropertyMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public MaterialFloatPropertyMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public MaterialFloatPropertyMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(Material material, string propertyName, float targetValue)
        {
            if (!material.HasProperty(propertyName))
                throw new MissingMemberException("material does not contain the property: " + propertyName);

            Material = material;
            PropertyName = propertyName;
            SetValues(() => this.Material.GetFloat(this.PropertyName), targetValue);
        }

        public void SetParameters(Material material, string propertyName, float startValue, float targetValue)
        {
            if (!material.HasProperty(propertyName))
                throw new MissingMemberException("material does not contain the property: " + propertyName);

            Material = material;
            PropertyName = propertyName;
            SetValues(startValue, targetValue);
        }

        protected override void OnValueChanged()
        {
            if (Material != null)
            {
                Material.SetFloat(PropertyName, CurrentValue);
            }
        }
    }
}
