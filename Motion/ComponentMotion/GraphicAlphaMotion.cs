using UnityEngine;
using UnityEngine.UI;

namespace Expanse
{
    /// <summary>
    /// Motion that moves an Graphic alpha value towards a target alpha value.
    /// </summary>
    public class GraphicAlphaMotion : FloatMotion
    {
        public Graphic Graphic { get; private set; }

        public GraphicAlphaMotion() : base(1, null, null) { }
        public GraphicAlphaMotion(float duration) : base(duration, null, null) { }
        public GraphicAlphaMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public GraphicAlphaMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public GraphicAlphaMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(Graphic graphic, float targetAlpha)
        {
            Graphic = graphic;
            SetValues(() => this.Graphic.color.a, targetAlpha);
        }

        public void SetParameters(Graphic graphic, float startAlpha, float targetAlpha)
        {
            Graphic = graphic;
            SetValues(startAlpha, targetAlpha);
        }

        protected override void OnValueChanged()
        {
            if (Graphic != null)
            {
                Graphic.color = Graphic.color.WithAlpha(CurrentValue);
            }
        }
    }
}
