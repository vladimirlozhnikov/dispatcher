namespace Dispatcher.UI.Forms
{
    partial class OrdersHistoryFromToForm
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
            this.ordersListView = new System.Windows.Forms.ListView();
            this.orderNumberColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.orderStatusColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.customerPhoneColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.driverPhoneColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.createdColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label2 = new System.Windows.Forms.Label();
            this.namePhoneTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.fromButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.toButton = new System.Windows.Forms.Button();
            this.refreshButton = new System.Windows.Forms.Button();
            this.monthCalendar = new System.Windows.Forms.MonthCalendar();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(956, 501);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(102, 45);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "Закрыть";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // ordersListView
            // 
            this.ordersListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.orderNumberColumn,
            this.orderStatusColumn,
            this.customerPhoneColumn,
            this.driverPhoneColumn,
            this.createdColumn});
            this.ordersListView.GridLines = true;
            this.ordersListView.Location = new System.Drawing.Point(17, 163);
            this.ordersListView.MultiSelect = false;
            this.ordersListView.Name = "ordersListView";
            this.ordersListView.Size = new System.Drawing.Size(1041, 332);
            this.ordersListView.TabIndex = 3;
            this.ordersListView.UseCompatibleStateImageBehavior = false;
            this.ordersListView.View = System.Windows.Forms.View.Details;
            // 
            // orderNumberColumn
            // 
            this.orderNumberColumn.Text = "Заказ";
            this.orderNumberColumn.Width = 131;
            // 
            // orderStatusColumn
            // 
            this.orderStatusColumn.Text = "Статус";
            this.orderStatusColumn.Width = 143;
            // 
            // customerPhoneColumn
            // 
            this.customerPhoneColumn.Text = "Клиент";
            this.customerPhoneColumn.Width = 180;
            // 
            // driverPhoneColumn
            // 
            this.driverPhoneColumn.Text = "Водитель";
            this.driverPhoneColumn.Width = 170;
            // 
            // createdColumn
            // 
            this.createdColumn.Text = "Создан";
            this.createdColumn.Width = 185;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Телефон:";
            // 
            // namePhoneTextBox
            // 
            this.namePhoneTextBox.Location = new System.Drawing.Point(106, 46);
            this.namePhoneTextBox.Name = "namePhoneTextBox";
            this.namePhoneTextBox.Size = new System.Drawing.Size(461, 26);
            this.namePhoneTextBox.TabIndex = 5;
            this.namePhoneTextBox.TextChanged += new System.EventHandler(this.namePhoneTextBox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Начало:";
            // 
            // fromButton
            // 
            this.fromButton.Location = new System.Drawing.Point(17, 114);
            this.fromButton.Name = "fromButton";
            this.fromButton.Size = new System.Drawing.Size(200, 33);
            this.fromButton.TabIndex = 8;
            this.fromButton.Text = "Не указано";
            this.fromButton.UseVisualStyleBackColor = true;
            this.fromButton.Click += new System.EventHandler(this.fromButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(363, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 20);
            this.label3.TabIndex = 9;
            this.label3.Text = "Окончание:";
            // 
            // toButton
            // 
            this.toButton.Location = new System.Drawing.Point(367, 114);
            this.toButton.Name = "toButton";
            this.toButton.Size = new System.Drawing.Size(200, 33);
            this.toButton.TabIndex = 10;
            this.toButton.Text = "Не указано";
            this.toButton.UseVisualStyleBackColor = true;
            this.toButton.Click += new System.EventHandler(this.toButton_Click);
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(834, 501);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(116, 45);
            this.refreshButton.TabIndex = 11;
            this.refreshButton.Text = "Обновить";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // monthCalendar
            // 
            this.monthCalendar.Location = new System.Drawing.Point(641, 1);
            this.monthCalendar.Name = "monthCalendar";
            this.monthCalendar.TabIndex = 12;
            // 
            // OrdersHistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1070, 549);
            this.Controls.Add(this.monthCalendar);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.toButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.fromButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.namePhoneTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ordersListView);
            this.Controls.Add(this.closeButton);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "OrdersHistoryForm";
            this.Text = "История заказов";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.ListView ordersListView;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox namePhoneTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button fromButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button toButton;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.ColumnHeader orderNumberColumn;
        private System.Windows.Forms.ColumnHeader orderStatusColumn;
        private System.Windows.Forms.ColumnHeader customerPhoneColumn;
        private System.Windows.Forms.ColumnHeader driverPhoneColumn;
        private System.Windows.Forms.ColumnHeader createdColumn;
        private System.Windows.Forms.MonthCalendar monthCalendar;
    }
}