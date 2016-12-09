using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using Expanse;

namespace Expanse
{
    public static class RectUtil
    {
        public static Rect AddSize(this Rect source, float addWidth, float addHeight)
        {
            Rect newRect = new Rect(source);
            newRect.width += addWidth;
            newRect.height += addHeight;
            return newRect;
        }

        public static Rect SetSize(this Rect source, float width, float height)
        {
            Rect newRect = new Rect(source);
            newRect.width = width;
            newRect.height = height;
            return newRect;
        }

        public static Rect AddPosition(this Rect source, float addX, float addY)
        {
            Rect newRect = new Rect(source);
            newRect.x += addX;
            newRect.y += addY;
            return newRect;
        }

        public static Rect SetPosition(this Rect source, float x, float y)
        {
            Rect newRect = new Rect(source);
            newRect.x = x;
            newRect.y = y;
            return newRect;
        }

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
            newRect.width = step;

            return newRect;
        }
    }
}