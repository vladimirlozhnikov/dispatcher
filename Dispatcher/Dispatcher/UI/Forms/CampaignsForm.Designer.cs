namespace Dispatcher.UI.Forms
{
    partial class CampaignsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.closeButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.campaignGridView = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CampaignName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CampaignDateFrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CampaignExpiresTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.refreshButton = new System.Windows.Forms.Button();
            this.newCampaignButton = new System.Windows.Forms.Button();
            this.CampaignActive = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.campaignGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.closeButton.Location = new System.Drawing.Point(622, 502);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(105, 44);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Закрыть";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Список бонусов";
            // 
            // campaignGridView
            // 
            this.campaignGridView.AllowUserToAddRows = false;
            this.campaignGridView.AllowUserToDeleteRows = false;
            this.campaignGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.campaignGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.campaignGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.campaignGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.campaignGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.CampaignName,
            this.CampaignDateFrom,
            this.CampaignExpiresTo,
            this.CampaignActive});
            this.campaignGridView.Location = new System.Drawing.Point(16, 32);
            this.campaignGridView.Name = "campaignGridView";
            this.campaignGridView.ReadOnly = true;
            this.campaignGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.campaignGridView.Size = new System.Drawing.Size(716, 460);
            this.campaignGridView.TabIndex = 2;
            // 
            // Id
            // 
            this.Id.HeaderText = "Номер";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            // 
            // CampaignName
            // 
            this.CampaignName.HeaderText = "Имя";
            this.CampaignName.Name = "CampaignName";
            this.CampaignName.ReadOnly = true;
            // 
            // CampaignDateFrom
            // 
            this.CampaignDateFrom.HeaderText = "Дата начала";
            this.CampaignDateFrom.Name = "CampaignDateFrom";
            this.CampaignDateFrom.ReadOnly = true;
            // 
            // CampaignExpiresTo
            // 
            this.CampaignExpiresTo.HeaderText = "Дата окончания";
            this.CampaignExpiresTo.Name = "CampaignExpiresTo";
            this.CampaignExpiresTo.ReadOnly = true;
            // 
            // refreshButton
            // 
            this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.refreshButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.refreshButton.Location = new System.Drawing.Point(16, 502);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(105, 44);
            this.refreshButton.TabIndex = 3;
            this.refreshButton.Text = "Обновить";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // newCampaignButton
            // 
            this.newCampaignButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.newCampaignButton.Location = new System.Drawing.Point(127, 502);
            this.newCampaignButton.Name = "newCampaignButton";
            this.newCampaignButton.Size = new System.Drawing.Size(105, 44);
            this.newCampaignButton.TabIndex = 4;
            this.newCampaignButton.Text = "Новая";
            this.newCampaignButton.UseVisualStyleBackColor = true;
            this.newCampaignButton.Click += new System.EventHandler(this.newCampaignButton_Click);
            // 
            // CampaignActive
            // 
            this.CampaignActive.HeaderText = "Активность";
            this.CampaignActive.Name = "CampaignActive";
            this.CampaignActive.ReadOnly = true;
            // 
            // CampaignsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(739, 558);
            this.Controls.Add(this.newCampaignButton);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.campaignGridView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.closeButton);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CampaignsForm";
            this.Text = "Бонусные кампании";
            ((System.ComponentModel.ISupportInitialize)(this.campaignGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView campaignGridView;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn CampaignName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CampaignDateFrom;
        private System.Windows.Forms.DataGridViewTextBoxColumn CampaignExpiresTo;
        private System.Windows.Forms.Button newCampaignButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn CampaignActive;
    }
}