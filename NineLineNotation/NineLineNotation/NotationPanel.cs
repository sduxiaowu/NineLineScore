using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace NineLineNotation
{
    public partial class NotationPanel : UserControl, ICanvasOwner
    {
        public DataModel m_data;
        public NotationPanel()
        {
            m_data = new DataModel();
            InitializeComponent();
            BorderStyle = BorderStyle.FixedSingle;
        }
    }
}
