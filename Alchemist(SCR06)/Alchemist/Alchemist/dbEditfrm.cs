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

            SCR06DBController db = Program.SCR06DB;
            db.dbGetColorComboBox(ref comboBox6);
            db.dbGetColorComboBox(ref comboBox8);
            db.dbGetColorComboBox(ref comboBox9);
            db.dbGetColorComboBox(ref comboBox10);
            db.dbGetColorComboBox(ref comboBox11);
            db.dbGetColorComboBox(ref comboBox12);
            db.dbGetColorComboBox(ref comboBox13);
            db.dbGetColorComboBox(ref comboBox14);
            db.dbGetColorComboBox(ref comboBox15);
            db.dbGetColorComboBox(ref comboBox16);
            db.dbGetColorComboBox(ref comboBox17);
            db.dbGetColorComboBox(ref comboBox18);
            db.dbGetColorComboBox(ref comboBox19);
            db.dbGetColorComboBox(ref comboBox20);
            db.dbGetColorComboBox(ref comboBox21);
            db.dbGetColorComboBox(ref comboBox22);
            db.dbGetColorComboBox(ref comboBox23);
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

        //電線種マスター一覧
        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            SCR06DBController db = Program.SCR06DB;
            ComboBox cbx = (ComboBox)sender;
            cbx.Items.Clear();
            db.dbGetWireTypeComboBox(ref cbx);
        }

        //電線サイズマスター一覧
        private void comboBox2_DropDown(object sender, EventArgs e)
        {
            SCR06DBController db = Program.SCR06DB;
            ComboBox cbx = (ComboBox)sender;
            cbx.Items.Clear();
            db.dbGetWireSizeComboBox(ref cbx);
        }

        //電線色マスター一覧
        private void comboBox3_DropDown(object sender, EventArgs e)
        {
            SCR06DBController db = Program.SCR06DB;
            comboBox3.Items.Clear();
            db.dbGetColorComboBox(ref comboBox3);
        }

        //電線色番号マスター一覧
        private void comboBox4_DropDown(object sender, EventArgs e)
        {
            SCR06DBController db = Program.SCR06DB;
            comboBox4.Items.Clear();
            db.dbGetColorNoComboBox(ref comboBox4);
        }

        //電線詳細マスターから使用する電線コード一覧
        private void comboBox5_DropDown(object sender, EventArgs e)
        {
            SCR06DBController db = Program.SCR06DB;
            comboBox5.Items.Clear();
            db.dbGetWireDetailComboBox(ref comboBox5);
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
            string gname = "";
            string bname = "";
            
            if (comboBox24.Text == "" || comboBox25.Text == "") return;
            SCR06DBController db = Program.SCR06DB;
            db.dbGetGuideName(comboBox24.Text, comboBox25.Text, ref gname);
            db.dbGetBladeName(comboBox24.Text, comboBox25.Text, ref bname);

            textBox6.Text = gname;
            textBox7.Text = bname;
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
            m.C_Col2_1 = comboBox10.Text;
            m.C_Col2_2 = comboBox11.Text;
            m.C_Col3_1 = comboBox12.Text;
            m.C_Col3_2 = comboBox13.Text;
            m.C_Col4_1 = comboBox14.Text;
            m.C_Col4_2 = comboBox15.Text;
            m.C_Col5_1 = comboBox16.Text;
            m.C_Col5_2 = comboBox17.Text;
            m.C_Col6_1 = comboBox18.Text;
            m.C_Col6_2 = comboBox19.Text;
            m.C_Col7_1 = comboBox20.Text;
            m.C_Col7_2 = comboBox21.Text;
            m.C_Col8_1 = comboBox22.Text;
            m.C_Col8_2 = comboBox23.Text;

            msgQueryResult(db.dbUpdateWireDetail(m));
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


    }
}
