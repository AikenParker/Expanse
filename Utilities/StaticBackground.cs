using UnityEngine;

namespace Expanse.Utilities
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class StaticBackground : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private Transform Transform;

        [SerializeField, HideInInspector]
        private SpriteRenderer SpriteRenderer;

        [SerializeField]
        private Camera Camera;

        public Vector2 screenPosition = new Vector2(0.5f, 0.5f);
        public ScaleMode scaleMode = ScaleMode.MaxFit;
        public float scaleFactor = 1f;

        private void Reset()
        {
            Transform = this.transform;
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Camera = Camera.main;

            InvalidateTransform();
        }

        public void InvalidateTransform()
        {
            if (SpriteRenderer.sprite == null || Camera == null)
                return;

            Vector3 newPosition = Camera.ViewportToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0f));
            newPosition.z = 0f;

            Transform.position = newPosition;

            switch (scaleMode)
            {
                case ScaleMode.Native:
                    Transform.localScale = Vector3.zero;
                    break;

                case ScaleMode.MinFit:
                    Transform.localScale = this.GetMinScale() * scaleFactor;
                    break;

                case ScaleMode.MaxFit:
                    Transform.localScale = this.GetMaxScale() * scaleFactor;
                    break;

                case ScaleMode.Stretch:
                    Transform.localScale = this.GetStretchScale() * scaleFactor;
                    break;
            }
        }

        private Vector2 GetSpriteSizeRatio()
        {
            Vector3 spriteSize = SpriteRenderer.sprite.bounds.size;

            Vector3 camCornerA = Camera.ViewportToWorldPoint(Vector3.zero);
            Vector3 camCornerB = Camera.ViewportToWorldPoint(Vector3.one);

            float widthRatio = (camCornerB.x - camCornerA.x) / spriteSize.x;
            float heightRatio = (camCornerB.y - camCornerA.y) / spriteSize.y;

            return new Vector2(widthRatio, heightRatio);
        }

        private Vector3 GetMinScale()
        {
            Vector2 spriteSizeRatio = this.GetSpriteSizeRatio();

            float minRatio = Mathf.Min(spriteSizeRatio.x, spriteSizeRatio.y);

            return new Vector3(minRatio, minRatio, 1f);
        }

        private Vector3 GetMaxScale()
        {
            Vector2 spriteSizeRatio = this.GetSpriteSizeRatio();

            float maxRatio = Mathf.Max(spriteSizeRatio.x, spriteSizeRatio.y);

            return new Vector3(maxRatio, maxRatio, 1f);
        }

        private Vector3 GetStretchScale()
        {
            Vector2 spriteSizeRatio = this.GetSpriteSizeRatio();

            return new Vector3(spriteSizeRatio.x, spriteSizeRatio.y, 1f);
        }

        public enum ScaleMode
        {
            Native = 0,
            MaxFit,
            MinFit,
            Stretch
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("CONTEXT/StaticBackground/Invalidate Transform")]
        static void InvalidateTransform(UnityEditor.MenuCommand command)
        {
            StaticBackground staticBackground = (StaticBackground)command.context;

            UnityEditor.Undo.RecordObject(staticBackground.Transform, staticBackground.name + " Invalidate Transform");

            staticBackground.InvalidateTransform();
        }
#endif
    }
}
