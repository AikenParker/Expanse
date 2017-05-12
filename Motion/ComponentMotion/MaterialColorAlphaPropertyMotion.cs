using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Expanse
{
    /// <summary>
    /// Motion that moves a Material color property alpha value towards a target alpha value.
    /// </summary>
    public class MaterialColorAlphaPropertyMotion : FloatMotion
    {
        public Material Material { get; private set; }
        public string PropertyName { get; private set; }

        public MaterialColorAlphaPropertyMotion() : base(1, null, null) { }
        public MaterialColorAlphaPropertyMotion(float duration) : base(duration, null, null) { }
        public MaterialColorAlphaPropertyMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public MaterialColorAlphaPropertyMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public MaterialColorAlphaPropertyMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(Material material, string propertyName, float targetAlpha)
        {
            if (!material.HasProperty(propertyName))
                throw new MissingMemberException("material does not contain the property: " + propertyName);

            Material = material;
            PropertyName = propertyName;
            SetValues(() => this.Material.GetColor(this.PropertyName).a, targetAlpha);
        }

        public void SetParameters(Material material, string propertyName, float startAlpha, float targetAlpha)
        {
            if (!material.HasProperty(propertyName))
                throw new MissingMemberException("material does not contain the property: " + propertyName);

            Material = material;
            PropertyName = propertyName;
            SetValues(startAlpha, targetAlpha);
        }

        protected override void OnValueChanged()
        {
            if (Material != null)
            {
                Material.SetColor(PropertyName, Material.GetColor(PropertyName).WithAlpha(CurrentValue));
            }
        }
    }
}
