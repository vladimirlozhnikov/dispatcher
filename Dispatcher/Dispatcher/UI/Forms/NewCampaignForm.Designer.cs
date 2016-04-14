namespace Dispatcher.UI.Forms
{
    partial class NewCampaignForm
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
            this.campaignNameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.campaignDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.campaignBeginTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.campaignFinishTimePicker = new System.Windows.Forms.DateTimePicker();
            this.saveButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.sourceDataGridView = new System.Windows.Forms.DataGridView();
            this.SourseId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SourceName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SourcePhone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sourceSearchTextBox = new System.Windows.Forms.TextBox();
            this.toRightOneButton = new System.Windows.Forms.Button();
            this.toRightAllButton = new System.Windows.Forms.Button();
            this.toLeftOneButton = new System.Windows.Forms.Button();
            this.toLeftAllButton = new System.Windows.Forms.Button();
            this.desctinationDataGridView = new System.Windows.Forms.DataGridView();
            this.DestinationId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DestinationName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DestinationPhone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.destinationSearchTextBox = new System.Windows.Forms.TextBox();
            this.fbCheckBox = new System.Windows.Forms.CheckBox();
            this.vkCheckBox = new System.Windows.Forms.CheckBox();
            this.okCheckBox = new System.Windows.Forms.CheckBox();
            this.activeCheckBox = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.discountNumericTextBox = new Dispatcher.UI.CustomControls.NumericTextBox();
            this.useCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.sourceDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.desctinationDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.closeButton.Location = new System.Drawing.Point(996, 606);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(105, 44);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "Закрыть";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Имя кампании:";
            // 
            // campaignNameTextBox
            // 
            this.campaignNameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.campaignNameTextBox.Location = new System.Drawing.Point(15, 38);
            this.campaignNameTextBox.Name = "campaignNameTextBox";
            this.campaignNameTextBox.Size = new System.Drawing.Size(312, 26);
            this.campaignNameTextBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(228, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Краткая информация о кампании:";
            // 
            // campaignDescriptionTextBox
            // 
            this.campaignDescriptionTextBox.Location = new System.Drawing.Point(15, 100);
            this.campaignDescriptionTextBox.Multiline = true;
            this.campaignDescriptionTextBox.Name = "campaignDescriptionTextBox";
            this.campaignDescriptionTextBox.Size = new System.Drawing.Size(309, 233);
            this.campaignDescriptionTextBox.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 370);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(161, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Дата начала кампании:";
            // 
            // campaignBeginTimePicker
            // 
            this.campaignBeginTimePicker.Location = new System.Drawing.Point(15, 389);
            this.campaignBeginTimePicker.Name = "campaignBeginTimePicker";
            this.campaignBeginTimePicker.Size = new System.Drawing.Size(306, 22);
            this.campaignBeginTimePicker.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 434);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(183, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "Дата окончания кампании:";
            // 
            // campaignFinishTimePicker
            // 
            this.campaignFinishTimePicker.Location = new System.Drawing.Point(15, 453);
            this.campaignFinishTimePicker.Name = "campaignFinishTimePicker";
            this.campaignFinishTimePicker.Size = new System.Drawing.Size(306, 22);
            this.campaignFinishTimePicker.TabIndex = 9;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.saveButton.Location = new System.Drawing.Point(15, 606);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(115, 44);
            this.saveButton.TabIndex = 10;
            this.saveButton.Text = "Сохранить";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(353, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(220, 16);
            this.label5.TabIndex = 11;
            this.label5.Text = "Выберите участников кампании";
            // 
            // sourceDataGridView
            // 
            this.sourceDataGridView.AllowUserToAddRows = false;
            this.sourceDataGridView.AllowUserToDeleteRows = false;
            this.sourceDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.sourceDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.sourceDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sourceDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SourseId,
            this.SourceName,
            this.SourcePhone});
            this.sourceDataGridView.Location = new System.Drawing.Point(356, 71);
            this.sourceDataGridView.Name = "sourceDataGridView";
            this.sourceDataGridView.ReadOnly = true;
            this.sourceDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.sourceDataGridView.Size = new System.Drawing.Size(349, 404);
            this.sourceDataGridView.TabIndex = 12;
            // 
            // SourseId
            // 
            this.SourseId.FillWeight = 76.14214F;
            this.SourseId.HeaderText = "Номер";
            this.SourseId.Name = "SourseId";
            this.SourseId.ReadOnly = true;
            // 
            // SourceName
            // 
            this.SourceName.FillWeight = 111.9289F;
            this.SourceName.HeaderText = "Имя";
            this.SourceName.Name = "SourceName";
            this.SourceName.ReadOnly = true;
            // 
            // SourcePhone
            // 
            this.SourcePhone.FillWeight = 111.9289F;
            this.SourcePhone.HeaderText = "Телефон";
            this.SourcePhone.Name = "SourcePhone";
            this.SourcePhone.ReadOnly = true;
            // 
            // sourceSearchTextBox
            // 
            this.sourceSearchTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.sourceSearchTextBox.Location = new System.Drawing.Point(356, 39);
            this.sourceSearchTextBox.Name = "sourceSearchTextBox";
            this.sourceSearchTextBox.Size = new System.Drawing.Size(349, 26);
            this.sourceSearchTextBox.TabIndex = 13;
            this.sourceSearchTextBox.TextChanged += new System.EventHandler(this.sourceSearchTextBox_TextChanged);
            // 
            // toRightOneButton
            // 
            this.toRightOneButton.Location = new System.Drawing.Point(711, 174);
            this.toRightOneButton.Name = "toRightOneButton";
            this.toRightOneButton.Size = new System.Drawing.Size(40, 30);
            this.toRightOneButton.TabIndex = 14;
            this.toRightOneButton.Text = ">";
            this.toRightOneButton.UseVisualStyleBackColor = true;
            this.toRightOneButton.Click += new System.EventHandler(this.toRightOneButton_Click);
            // 
            // toRightAllButton
            // 
            this.toRightAllButton.Location = new System.Drawing.Point(711, 210);
            this.toRightAllButton.Name = "toRightAllButton";
            this.toRightAllButton.Size = new System.Drawing.Size(40, 30);
            this.toRightAllButton.TabIndex = 15;
            this.toRightAllButton.Text = ">>";
            this.toRightAllButton.UseVisualStyleBackColor = true;
            this.toRightAllButton.Click += new System.EventHandler(this.toRightAllButton_Click);
            // 
            // toLeftOneButton
            // 
            this.toLeftOneButton.Location = new System.Drawing.Point(711, 246);
            this.toLeftOneButton.Name = "toLeftOneButton";
            this.toLeftOneButton.Size = new System.Drawing.Size(40, 30);
            this.toLeftOneButton.TabIndex = 16;
            this.toLeftOneButton.Text = "<";
            this.toLeftOneButton.UseVisualStyleBackColor = true;
            this.toLeftOneButton.Click += new System.EventHandler(this.toLeftOneButton_Click);
            // 
            // toLeftAllButton
            // 
            this.toLeftAllButton.Location = new System.Drawing.Point(711, 282);
            this.toLeftAllButton.Name = "toLeftAllButton";
            this.toLeftAllButton.Size = new System.Drawing.Size(40, 30);
            this.toLeftAllButton.TabIndex = 17;
            this.toLeftAllButton.Text = "<<";
            this.toLeftAllButton.UseVisualStyleBackColor = true;
            this.toLeftAllButton.Click += new System.EventHandler(this.toLeftAllButton_Click);
            // 
            // desctinationDataGridView
            // 
            this.desctinationDataGridView.AllowUserToAddRows = false;
            this.desctinationDataGridView.AllowUserToDeleteRows = false;
            this.desctinationDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.desctinationDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.desctinationDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.desctinationDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.desctinationDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DestinationId,
            this.DestinationName,
            this.DestinationPhone});
            this.desctinationDataGridView.Location = new System.Drawing.Point(757, 71);
            this.desctinationDataGridView.Name = "desctinationDataGridView";
            this.desctinationDataGridView.ReadOnly = true;
            this.desctinationDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.desctinationDataGridView.Size = new System.Drawing.Size(349, 404);
            this.desctinationDataGridView.TabIndex = 18;
            // 
            // DestinationId
            // 
            this.DestinationId.FillWeight = 76.14214F;
            this.DestinationId.HeaderText = "Номер";
            this.DestinationId.Name = "DestinationId";
            this.DestinationId.ReadOnly = true;
            // 
            // DestinationName
            // 
            this.DestinationName.FillWeight = 111.9289F;
            this.DestinationName.HeaderText = "Имя";
            this.DestinationName.Name = "DestinationName";
            this.DestinationName.ReadOnly = true;
            // 
            // DestinationPhone
            // 
            this.DestinationPhone.FillWeight = 111.9289F;
            this.DestinationPhone.HeaderText = "Телефон";
            this.DestinationPhone.Name = "DestinationPhone";
            this.DestinationPhone.ReadOnly = true;
            // 
            // destinationSearchTextBox
            // 
            this.destinationSearchTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.destinationSearchTextBox.Location = new System.Drawing.Point(757, 38);
            this.destinationSearchTextBox.Name = "destinationSearchTextBox";
            this.destinationSearchTextBox.Size = new System.Drawing.Size(349, 26);
            this.destinationSearchTextBox.TabIndex = 19;
            this.destinationSearchTextBox.TextChanged += new System.EventHandler(this.destinationSearchTextBox_TextChanged);
            // 
            // fbCheckBox
            // 
            this.fbCheckBox.AutoSize = true;
            this.fbCheckBox.Location = new System.Drawing.Point(18, 499);
            this.fbCheckBox.Name = "fbCheckBox";
            this.fbCheckBox.Size = new System.Drawing.Size(89, 20);
            this.fbCheckBox.TabIndex = 20;
            this.fbCheckBox.Text = "Facebook";
            this.fbCheckBox.UseVisualStyleBackColor = true;
            // 
            // vkCheckBox
            // 
            this.vkCheckBox.AutoSize = true;
            this.vkCheckBox.Location = new System.Drawing.Point(18, 526);
            this.vkCheckBox.Name = "vkCheckBox";
            this.vkCheckBox.Size = new System.Drawing.Size(88, 20);
            this.vkCheckBox.TabIndex = 21;
            this.vkCheckBox.Text = "VKontakte";
            this.vkCheckBox.UseVisualStyleBackColor = true;
            // 
            // okCheckBox
            // 
            this.okCheckBox.AutoSize = true;
            this.okCheckBox.Location = new System.Drawing.Point(18, 553);
            this.okCheckBox.Name = "okCheckBox";
            this.okCheckBox.Size = new System.Drawing.Size(112, 20);
            this.okCheckBox.TabIndex = 22;
            this.okCheckBox.Text = "Odnoklassniki";
            this.okCheckBox.UseVisualStyleBackColor = true;
            // 
            // activeCheckBox
            // 
            this.activeCheckBox.AutoSize = true;
            this.activeCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.activeCheckBox.Location = new System.Drawing.Point(356, 499);
            this.activeCheckBox.Name = "activeCheckBox";
            this.activeCheckBox.Size = new System.Drawing.Size(101, 24);
            this.activeCheckBox.TabIndex = 23;
            this.activeCheckBox.Text = "Активная";
            this.activeCheckBox.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(353, 527);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(197, 16);
            this.label6.TabIndex = 24;
            this.label6.Text = "Размер скидки (в процентах)";
            // 
            // discountNumericTextBox
            // 
            this.discountNumericTextBox.AllowSpace = false;
            this.discountNumericTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.discountNumericTextBox.Location = new System.Drawing.Point(356, 553);
            this.discountNumericTextBox.Name = "discountNumericTextBox";
            this.discountNumericTextBox.Size = new System.Drawing.Size(194, 26);
            this.discountNumericTextBox.TabIndex = 25;
            // 
            // useCheckBox
            // 
            this.useCheckBox.AutoSize = true;
            this.useCheckBox.Location = new System.Drawing.Point(18, 579);
            this.useCheckBox.Name = "useCheckBox";
            this.useCheckBox.Size = new System.Drawing.Size(222, 20);
            this.useCheckBox.TabIndex = 26;
            this.useCheckBox.Text = "Одноразовое использование";
            this.useCheckBox.UseVisualStyleBackColor = true;
            this.useCheckBox.CheckedChanged += new System.EventHandler(this.useCheckBox_CheckedChanged);
            // 
            // NewCampaignForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1113, 652);
            this.Controls.Add(this.useCheckBox);
            this.Controls.Add(this.discountNumericTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.activeCheckBox);
            this.Controls.Add(this.okCheckBox);
            this.Controls.Add(this.vkCheckBox);
            this.Controls.Add(this.fbCheckBox);
            this.Controls.Add(this.destinationSearchTextBox);
            this.Controls.Add(this.desctinationDataGridView);
            this.Controls.Add(this.toLeftAllButton);
            this.Controls.Add(this.toLeftOneButton);
            this.Controls.Add(this.toRightAllButton);
            this.Controls.Add(this.toRightOneButton);
            this.Controls.Add(this.sourceSearchTextBox);
            this.Controls.Add(this.sourceDataGridView);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.campaignFinishTimePicker);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.campaignBeginTimePicker);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.campaignDescriptionTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.campaignNameTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.closeButton);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1129, 690);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1129, 690);
            this.Name = "NewCampaignForm";
            this.Text = "Кампания";
            ((System.ComponentModel.ISupportInitialize)(this.sourceDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.desctinationDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox campaignNameTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox campaignDescriptionTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker campaignBeginTimePicker;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker campaignFinishTimePicker;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView sourceDataGridView;
        private System.Windows.Forms.TextBox sourceSearchTextBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn SourseId;
        private System.Windows.Forms.DataGridViewTextBoxColumn SourceName;
        private System.Windows.Forms.DataGridViewTextBoxColumn SourcePhone;
        private System.Windows.Forms.Button toRightOneButton;
        private System.Windows.Forms.Button toRightAllButton;
        private System.Windows.Forms.Button toLeftOneButton;
        private System.Windows.Forms.Button toLeftAllButton;
        private System.Windows.Forms.DataGridView desctinationDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn DestinationId;
        private System.Windows.Forms.DataGridViewTextBoxColumn DestinationName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DestinationPhone;
        private System.Windows.Forms.TextBox destinationSearchTextBox;
        private System.Windows.Forms.CheckBox fbCheckBox;
        private System.Windows.Forms.CheckBox vkCheckBox;
        private System.Windows.Forms.CheckBox okCheckBox;
        private System.Windows.Forms.CheckBox activeCheckBox;
        private System.Windows.Forms.Label label6;
        private CustomControls.NumericTextBox discountNumericTextBox;
        private System.Windows.Forms.CheckBox useCheckBox;
    }
}