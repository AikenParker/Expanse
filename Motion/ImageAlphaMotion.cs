using UnityEngine;
using UnityEngine.UI;

namespace Expanse
{
    /// <summary>
    /// Motion that moves an Image alpha value towards a target alpha value.
    /// </summary>
    public class ImageAlphaMotion : FloatMotion
    {
        public Image Image { get; private set; }

        public ImageAlphaMotion() : base(1, null, null) { }
        public ImageAlphaMotion(float duration) : base(duration, null, null) { }
        public ImageAlphaMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public ImageAlphaMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public ImageAlphaMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(Image image, float targetAlpha)
        {
            Image = image;
            SetParameters(() => this.Image.color.a, targetAlpha);
        }

        public void SetParameters(Image image, float startAlpha, float targetAlpha)
        {
            Image = image;
            SetParameters(startAlpha, targetAlpha);
        }

        protected override void OnValueChanged()
        {
            if (Image != null)
            {
                Image.color = Image.color.WithAlpha(CurrentValue);
            }
        }
    }
}
