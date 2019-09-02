using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ActualizarDeudores
{
    partial class Britanico : ServiceBase
    {
        public Britanico()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            lapso.Start();
        }

        protected override void OnStop()
        {
            lapso.Stop();
        }

        private void Lapso_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

        }
    }
}
