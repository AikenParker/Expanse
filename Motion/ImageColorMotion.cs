using UnityEngine;
using UnityEngine.UI;

namespace Expanse
{
    /// <summary>
    /// Motion that moves an Image color value towards a target color.
    /// </summary>
    public class ImageColorMotion : ColorMotion
    {
        public Image Image { get; private set; }

        public ImageColorMotion() : base(1, null, null) { }
        public ImageColorMotion(float duration) : base(duration, null, null) { }
        public ImageColorMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public ImageColorMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public ImageColorMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(Image image, Color targetColor)
        {
            Image = image;
            SetParameters(() => this.Image.color, targetColor);
        }

        public void SetParameters(Image image, Color startColor, Color targetColor)
        {
            Image = image;
            SetParameters(startColor, targetColor);
        }

        protected override void OnValueChanged()
        {
            if (Image != null)
            {
                Image.color = CurrentValue;
            }
        }
    }
}
