using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace RailwayGIS
{
    public class PropertyGridEx : PropertyGrid
    {
        private int firstColWidth;
        public int FirstColWidth
        {
            get
            {
                return firstColWidth;
            }
            set
            {
                firstColWidth = value;
            }
        }
        protected override void OnLayout(LayoutEventArgs e)
        {
            //int width = 150;
            Control propertyGridView = this.Controls[2];
            Type propertyGridViewType = propertyGridView.GetType();
            propertyGridViewType.InvokeMember("MoveSplitterTo",
            BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
            null, propertyGridView, new object[] { firstColWidth });
            base.OnLayout(e);
        }
    }
}
