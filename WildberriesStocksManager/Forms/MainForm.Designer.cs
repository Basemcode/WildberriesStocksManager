namespace WildberriesStocksManager
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnGetData = new Button();
            dgvProductsInfo = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dgvProductsInfo).BeginInit();
            SuspendLayout();
            // 
            // btnGetData
            // 
            btnGetData.Location = new Point(12, 365);
            btnGetData.Name = "btnGetData";
            btnGetData.Size = new Size(94, 73);
            btnGetData.TabIndex = 0;
            btnGetData.Text = "GetData";
            btnGetData.UseVisualStyleBackColor = true;
            btnGetData.Click += btnGetData_Click;
            // 
            // dgvProductsInfo
            // 
            dgvProductsInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProductsInfo.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProductsInfo.Location = new Point(7, 10);
            dgvProductsInfo.Name = "dgvProductsInfo";
            dgvProductsInfo.RowHeadersWidth = 51;
            dgvProductsInfo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductsInfo.Size = new Size(1277, 339);
            dgvProductsInfo.TabIndex = 1;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1293, 450);
            Controls.Add(dgvProductsInfo);
            Controls.Add(btnGetData);
            Name = "MainForm";
            Text = "MainForm";
            ((System.ComponentModel.ISupportInitialize)dgvProductsInfo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btnGetData;
        private DataGridView dgvProductsInfo;
    }
}
