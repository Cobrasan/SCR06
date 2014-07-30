using System;
using System.Threading;
using System.Diagnostics;

namespace Alchemist
{
    public partial class DataController : IDisposable
    {
        private TraceSource ts = new TraceSource("DataControllerThread");

        /// <summary>
        /// DataControllerのスレッド
        /// </summary>
        private void dataControllerThread()
        {
            ts.TraceInformation("DataControllerThreadが開始しました。");

            try
            {
                while (true)
                {
                    Thread.Sleep(500);
                }
            }
            catch (ThreadAbortException)
            {
                /* 何もしない */
            }

            ts.TraceInformation("DataControllerThreadが終了しました。");
        }
    }
}
