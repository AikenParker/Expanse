﻿using UnityEngine;
using Rect = UnityEngine.Rect;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of Rect related extension methods.
    /// </summary>
    public static class RectExt
    {
        public enum RectMode
        {
            TopLeft,
            Center
        }

        /// <summary>
        /// Creates a new rect with added width and height.
        /// </summary>
        public static Rect AddSize(this Rect source, float addWidth, float addHeight, RectMode rectMode = RectMode.TopLeft)
        {
            Rect newRect = new Rect(source);

            switch (rectMode)
            {
                case RectMode.TopLeft:
                    newRect.width += addWidth;
                    newRect.height += addHeight;
                    break;

                case RectMode.Center:
                    newRect.width += addWidth;
                    newRect.height += addHeight;
                    newRect.x -= addWidth / 2f;
                    newRect.y -= addHeight / 2f;
                    break;
            }

            return newRect;
        }

        /// <summary>
        /// Creates a new rect with set width and height.
        /// </summary>
        public static Rect WithSize(this Rect source, float width, float height, RectMode rectMode = RectMode.TopLeft)
        {
            Rect newRect = new Rect(source);

            switch (rectMode)
            {
                case RectMode.TopLeft:
                    newRect.width = width;
                    newRect.height = height;
                    break;

                case RectMode.Center:
                    float widthDiff = newRect.width - width;
                    float heightDiff = newRect.height - height;
                    newRect.width += widthDiff;
                    newRect.height += heightDiff;
                    newRect.x -= widthDiff / 2f;
                    newRect.y -= heightDiff / 2f;
                    break;
            }

            return newRect;
        }

        /// <summary>
        /// Creates a new rect with added position.
        /// </summary>
        public static Rect AddPosition(this Rect source, float addedX, float addedY)
        {
            Rect newRect = new Rect(source);
            newRect.position += new Vector2(addedX, addedY);
            return newRect;
        }

        /// <summary>
        /// Creates a new rect with set position.
        /// </summary>
        public static Rect WithPosition(this Rect source, Vector2 newPosition)
        {
            Rect newRect = new Rect(source);
            newRect.position = newPosition;
            return newRect;
        }

        /// <summary>
        /// Creates a rect with a scaled size.
        /// </summary>
        public static Rect Scale(this Rect source, Vector2 scale, RectMode rectMode = RectMode.TopLeft)
        {
            Rect newRect = new Rect(source);

            switch (rectMode)
            {
                case RectMode.TopLeft:
                    newRect.width *= scale.x;
                    newRect.height *= scale.y;
                    break;

                case RectMode.Center:
                    float widthDiff = newRect.width - (newRect.width * scale.x);
                    float heightDiff = newRect.height - (newRect.height * scale.x);
                    newRect.width += widthDiff;
                    newRect.height += heightDiff;
                    newRect.x -= widthDiff / 2f;
                    newRect.y -= heightDiff / 2f;
                    break;
            }

            return newRect;
        }

        /// <summary>
        /// Returns a new rect defined as 1 of X rects evenly sized horizontally within a larger rect.
        /// </summary>
        public static Rect SplitWidth(this Rect source, int index, int count, float spacing = 0f)
        {
            if (source.width == 0)
                return source;

            float totalSpacing = spacing * (count - 1);
            float workableWidth = source.width - totalSpacing;
            float singleWidth = workableWidth / count;
            float step = singleWidth + spacing;
            float startX = (index - 1) * step;

            Rect newRect = new Rect(source);
            newRect.x += startX;
            newRect.width = singleWidth;

            return newRect;
        }

        /// <summary>
        /// Returns a new rect defined as 1 of X rects evenly sized vertically within a larger rect.
        /// </summary>
        public static Rect SplitHeight(this Rect source, int index, int count, float spacing = 0f)
        {
            if (source.height == 0)
                return source;

            float totalSpacing = spacing * (count - 1);
            float workableHeight = source.height - totalSpacing;
            float singleHeight = workableHeight / count;
            float step = singleHeight + spacing;
            float startY = (index - 1) * step;

            Rect newRect = new Rect(source);
            newRect.y += startY;
            newRect.height = singleHeight;

            return newRect;
        }
    }
}
