using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NineLineNotation
{
    public partial class TestCanvasForm : Form, ICanvasOwner
    {
        CanvasCtrl m_canvas;
        DataModel m_data;
        public TestCanvasForm()
        {
            InitializeComponent();

            m_data = new DataModel();



            this.m_canvas = new CanvasCtrl(this, m_data);
            this.SuspendLayout();
            // 
            // canvasCtrl1
            // 
            this.m_canvas.BackColor = System.Drawing.Color.Silver;
            this.m_canvas.Location = new System.Drawing.Point(0, 0);
            this.m_canvas.Name = "canvasCtrl1";
            this.m_canvas.Size = new System.Drawing.Size(740, 848);
            this.m_canvas.TabIndex = 0;

            Controls.Add(m_canvas);
            this.ResumeLayout(false);

        }
    }
}
