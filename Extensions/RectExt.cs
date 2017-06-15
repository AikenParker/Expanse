using UnityEngine;
using Rect = UnityEngine.Rect;

namespace Expanse.Extensions
{
    /// <summary>
    /// A collection of UnityEngine.Rect related extension methods.
    /// </summary>
    public static class RectExt
    {
        /// <summary>
        /// RectMode describes the anchor of the rect when modifying it.
        /// </summary>
        public enum RectMode
        {
            /// <summary>
            /// Anchor is set to the top-left of the rect.
            /// </summary>
            TopLeft,

            /// <summary>
            /// Anchor is set to the center of the rect.
            /// </summary>
            Center
        }

        /// <summary>
        /// Creates a new rect with added width and height.
        /// </summary>
        /// <param name="source">Source rectangle value.</param>
        /// <param name="addWidth">Width to be added to the rectangle.</param>
        /// <param name="addHeight">Height to be added to the rectangle.</param>
        /// <param name="rectMode">Anchor position of the rect when modifying.</param>
        /// <returns>Returns a new rect with added width and height.</returns>
        public static Rect AddSize(this Rect source, float addWidth, float addHeight, RectMode rectMode = RectMode.TopLeft)
        {
            switch (rectMode)
            {
                case RectMode.TopLeft:
                    source.width += addWidth;
                    source.height += addHeight;
                    break;

                case RectMode.Center:
                    source.width += addWidth;
                    source.height += addHeight;
                    source.x -= addWidth / 2f;
                    source.y -= addHeight / 2f;
                    break;
            }

            return source;
        }

        /// <summary>
        /// Creates a new rect with set width and height.
        /// </summary>
        /// <param name="source">Source rectangle value.</param>
        /// <param name="width">Width to set to the rectangle.</param>
        /// <param name="height">Height to set to the rectangle.</param>
        /// <param name="rectMode">Anchor position of the rect when modifying.</param>
        /// <returns>Returns a new rect with set width and height.</returns>
        public static Rect WithSize(this Rect source, float width, float height, RectMode rectMode = RectMode.TopLeft)
        {
            switch (rectMode)
            {
                case RectMode.TopLeft:
                    source.width = width;
                    source.height = height;
                    break;

                case RectMode.Center:
                    float widthDiff = source.width - width;
                    float heightDiff = source.height - height;
                    source.width += widthDiff;
                    source.height += heightDiff;
                    source.x -= widthDiff / 2f;
                    source.y -= heightDiff / 2f;
                    break;
            }

            return source;
        }

        /// <summary>
        /// Creates a new rect with added position.
        /// </summary>
        /// <param name="source">Source rectangle value.</param>
        /// <param name="addPosition">Position to be added to the rect.</param>
        /// <returns>Returns a new rect with added positon.</returns>
        public static Rect AddPosition(this Rect source, Vector2 addPosition)
        {
            source.position += addPosition;
            return source;
        }

        /// <summary>
        /// Creates a new rect with set position.
        /// </summary>
        /// <param name="source">Source rectangle value.</param>
        /// <param name="newPosition">Position to be set on the rect.</param>
        /// <returns>Returns a new rect with new position set.</returns>
        public static Rect WithPosition(this Rect source, Vector2 newPosition)
        {
            source.position = newPosition;
            return source;
        }

        /// <summary>
        /// Creates a rect with a scaled size.
        /// </summary>
        /// <param name="source">Source rectangle value.</param>
        /// <param name="scale">Amount to scale rect size by.</param>
        /// <param name="rectMode">Anchor position of the rect when modifying.</param>
        /// <returns>Returns a new rect with scaled size.</returns>
        public static Rect Scale(this Rect source, float scale, RectMode rectMode = RectMode.TopLeft)
        {
            return Scale(source, new Vector2(scale, scale), rectMode);
        }

        /// <summary>
        /// Creates a rect with a scaled size.
        /// </summary>
        /// <param name="source">Source rectangle value.</param>
        /// <param name="scale">Amount to scale rect size by.</param>
        /// <param name="rectMode">Anchor position of the rect when modifying.</param>
        /// <returns>Returns a new rect with scaled size.</returns>
        public static Rect Scale(this Rect source, Vector2 scale, RectMode rectMode = RectMode.TopLeft)
        {
            switch (rectMode)
            {
                case RectMode.TopLeft:
                    source.width *= scale.x;
                    source.height *= scale.y;
                    break;

                case RectMode.Center:
                    float widthDiff = source.width - (source.width * scale.x);
                    float heightDiff = source.height - (source.height * scale.x);
                    source.width += widthDiff;
                    source.height += heightDiff;
                    source.x -= widthDiff / 2f;
                    source.y -= heightDiff / 2f;
                    break;
            }

            return source;
        }

        /// <summary>
        /// Returns a new rect defined as 1 of X rects evenly sized horizontally within a larger rect.
        /// </summary>
        /// <param name="source">Source rectangle value.</param>
        /// <param name="index">Index of the sub rectangle split from the source rectangle to return.</param>
        /// <param name="count">Amount of sub rectangles to split the source rectangle into.</param>
        /// <param name="spacing">Horizontal space between each sub rectangle.</param>
        /// <returns>Returns a sub rectangle of index when source is split into count amount of sub rectangles.</returns>
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
        /// <param name="source">Source rectangle value.</param>
        /// <param name="index">Index of the sub rectangle split from the source rectangle to return.</param>
        /// <param name="count">Amount of sub rectangles to split the source rectangle into.</param>
        /// <param name="spacing">Vertical space between each sub rectangle.</param>
        /// <returns>Returns a sub rectangle of index when source is split into count amount of sub rectangles.</returns>
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
