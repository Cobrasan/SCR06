using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Alchemist
{

    public delegate void dataEneterDelegate(TenKeyData td);

    public struct TenKeyData
    {
        public object obj;
        public double val;
        public int workid;
        public int workidtype;
        public int itemtype;
        public int columindex;
        public int rowindex;
        public object value;
    }

    class TenkeyControl
    {
        public event dataEneterDelegate dataEneterEvent;

        public TenKeyData tenKeyData;

        // テンキー表示
        public void tenkeyFormShow()
        {
            tenKeyfrm tenKeyForm = new tenKeyfrm();
            tenKeyForm.enterKeyEvent += new enterKeyDelegate(enterKeyEvent);
            tenKeyForm.val = tenKeyData.val;
            tenKeyForm.ShowDialog();
            tenKeyForm.Dispose();
        }

        // テンキーからエンター入力イベント
        public void enterKeyEvent(double value)
        {
            tenKeyData.val = value;
            dataEneterEvent(tenKeyData);
        }

    }
}
