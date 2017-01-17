using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using Expanse;
using UnityEditorInternal;
using System.Collections;

namespace Expanse
{
    /// <summary>
    /// Generic editor reorderable list wrapper.
    /// </summary>
    public class ReorderableList<T> : ReorderableList
    {
        public string DisplayName { get; set; }

        public ReorderableList(IList<T> source, string displayName) : base((IList)source, typeof(T))
        {
            DisplayName = displayName;
            this.displayAdd = this.displayRemove = this.draggable = true;

            SetDefaults();
        }

        public ReorderableList(IList<T> source, string displayName, bool draggable, bool displayAdd, bool displayRemove) : base((IList)source, typeof(T))
        {
            DisplayName = displayName;
            this.displayAdd = displayAdd;
            this.displayRemove = displayRemove;
            this.draggable = draggable;

            SetDefaults();
        }

        private void SetDefaults()
        {
            SetDefaultHeader();
        }

        private void DoDefaultHeader(Rect rect)
        {
            GUI.Label(rect, DisplayName);
        }

        public void SetDefaultHeader()
        {
            this.drawHeaderCallback = DoDefaultHeader;
        }

        public void Draw(Rect? rect = null)
        {
            if (rect.HasValue)
                DoList(rect.Value);
            else
                DoLayoutList();
        }

        public T this[int index]
        {
            get
            {
                return (T)this.list[index];
            }
            set
            {
                this.list[index] = value;
            }
        }
    }
}
