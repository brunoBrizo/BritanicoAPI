namespace BritanicoService
{
    partial class Britanico
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.lapsoEmail = new System.Timers.Timer();
            this.lapsoDeudores = new System.Timers.Timer();
            this.lapsoLimpiarEstudiante = new System.Timers.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.lapsoEmail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lapsoDeudores)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lapsoLimpiarEstudiante)).BeginInit();
            // 
            // lapsoEmail
            // 
            this.lapsoEmail.Enabled = true;
            this.lapsoEmail.Interval = 82800000D;
            this.lapsoEmail.Elapsed += new System.Timers.ElapsedEventHandler(this.LapsoEmail_Elapsed);
            // 
            // lapsoDeudores
            // 
            this.lapsoDeudores.Enabled = true;
            this.lapsoDeudores.Interval = 18000000D;
            this.lapsoDeudores.Elapsed += new System.Timers.ElapsedEventHandler(this.LapsoDeudores_Elapsed);
            // 
            // lapsoLimpiarEstudiante
            // 
            this.lapsoLimpiarEstudiante.Enabled = true;
            this.lapsoLimpiarEstudiante.Interval = 84600000D;
            this.lapsoLimpiarEstudiante.Elapsed += new System.Timers.ElapsedEventHandler(this.LapsoLimpiarEstudiante_Elapsed);
            // 
            // Britanico
            // 
            this.ServiceName = "Britanico";
            ((System.ComponentModel.ISupportInitialize)(this.lapsoEmail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lapsoDeudores)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lapsoLimpiarEstudiante)).EndInit();

        }

        #endregion

        private System.Timers.Timer lapsoEmail;
        private System.Timers.Timer lapsoDeudores;
        private System.Timers.Timer lapsoLimpiarEstudiante;
    }
}
