using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AutoSizeFont
{
    public partial class frmMain : Form
    {
        private Rectangle m_rect = new Rectangle(150, 20, 100, 25);
        private Bitmap m_image = null;
        private const int ORIGINAL_FONT_SIZE=15;
        private const string FONT_NAME = "宋体";
        private const float FONT_SIZE_CHANGE_STEP = 0.5f;
        private string m_content = string.Empty;

        private delegate void DrawTextDelegate(Graphics g,string content);
        private DrawTextDelegate m_textDrawer = null;

        public frmMain()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
        }

        private void pnlBoard_Paint(object sender, PaintEventArgs e)
        {
            using (Pen p = new Pen(Color.Red, 1))
            {
                e.Graphics.DrawRectangle(p, m_rect);
            }

            if (m_textDrawer == null)
            {
                return;
            }

            m_textDrawer(e.Graphics, m_content);
        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtContent.Text))
            {
                MessageBox.Show("绘制的内容不能为空");
                txtContent.Focus();
                return;
            }

            m_content=txtContent.Text;

            if (rbNormal.Checked)
            {
                m_textDrawer = DrawTextNormal;
            }
            else if (rbAutoFontSize.Checked)
            {
                m_textDrawer = DrawTextAutoChangeFontSize;
            }
            else
            {
                using (Graphics g = pnlBoard.CreateGraphics())
                {
                    using (Font f = new Font(FONT_NAME, ORIGINAL_FONT_SIZE))
                    {
                        SizeF contentSize = g.MeasureString(m_content, f);
                        if (m_image != null)
                        {
                            m_image.Dispose();
                        }
                        m_image = new Bitmap(Convert.ToInt32(contentSize.Width + 1), Convert.ToInt32(contentSize.Height + 1));

                        using (Graphics gImage = Graphics.FromImage(m_image))
                        {
                            gImage.DrawString(m_content, f, Brushes.Blue, new Rectangle(0, 0, m_image.Width, m_image.Height));
                        }
                    }
                }
                    
                m_textDrawer = DrawTextZoomImage;
            }

            pnlBoard.Refresh();
        }

        private void DrawTextNormal(Graphics g, string content)
        {
            using (Font f = new Font(FONT_NAME, ORIGINAL_FONT_SIZE))
            {
                g.DrawString(content, f, Brushes.Blue, m_rect);
            }
        }

        private void DrawTextAutoChangeFontSize(Graphics g, string content)
        {
            float fontSize = ORIGINAL_FONT_SIZE;
            Font f = new Font(FONT_NAME, ORIGINAL_FONT_SIZE);

            float contentWidth = g.MeasureString(content, f).Width;
            while ((contentWidth > m_rect.Width))
            {
                f.Dispose();
                fontSize-=FONT_SIZE_CHANGE_STEP;

                f = new Font(FONT_NAME, fontSize);
                contentWidth = g.MeasureString(content, f).Width;
            }

            g.DrawString(content, f, Brushes.Blue, m_rect);
            f.Dispose();            
        }

        private void DrawTextZoomImage(Graphics g, string content)
        {
            g.DrawImage(m_image, m_rect);
        }

    }
}
