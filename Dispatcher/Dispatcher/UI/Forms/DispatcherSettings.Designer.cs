namespace Dispatcher.UI.Forms
{
    partial class DispatcherSettings
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.enableQueueCheckBox = new System.Windows.Forms.CheckBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.hoursTextBox = new Dispatcher.UI.CustomControls.NumericTextBox();
            this.metersTextBox = new Dispatcher.UI.CustomControls.NumericTextBox();
            this.secondsTextBox = new Dispatcher.UI.CustomControls.NumericTextBox();
            this.ignoreTextBox = new Dispatcher.UI.CustomControls.NumericTextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(10, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(322, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Период для получения списка заказов (часов) *";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(10, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(236, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Радиус для поиска машин (метры) *";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ignoreTextBox);
            this.groupBox1.Controls.Add(this.secondsTextBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.enableQueueCheckBox);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(13, 151);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(322, 277);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Настройки очереди";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 133);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(288, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "Количество проигнорированных заказов *";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(313, 16);
            this.label3.TabIndex = 1;
            this.label3.Text = "Период времени отклика водителя (секунды) *";
            // 
            // enableQueueCheckBox
            // 
            this.enableQueueCheckBox.AutoSize = true;
            this.enableQueueCheckBox.Location = new System.Drawing.Point(7, 22);
            this.enableQueueCheckBox.Name = "enableQueueCheckBox";
            this.enableQueueCheckBox.Size = new System.Drawing.Size(149, 20);
            this.enableQueueCheckBox.TabIndex = 0;
            this.enableQueueCheckBox.Text = "Включить очередь";
            this.enableQueueCheckBox.UseVisualStyleBackColor = true;
            this.enableQueueCheckBox.CheckedChanged += new System.EventHandler(this.enableQueueCheckBox_CheckedChanged);
            // 
            // closeButton
            // 
            this.closeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.closeButton.Location = new System.Drawing.Point(222, 468);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(113, 30);
            this.closeButton.TabIndex = 5;
            this.closeButton.Text = "Закрыть";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.saveButton.Location = new System.Drawing.Point(13, 468);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(113, 30);
            this.saveButton.TabIndex = 6;
            this.saveButton.Text = "Сохранить";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // hoursTextBox
            // 
            this.hoursTextBox.AllowSpace = false;
            this.hoursTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hoursTextBox.Location = new System.Drawing.Point(13, 36);
            this.hoursTextBox.Name = "hoursTextBox";
            this.hoursTextBox.Size = new System.Drawing.Size(100, 26);
            this.hoursTextBox.TabIndex = 7;
            // 
            // metersTextBox
            // 
            this.metersTextBox.AllowSpace = false;
            this.metersTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.metersTextBox.Location = new System.Drawing.Point(13, 99);
            this.metersTextBox.Name = "metersTextBox";
            this.metersTextBox.Size = new System.Drawing.Size(100, 26);
            this.metersTextBox.TabIndex = 8;
            // 
            // secondsTextBox
            // 
            this.secondsTextBox.AllowSpace = false;
            this.secondsTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.secondsTextBox.Location = new System.Drawing.Point(7, 80);
            this.secondsTextBox.Name = "secondsTextBox";
            this.secondsTextBox.Size = new System.Drawing.Size(100, 26);
            this.secondsTextBox.TabIndex = 5;
            // 
            // ignoreTextBox
            // 
            this.ignoreTextBox.AllowSpace = false;
            this.ignoreTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ignoreTextBox.Location = new System.Drawing.Point(6, 152);
            this.ignoreTextBox.Name = "ignoreTextBox";
            this.ignoreTextBox.Size = new System.Drawing.Size(100, 26);
            this.ignoreTextBox.TabIndex = 6;
            // 
            // DispatcherSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 503);
            this.Controls.Add(this.metersTextBox);
            this.Controls.Add(this.hoursTextBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DispatcherSettings";
            this.Text = "Дополнительные настройки";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox enableQueueCheckBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button saveButton;
        private CustomControls.NumericTextBox hoursTextBox;
        private CustomControls.NumericTextBox metersTextBox;
        private CustomControls.NumericTextBox secondsTextBox;
        private CustomControls.NumericTextBox ignoreTextBox;
    }
}