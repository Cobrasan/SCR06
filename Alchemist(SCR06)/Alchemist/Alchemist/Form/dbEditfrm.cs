using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Alchemist
{
    public partial class dbEditfrm : Form
    {
        public dbEditfrm()
        {
            InitializeComponent();

            //電線種選択イベント
            comboBox24.DropDown += comboBox1_DropDown;
            comboBox30.DropDown += comboBox1_DropDown;

            //電線サイズ選択イベント
            comboBox25.DropDown += comboBox2_DropDown;
            comboBox31.DropDown += comboBox2_DropDown;

            //電線色選択イベント
            comboBox6.DropDown += comboBox3_DropDown;
            comboBox8.DropDown += comboBox3_DropDown;
            comboBox9.DropDown += comboBox3_DropDown;
            comboBox10.DropDown += comboBox3_DropDown;
            comboBox11.DropDown += comboBox3_DropDown;
            comboBox12.DropDown += comboBox3_DropDown;
            comboBox13.DropDown += comboBox3_DropDown;
            comboBox14.DropDown += comboBox3_DropDown;
            comboBox15.DropDown += comboBox3_DropDown;
            comboBox16.DropDown += comboBox3_DropDown;
            comboBox17.DropDown += comboBox3_DropDown;
            comboBox18.DropDown += comboBox3_DropDown;
            comboBox19.DropDown += comboBox3_DropDown;
            comboBox20.DropDown += comboBox3_DropDown;
            comboBox21.DropDown += comboBox3_DropDown;
            comboBox22.DropDown += comboBox3_DropDown;
            comboBox23.DropDown += comboBox3_DropDown;
            comboBox32.DropDown += comboBox3_DropDown;

            //電線色番号イベント
            comboBox33.DropDown += comboBox4_DropDown;

            //電線コード作成イベント
            comboBox30.SelectedIndexChanged += createWireCode;
            comboBox31.SelectedIndexChanged += createWireCode;
            comboBox32.SelectedIndexChanged += createWireCode;
            comboBox33.SelectedIndexChanged += createWireCode;
        }

        //結果表示
        private void msgQueryResult(int msg)
        {
            if (msg == SystemConstants.ERR_MASTER_UPDATE)
                Utility.ShowErrorMsg(SystemConstants.SYSTEM_MSG056);
            else
                Utility.ShowInfoMsg(SystemConstants.SYSTEM_MSG055);
        }

        //電線詳細更新可能確認
        private int chkQueryWireDetail()
        {
            int cnum;

            if (comboBox5.Text == "" || comboBox6.Text == "" || comboBox7.Text == "") return 0;
            if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG054) == false) return 0;

            try
            {
                cnum = Int16.Parse(comboBox7.Text);
            }
            catch
            {
                return 0;
            }

            if (cnum >= 1)
            {
                if (comboBox8.Text == "" || comboBox9.Text == "") return 0;
            }
            if (cnum >= 2)
            {
                if (comboBox10.Text == "" || comboBox11.Text == "") return 0;
            }
            if (cnum >= 3)
            {
                if (comboBox12.Text == "" || comboBox13.Text == "") return 0;
            }
            if (cnum >= 4)
            {
                if (comboBox14.Text == "" || comboBox15.Text == "") return 0;
            }
            if (cnum >= 5)
            {
                if (comboBox16.Text == "" || comboBox17.Text == "") return 0;
            }
            if (cnum >= 6)
            {
                if (comboBox17.Text == "" || comboBox18.Text == "") return 0;
            }
            if (cnum >= 7)
            {
                if (comboBox19.Text == "" || comboBox20.Text == "") return 0;
            }
            if (cnum == 8)
            {
                if (comboBox21.Text == "" || comboBox22.Text == "") return 0;
            }

            return cnum;
        }

        //部署名取得
        private string getSectionName(int SectionCode)
        {
            string section = "";
            SCR06DBController db = Program.SCR06DB;
            db.dbGetSectionName(SectionCode, ref section);
            return section;
        }

        //電線コード作成イベント              
        private void createWireCode(object sender, EventArgs e)
        {
            SCR06DBController db = Program.SCR06DB;
            string wirecode = db.dbGetWireCode(comboBox30.Text, comboBox31.Text, comboBox32.Text, comboBox33.Text);
            if (wirecode != "")
            {
                comboBox5.Text = wirecode;
            }
        }

        //電線種マスター一覧
        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            SCR06DBController db = Program.SCR06DB;
            ComboBox cbx = (ComboBox)sender;
            cbx.Items.Clear();
            db.dbGetItemWireTypeComboBox(ref cbx);
        }

        //電線サイズマスター一覧
        private void comboBox2_DropDown(object sender, EventArgs e)
        {
            SCR06DBController db = Program.SCR06DB;
            ComboBox cbx = (ComboBox)sender;
            cbx.Items.Clear();
            db.dbGetItemWireSizeComboBox(ref cbx);
        }

        //電線色マスター一覧
        private void comboBox3_DropDown(object sender, EventArgs e)
        {
            SCR06DBController db = Program.SCR06DB;
            ComboBox cbx = (ComboBox)sender;
            cbx.Items.Clear();
            db.dbGetItemColorCharComboBox(ref cbx);
        }

        //電線色番号マスター一覧
        private void comboBox4_DropDown(object sender, EventArgs e)
        {
            SCR06DBController db = Program.SCR06DB;
            ComboBox cbx = (ComboBox)sender;
            cbx.Items.Clear();
            db.dbGetItemColorNoComboBox(ref cbx);
        }

        //電線詳細マスターから使用する電線コード一覧
        private void comboBox5_DropDown(object sender, EventArgs e)
        {
            SCR06DBController db = Program.SCR06DB;
            comboBox5.Items.Clear();
            db.dbGetItemWireDetailComboBox(ref comboBox5);
        }

        //品質記録項目マスターから使用する品質記録コード一覧
        private void comboBox26_DropDown(object sender, EventArgs e)
        {
            SCR06DBController db = Program.SCR06DB;
            comboBox26.Items.Clear();
            db.dbGetItemQualityCodeComboBox(ref comboBox26);
        }

        //作業者コード一覧
        private void comboBox27_DropDown(object sender, EventArgs e)
        {
            SCR06DBController db = Program.SCR06DB;
            comboBox27.Items.Clear();
            db.dbGetItemOperatorCodeComboBox(ref comboBox27);
        }

        //部署マスター一覧
        private void comboBox28_DropDown(object sender, EventArgs e)
        {
            SCR06DBController db = Program.SCR06DB;
            ComboBox cbx = (ComboBox)sender;
            cbx.Items.Clear();
            db.dbGetItemSectionCodeComboBox(ref cbx);
        }

        //電線種マスター選択
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SCR06DBController db = Program.SCR06DB;
            textBox1.Text = db.dbGetWireTypeCode(comboBox1.Text);
        }

        //電線サイズマスター選択
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            SCR06DBController db = Program.SCR06DB;
            textBox2.Text = db.dbGetWireSizeCode(comboBox2.Text);
        }

        //電線色マスター選択
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            SCR06DBController db = Program.SCR06DB;
            textBox3.Text = db.dbGetColorCode(comboBox3.Text);
            textBox4.Text = db.dbGetColorRGBCode(comboBox3.Text);
        }

        //電線色番号マスター選択
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            SCR06DBController db = Program.SCR06DB;
            textBox5.Text = db.dbGetColorNoCode(comboBox4.Text);
        }

        //電線詳細マスター選択
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            SCR06DBController db = Program.SCR06DB;

            M_WireDetail wireinfo = new M_WireDetail();
            db.dbGetWireInfomation(comboBox5.Text, ref wireinfo);

            comboBox6.Text = wireinfo.Wire_Color;
            comboBox7.Text = wireinfo.Core_Num.ToString();
            comboBox8.Text = wireinfo.C_Col1_1;
            comboBox9.Text = wireinfo.C_Col1_2;
            comboBox10.Text = wireinfo.C_Col2_1;
            comboBox11.Text = wireinfo.C_Col2_2;
            comboBox12.Text = wireinfo.C_Col3_1;
            comboBox13.Text = wireinfo.C_Col3_2;
            comboBox14.Text = wireinfo.C_Col4_1;
            comboBox15.Text = wireinfo.C_Col4_2;
            comboBox16.Text = wireinfo.C_Col5_1;
            comboBox17.Text = wireinfo.C_Col5_2;
            comboBox18.Text = wireinfo.C_Col6_1;
            comboBox19.Text = wireinfo.C_Col6_2;
            comboBox20.Text = wireinfo.C_Col7_1;
            comboBox21.Text = wireinfo.C_Col7_2;
            comboBox22.Text = wireinfo.C_Col8_1;
            comboBox23.Text = wireinfo.C_Col8_2;
        }

        //ガイド・ブレードマスター選択
        private void comboBox24_SelectedIndexChanged(object sender, EventArgs e)
        {
            string gname1 = "";
            string gname2 = "";
            string bname = "";
            
            if (comboBox24.Text == "" || comboBox25.Text == "") return;
            SCR06DBController db = Program.SCR06DB;
            db.dbGetGuideName(comboBox24.Text, comboBox25.Text, ref gname1, ref gname2);
            db.dbGetBladeName(comboBox24.Text, comboBox25.Text, ref bname);

            textBox6.Text = gname1;
            textBox12.Text = gname2;
            textBox7.Text = bname;
        }

        //品質記録項目マスター選択
        private void comboBox26_SelectedIndexChanged(object sender, EventArgs e)
        {
            int num;
            try
            {
                num = Int16.Parse(comboBox26.Text);
            }
            catch
            {
                return;
            }
            SCR06DBController db = Program.SCR06DB;
            textBox8.Text = db.dbGetQualityItem(num);                      
        }

        //作業者マスター選択
        private void comboBox27_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opname = "";
            
            SCR06DBController db = Program.SCR06DB;
            M_Operator op = new M_Operator();

            db.dbGetOperatorName(comboBox27.Text, ref opname);
            textBox9.Text = opname;
            db.dbGetOperatorGroup(comboBox27.Text, ref op);
            comboBox28.Text = op.OperatorGroup.ToString();
            try
            {
                textBox10.Text = getSectionName(Int32.Parse(comboBox28.Text));
            }
            catch 
            {
                comboBox28.Text = "";
                textBox10.Text = "";
            }
        }

        //部署コード選択
        private void comboBox28_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                textBox10.Text = getSectionName(Int32.Parse(comboBox28.Text));
            }
            catch 
            {
                comboBox28.Text = "";
                textBox10.Text = "";
            }
        }

        //部署コード選択
        private void comboBox29_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                textBox11.Text = getSectionName(Int32.Parse(comboBox29.Text));
            }
            catch
            {
                comboBox29.Text = "";
                textBox11.Text = "";
            }
        }

        //電線種マスター登録
        private void button1_Click(object sender, EventArgs e)
        {
            if(comboBox1.Text == "") return;
            if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG054) == false) return; 

            SCR06DBController db = Program.SCR06DB;
            M_WireType m = new M_WireType();
            m.WireType = comboBox1.Text;
            m.WireTypeCode = textBox1.Text;

            msgQueryResult(db.dbUpdateWireType(m));
        }

        //電線サイズマスター登録
        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox2.Text == "") return;
            if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG054) == false) return;

            SCR06DBController db = Program.SCR06DB;
            M_WireSize m = new M_WireSize();
            m.WireSize = comboBox2.Text;
            m.WireSizeCode = textBox2.Text;

            msgQueryResult(db.dbUpdateWireSize(m));
        }

        //電線色マスター登録
        private void button6_Click(object sender, EventArgs e)
        {
            if (comboBox3.Text == "") return;
            if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG054) == false) return;

            SCR06DBController db = Program.SCR06DB;
            M_Color m = new M_Color();
            m.ColorChar = comboBox3.Text;
            m.ColorCode = textBox3.Text;
            m.ColorRGBCode = textBox4.Text;

            msgQueryResult(db.dbUpdateColor(m));
        }

        //電線色番号マスター登録
        private void button8_Click(object sender, EventArgs e)
        {
            if (comboBox4.Text == "") return;
            if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG054) == false) return;

            SCR06DBController db = Program.SCR06DB;
            M_ColorNo m = new M_ColorNo();
            m.ColorNo = comboBox4.Text;
            m.ColorNoCode = textBox5.Text;

            msgQueryResult(db.dbUpdateColorNo(m));
        }

        //電線詳細マスター登録
        private void button10_Click(object sender, EventArgs e)
        {            
            int num = chkQueryWireDetail();
            if (num == 0) return;

            SCR06DBController db = Program.SCR06DB;
            M_WireDetail m = new M_WireDetail();
            m.Wire_Code = comboBox5.Text;
            m.Wire_Color = comboBox6.Text;
            m.Core_Num = num;
            m.C_Col1_1 = comboBox8.Text;
            m.C_Col1_2 = comboBox9.Text;
            if (num > 1)
            {
                m.C_Col2_1 = comboBox10.Text;
                m.C_Col2_2 = comboBox11.Text;
            }
            if (num > 2)
            {
                m.C_Col3_1 = comboBox12.Text;
                m.C_Col3_2 = comboBox13.Text;
            }
            if (num > 3)
            {
                m.C_Col4_1 = comboBox14.Text;
                m.C_Col4_2 = comboBox15.Text;
            }
            if (num > 4)
            {
                m.C_Col5_1 = comboBox16.Text;
                m.C_Col5_2 = comboBox17.Text;
            }
            if (num > 5)
            {
                m.C_Col6_1 = comboBox18.Text;
                m.C_Col6_2 = comboBox19.Text;
            }
            if (num > 6)
            {
                m.C_Col7_1 = comboBox20.Text;
                m.C_Col7_2 = comboBox21.Text;
            }
            if (num > 7)
            {
                m.C_Col8_1 = comboBox22.Text;
                m.C_Col8_2 = comboBox23.Text;
            }

            msgQueryResult(db.dbUpdateWireDetail(m));
        }

        //ガイド・ブレード登録
        private void button12_Click(object sender, EventArgs e)
        {
            if (comboBox24.Text == "" || comboBox25.Text == "") return;
            if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG054) == false) return;

            SCR06DBController db = Program.SCR06DB;
            M_GuideBlade m = new M_GuideBlade();
            m.WireType = comboBox24.Text;
            m.WireSize = comboBox25.Text;
            m.Guide1 = textBox6.Text;
            m.Guide2 = textBox12.Text;
            m.Blade = textBox7.Text;

            msgQueryResult(db.dbUpdateGuideBlade(m));
        }

        //品質記録項目マスター登録
        private void button14_Click(object sender, EventArgs e)
        {
            if (comboBox26.Text == "") return;
            if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG054) == false) return;
            
            int num;
            try
            {
                num = Int16.Parse(comboBox26.Text);
            }
            catch
            {
                return;
            }
   
            SCR06DBController db = Program.SCR06DB;
            M_Quality m = new M_Quality();
            m.QualityCode = num;
            m.QualityItem = textBox8.Text;

            msgQueryResult(db.dbUpdateQuality(m));
        }

        //作業者マスター登録
        private void button16_Click(object sender, EventArgs e)
        {
            if (comboBox27.Text == "") return;
            if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG054) == false) return;            

            SCR06DBController db = Program.SCR06DB;
            M_Operator m = new M_Operator();
            m.OperatorCode = comboBox27.Text;
            m.OperatorName = textBox9.Text;

            try
            {
                if (comboBox28.Text != "")
                    m.OperatorGroup = Int32.Parse(comboBox28.Text);
            }
            catch
            {
                m.OperatorGroup = null;
            }

            msgQueryResult(db.dbUpdateOperator(m));
        }

        //部署コード登録
        private void button18_Click(object sender, EventArgs e)
        {
            if (comboBox29.Text == "") return;
            if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG054) == false) return;

            SCR06DBController db = Program.SCR06DB;
            M_Section m = new M_Section();

            try
            {
                if (comboBox29.Text != "")
                    m.SectionCode = Int32.Parse(comboBox29.Text);                
            }
            catch
            {
                return;
            }
            m.SectionName = textBox11.Text;

            msgQueryResult(db.dbUpdateSection(m));
        }

        //電線種マスター削除
        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "") return;
            if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG054) == false) return; 
            
            SCR06DBController db = Program.SCR06DB;
            M_WireType m = new M_WireType();
            m.WireType = comboBox1.Text;

            msgQueryResult(db.dbDeleteWireType(m));
        }

        //電線サイズマスター削除
        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox2.Text == "") return;
            if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG054) == false) return;

            SCR06DBController db = Program.SCR06DB;
            M_WireSize m = new M_WireSize();
            m.WireSize = comboBox2.Text;

            msgQueryResult(db.dbDeleteWireSize(m));
        }

        //電線色マスター削除
        private void button5_Click(object sender, EventArgs e)
        {
            if (comboBox3.Text == "") return;
            if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG054) == false) return;

            SCR06DBController db = Program.SCR06DB;
            M_Color m = new M_Color();
            m.ColorChar = comboBox3.Text;

            msgQueryResult(db.dbDeleteColor(m));
        }

        //電線色番号マスター削除
        private void button7_Click(object sender, EventArgs e)
        {
            if (comboBox4.Text == "") return;
            if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG054) == false) return;

            SCR06DBController db = Program.SCR06DB;
            M_ColorNo m = new M_ColorNo();
            m.ColorNo = comboBox4.Text;

            msgQueryResult(db.dbDeleteColorNo(m));
        }

        //電線詳細マスター削除
        private void button9_Click(object sender, EventArgs e)
        {
            int num = chkQueryWireDetail();
            if (num == 0) return;

            SCR06DBController db = Program.SCR06DB;
            M_WireDetail m = new M_WireDetail();
            m.Wire_Code = comboBox5.Text;

            msgQueryResult(db.dbDeleteWireDetail(m));
        }

        //品質記録項目マスター削除
        private void button13_Click(object sender, EventArgs e)
        {
            int num;
            try
            {
                num = Int16.Parse(comboBox26.Text);
            }
            catch
            {
                return;
            }

            if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG054) == false) return;

            SCR06DBController db = Program.SCR06DB;
            M_Quality m = new M_Quality();
            m.QualityCode = num;

            msgQueryResult(db.dbDeleteQuality(m));
        }

        //作業者マスター削除
        private void button15_Click(object sender, EventArgs e)
        {
            if (comboBox27.Text == "") return;
            if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG054) == false) return;

            SCR06DBController db = Program.SCR06DB;
            M_Operator m = new M_Operator();
            m.OperatorCode = comboBox27.Text;

            msgQueryResult(db.dbDeleteOperator(m));
        }

        //部署マスター削除
        private void button17_Click(object sender, EventArgs e)
        {
            int num;
            try
            {
                num = Int16.Parse(comboBox29.Text);
            }
            catch
            {
                return;
            }

            if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG054) == false) return;

            SCR06DBController db = Program.SCR06DB;
            M_Section m = new M_Section();
            m.SectionCode = num;

            msgQueryResult(db.dbDeleteSection(m));
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (comboBox24.Text == "" || comboBox25.Text == "") return;
            if (Utility.ShowConfirmMsg(SystemConstants.SYSTEM_MSG054) == false) return;

            SCR06DBController db = Program.SCR06DB;
            M_GuideBlade m = new M_GuideBlade();
            m.WireType = comboBox24.Text;
            m.WireSize = comboBox25.Text;

            msgQueryResult(db.dbDeleteGuideBlade(m));
        }

    }
}
