using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Alchemist
{
	public class CustomTextBox : TextBox
	{
		// コンストラクタ
		public CustomTextBox()
		{
			// デフォルト設定
			NormalColor = Color.Black;		/* 背景＝黒 */
			FocusColor = Color.Blue;		/* フォーカスした時の色=青 */
            FocusStringSelect = true;      /* フォーカス時文字を選択するか */
			AllowSign = false;				/* 符号は許可しない */
			AllowDot = true;				/* ドットは許可する */
			AllowHex = false;				/* 16進文字列を許可 */
            AllowAll = false;				/* 16進文字列を許可 */
		}

        //マウスキーダウンを受け取った時
        protected override void OnMouseDown(MouseEventArgs e)
        {
            //文字を選択する
            if (FocusStringSelect) base.SelectAll();

            base.OnMouseDown(e);
        }

        //Enterを受け取った時
        protected override void OnEnter(EventArgs e)
        {
            //文字を選択する
            if (FocusStringSelect) base.SelectAll();

            base.OnEnter(e);
        }

		// フォーカスを受け取った時
		protected override void OnGotFocus(EventArgs e)
		{
			BackColor = FocusColor;

			base.OnGotFocus(e);
		}

		// フォーカスを失った時の動作
		protected override void OnLostFocus(EventArgs e)
		{
			BackColor = NormalColor;

			base.OnLostFocus(e);
		}

		// キーダウンイベント
		protected override void OnKeyDown(KeyEventArgs e)
		{
			// リターンキーが押下された場合、
			// リターンキー押下イベントを実行する
			if (e.KeyCode == Keys.Return)
			{
				if (EnterKeyDown != null)
				{
					EnterKeyDown(e);
				}
			}

			base.OnKeyDown(e);
		}

		// WndProcのオーバーライド（入力の制限)
		protected override void WndProc(ref Message m)
		{
			const int WM_CHAR  = 0x0102;
			const int WM_PASTE = 0x0302;

            // 入力文字の制限なし
            if (AllowAll)
            {
                base.WndProc(ref m);
            }
			// ペーストメッセージ
			else if (m.Msg == WM_PASTE)
			{
				IDataObject iData = Clipboard.GetDataObject();
				//文字列がクリップボードにあるか
				if (iData != null && iData.GetDataPresent(DataFormats.Text))
				{
					string clipStr = (string)iData.GetData(DataFormats.Text);
					//クリップボードの文字列が数字か調べる
					if (!System.Text.RegularExpressions.Regex.IsMatch(
						clipStr,
						@"^[0-9]+$"))
						return;
				}
			}
			// キー入力メッセージ
			else if (m.Msg == WM_CHAR)
			{
				int c = m.WParam.ToInt32();

				// 数字の場合は、無条件に許可する
				if (((c >= '0') && (c <= '9')) || (c == '\b'))
				{
					base.WndProc(ref m);
				}
				// 16進文字列が許可されていればA-Fを許可
				else if (AllowHex && ((c >= 'A') && (c <= 'F'))) {
					base.WndProc(ref m);
				}
				// ドットの場合
				else if (AllowDot && (c == '.')) {
					base.WndProc(ref m);
				}
				else if (AllowSign && (c == '-')) {
					base.WndProc(ref m);
				}
				
				// フィルタリングする場合は、何もしない
				else
				{
					m.Result = new IntPtr(1);
				}
			}
			else
			{
				base.WndProc(ref m);
			}
		}

		/* 以下プロパティ */

		// 通常カラー
        /// <summary>
        /// 通常時の背景色
        /// </summary>
		public Color NormalColor
		{
			get; set;
		}

        /// <summary>
        /// フォーカス時の文字選択
        /// </summary>
        public bool FocusStringSelect
        {
            get; set;
        }

		// フォーカスしたときの色
        /// <summary>
        /// フォーカスされた時の背景色
        /// </summary>
		public Color FocusColor 
		{
			get; set;
		}

		// 符号文字を許すか？
        /// <summary>
        /// 符号文字の許可不許可
        /// </summary>
		public bool AllowSign {
			get; set;
		}

		// ドット文字を許すか？
        /// <summary>
        /// ドット文字の許可不許可
        /// </summary>
		public bool AllowDot {
			get; set;
		}

		// 16進文字列を許可
        /// <summary>
        /// 16進数文字列の許可不許可
        /// </summary>
		public bool AllowHex {
			get; set;
		}

        // 入力文字の制限なし
        /// <summary>
        /// 入力文字の制限の有無
        /// </summary>
        public bool AllowAll
        {
            get; set;
        }

		/* イベントハンドラ */
		public delegate void EnterKeyEventHandler(System.EventArgs e);
		public event EnterKeyEventHandler EnterKeyDown;
	}

    public class CustomWireSelectorPanel : Panel
    {
        //コンストラクタ
        public CustomWireSelectorPanel()
        {
            //デフォルト設定
            NormalColor = Color.Black;
            FocusColor = Color.Blue;
        }

        protected override void OnClick(EventArgs e)
        {
            

            base.OnClick(e);
        }


        /* プロパティ */

        //通常時の色
        public Color NormalColor
        {
            get; set;
        }

        //フォーカスした時の色
        public Color FocusColor
        {
            get; set;
        }

    }
}
