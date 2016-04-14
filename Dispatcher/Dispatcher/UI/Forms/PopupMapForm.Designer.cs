namespace Dispatcher.UI.Forms
{
    partial class PopupMapForm
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
            this.searchSplitContainer = new System.Windows.Forms.SplitContainer();
            this.streetsListBox = new System.Windows.Forms.ListBox();
            this.mapControl = new GMap.NET.WindowsForms.GMapControl();
            ((System.ComponentModel.ISupportInitialize)(this.searchSplitContainer)).BeginInit();
            this.searchSplitContainer.Panel1.SuspendLayout();
            this.searchSplitContainer.Panel2.SuspendLayout();
            this.searchSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // searchSplitContainer
            // 
            this.searchSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.searchSplitContainer.Name = "searchSplitContainer";
            // 
            // searchSplitContainer.Panel1
            // 
            this.searchSplitContainer.Panel1.Controls.Add(this.streetsListBox);
            // 
            // searchSplitContainer.Panel2
            // 
            this.searchSplitContainer.Panel2.Controls.Add(this.mapControl);
            this.searchSplitContainer.Size = new System.Drawing.Size(921, 594);
            this.searchSplitContainer.SplitterDistance = 394;
            this.searchSplitContainer.TabIndex = 0;
            // 
            // streetsListBox
            // 
            this.streetsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.streetsListBox.FormattingEnabled = true;
            this.streetsListBox.Location = new System.Drawing.Point(0, 0);
            this.streetsListBox.Name = "streetsListBox";
            this.streetsListBox.Size = new System.Drawing.Size(392, 592);
            this.streetsListBox.TabIndex = 0;
            this.streetsListBox.SelectedIndexChanged += new System.EventHandler(this.streetsListBox_SelectedIndexChanged);
            // 
            // mapControl
            // 
            this.mapControl.Bearing = 0F;
            this.mapControl.CanDragMap = true;
            this.mapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapControl.EmptyTileColor = System.Drawing.Color.Navy;
            this.mapControl.GrayScaleMode = false;
            this.mapControl.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.mapControl.LevelsKeepInMemmory = 5;
            this.mapControl.Location = new System.Drawing.Point(0, 0);
            this.mapControl.MarkersEnabled = true;
            this.mapControl.MaxZoom = 16;
            this.mapControl.MinZoom = 2;
            this.mapControl.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.mapControl.Name = "mapControl";
            this.mapControl.NegativeMode = false;
            this.mapControl.PolygonsEnabled = true;
            this.mapControl.RetryLoadTile = 0;
            this.mapControl.RoutesEnabled = true;
            this.mapControl.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.mapControl.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.mapControl.ShowTileGridLines = false;
            this.mapControl.Size = new System.Drawing.Size(521, 592);
            this.mapControl.TabIndex = 0;
            this.mapControl.Zoom = 2D;
            // 
            // PopupMapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(921, 594);
            this.ControlBox = false;
            this.Controls.Add(this.searchSplitContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PopupMapForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Поиск адреса...";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.PopupMapForm_Load);
            this.searchSplitContainer.Panel1.ResumeLayout(false);
            this.searchSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.searchSplitContainer)).EndInit();
            this.searchSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer searchSplitContainer;
        private System.Windows.Forms.ListBox streetsListBox;
        private GMap.NET.WindowsForms.GMapControl mapControl;
    }
}