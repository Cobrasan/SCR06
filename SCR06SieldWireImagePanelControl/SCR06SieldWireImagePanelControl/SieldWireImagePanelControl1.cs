using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.PowerPacks;

namespace SCR06SieldWireImagePanelControl
{
    public partial class SieldWireImagePanel : UserControl
    {
        public SieldWireImagePanel()
        {
            InitializeComponent();
         
            changeSieldColor();
            changeSieldWidth();
            changeStripWireImage();
        }

        public string WireName = "";    // 電線名がセットされていたら設定済み
        
        private Color sieldColor = Color.Black;     // シールド線の色        
        private int coreWireNumber = 1;             // コア電線数
        private bool stripAOn = false;              // ストリップ１有無
        private bool stripBOn = false;              // ストリップ２有無

        // シールド線色設定
        public Color SieldColor
        {
            get { return sieldColor; }
            set
            {
                sieldColor = value;
                changeSieldColor();
            }
        }

        // 芯線数の設定
        public int CoreWireNumber
        {
            get { return coreWireNumber; }
            set
            {
                coreWireNumber = value;
                changeSieldWidth();
                changeStripWireImage();
            }
        }

        // ストリップ１有無
        public bool StripAOn
        {
            get { return stripAOn; }
            set
            {
                stripAOn = value;
                changeStripWireImage();
            }
        }

        // ストリップ２有無
        public bool StripBOn
        {
            get { return stripBOn; }
            set
            {
                stripBOn = value;
                changeStripWireImage();
            }
        }

        // 電線色の設定
        public void SetCoreColor(int CoreNum, Color CoreColor, Color LineColor)
        {
            setCoreColorLineShape(CoreNum, CoreColor);
            setLineColorLineShape(CoreNum, LineColor);
        }

        // シールド線の設定
        private void changeSieldColor()
        {
            rectangleShape1.FillGradientColor = sieldColor;
        }

        // シールド線の太さ変更
        private void changeSieldWidth()
        {
            switch (coreWireNumber)
            {
                case 1:
                case 2:
                    rectangleShape1.Height = 20;
                    break;
                case 3:
                case 4:
                    rectangleShape1.Height = 25;
                    break;
                case 5:
                case 6:
                    rectangleShape1.Height = 30;
                    break;
                case 7:
                case 8:
                    rectangleShape1.Height = 40;
                    break;
            }
        }

        // ストリップ加工イメージ表示
        private void changeStripWireImage()
        {
            if (stripAOn == false && stripBOn == false)
            {
                rectangleShape1.Left = 80;
                rectangleShape1.Width = 540;
            }
            if (stripAOn == false && stripBOn == true)
            {
                rectangleShape1.Left = 80;
                rectangleShape1.Width = 390;
            }
            if (stripAOn == true && stripBOn == false)
            {
                rectangleShape1.Left = 230;
                rectangleShape1.Width = 390;
            }
            if (stripAOn == true && stripBOn == true)
            {
                rectangleShape1.Left = 230;
                rectangleShape1.Width = 240;
            }

            if (stripAOn)
            {
                visibleLineShape(1, 1, true);

                if (coreWireNumber > 1) visibleLineShape(1, 2, true);
                else visibleLineShape(1, 2, false);

                if (coreWireNumber > 2) visibleLineShape(1, 3, true);
                else visibleLineShape(1, 3, false);

                if (coreWireNumber > 3) visibleLineShape(1, 4, true);
                else visibleLineShape(1, 4, false);

                if (coreWireNumber > 4) visibleLineShape(1, 5, true);
                else visibleLineShape(1, 5, false);

                if (coreWireNumber > 5) visibleLineShape(1, 6, true);
                else visibleLineShape(1, 6, false);

                if (coreWireNumber > 6) visibleLineShape(1, 7, true);
                else visibleLineShape(1, 7, false);

                if (coreWireNumber > 7) visibleLineShape(1, 8, true);
                else visibleLineShape(1, 8, false);
            }
            else
            {
                for(int i=1; i<=8; i++)
                    visibleLineShape(1, i, false);
            }

            if (stripBOn)
            {
                visibleLineShape(2, 1, true);

                if (coreWireNumber > 1) visibleLineShape(2, 2, true);
                else visibleLineShape(2, 2, false);

                if (coreWireNumber > 2) visibleLineShape(2, 3, true);
                else visibleLineShape(2, 3, false);

                if (coreWireNumber > 3) visibleLineShape(2, 4, true);
                else visibleLineShape(2, 4, false);

                if (coreWireNumber > 4) visibleLineShape(2, 5, true);
                else visibleLineShape(2, 5, false);

                if (coreWireNumber > 5) visibleLineShape(2, 6, true);
                else visibleLineShape(2, 6, false);

                if (coreWireNumber > 6) visibleLineShape(2, 7, true);
                else visibleLineShape(2, 7, false);

                if (coreWireNumber > 7) visibleLineShape(2, 8, true);
                else visibleLineShape(2, 8, false);
            }
            else
            {
                for (int i = 1; i <= 8; i++)
                    visibleLineShape(2, i, false);
            }

        }

        // 電線イメージの表示有無
        private void visibleLineShape(int side, int num, bool vsb)
        {
            foreach (Shape shape in shapeContainer1.Shapes)
            {
                if (shape.Name == "lineShape" + side.ToString() + "_" + num.ToString() + "_1")
                    shape.Visible = vsb;
                if (shape.Name == "lineShape" + side.ToString() + "_" + num.ToString() + "_2")
                    shape.Visible = vsb;
            }
        }

        // 電線色の設定
        private void setCoreColorLineShape(int num, Color cl)
        {
            foreach (Shape shape in shapeContainer1.Shapes)
            {
                if (shape.Name == "lineShape1" + "_" + num.ToString() + "_1" ||
                    shape.Name == "lineShape2" + "_" + num.ToString() + "_1")
                    shape.BorderColor = cl;
            }
        }

        // 電線中央色の設定
        private void setLineColorLineShape(int num, Color cl)
        {
            foreach (Shape shape in shapeContainer1.Shapes)
            {
                if (shape.Name == "lineShape1" + "_" + num.ToString() + "_2" ||
                    shape.Name == "lineShape2" + "_" + num.ToString() + "_2")
                    shape.BorderColor = cl;
            }
        }
    }
}
