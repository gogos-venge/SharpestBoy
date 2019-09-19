namespace SharpestBoy {
    partial class Debugger {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.disassembler = new System.Windows.Forms.ListView();
            this.bp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hexBox1 = new Be.Windows.Forms.HexBox();
            this.componentList = new System.Windows.Forms.CheckedListBox();
            this.State = new System.Windows.Forms.RichTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextInstructionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextCycleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextIRQToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextHALTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StackView = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.componentList);
            this.splitContainer1.Panel2.Controls.Add(this.State);
            this.splitContainer1.Size = new System.Drawing.Size(1184, 682);
            this.splitContainer1.SplitterDistance = 944;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.disassembler);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.StackView);
            this.splitContainer2.Panel2.Controls.Add(this.hexBox1);
            this.splitContainer2.Size = new System.Drawing.Size(944, 682);
            this.splitContainer2.SplitterDistance = 514;
            this.splitContainer2.TabIndex = 0;
            // 
            // disassembler
            // 
            this.disassembler.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.bp,
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader7,
            this.columnHeader5,
            this.columnHeader6});
            this.disassembler.Dock = System.Windows.Forms.DockStyle.Fill;
            this.disassembler.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.disassembler.FullRowSelect = true;
            this.disassembler.GridLines = true;
            this.disassembler.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.disassembler.HideSelection = false;
            this.disassembler.Location = new System.Drawing.Point(0, 0);
            this.disassembler.MultiSelect = false;
            this.disassembler.Name = "disassembler";
            this.disassembler.Size = new System.Drawing.Size(944, 514);
            this.disassembler.TabIndex = 0;
            this.disassembler.UseCompatibleStateImageBehavior = false;
            this.disassembler.View = System.Windows.Forms.View.Details;
            this.disassembler.VirtualMode = true;
            this.disassembler.DoubleClick += new System.EventHandler(this.debug_DoubleClick);
            // 
            // bp
            // 
            this.bp.Text = "";
            this.bp.Width = 20;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Address";
            this.columnHeader1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader1.Width = 169;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Byte(s)";
            this.columnHeader3.Width = 142;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Disassembled Opcode";
            this.columnHeader4.Width = 219;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Mnemonic";
            this.columnHeader7.Width = 148;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Clocks";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Comments";
            this.columnHeader6.Width = 165;
            // 
            // hexBox1
            // 
            this.hexBox1.ColumnInfoVisible = true;
            this.hexBox1.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexBox1.LineInfoVisible = true;
            this.hexBox1.Location = new System.Drawing.Point(0, 0);
            this.hexBox1.Name = "hexBox1";
            this.hexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexBox1.Size = new System.Drawing.Size(717, 164);
            this.hexBox1.StringViewVisible = true;
            this.hexBox1.TabIndex = 0;
            this.hexBox1.UseFixedBytesPerLine = true;
            this.hexBox1.VScrollBarVisible = true;
            this.hexBox1.CurrentPositionInLineChanged += new System.EventHandler(this.hexBox1_CurrentPositionInLineChanged);
            // 
            // componentList
            // 
            this.componentList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.componentList.FormattingEnabled = true;
            this.componentList.Location = new System.Drawing.Point(4, 518);
            this.componentList.Name = "componentList";
            this.componentList.Size = new System.Drawing.Size(229, 154);
            this.componentList.TabIndex = 1;
            this.componentList.Click += new System.EventHandler(this.componentList_Click);
            // 
            // State
            // 
            this.State.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.State.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.State.Location = new System.Drawing.Point(3, 3);
            this.State.Name = "State";
            this.State.Size = new System.Drawing.Size(236, 511);
            this.State.TabIndex = 0;
            this.State.Text = "";
            this.State.WordWrap = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Status});
            this.statusStrip1.Location = new System.Drawing.Point(0, 709);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1184, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // Status
            // 
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(0, 17);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1184, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nextInstructionToolStripMenuItem,
            this.nextCycleToolStripMenuItem,
            this.nextIRQToolStripMenuItem,
            this.nextHALTToolStripMenuItem});
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.runToolStripMenuItem.Text = "Run";
            // 
            // nextInstructionToolStripMenuItem
            // 
            this.nextInstructionToolStripMenuItem.Name = "nextInstructionToolStripMenuItem";
            this.nextInstructionToolStripMenuItem.ShortcutKeyDisplayString = "F5";
            this.nextInstructionToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.nextInstructionToolStripMenuItem.Text = "Next Instruction";
            this.nextInstructionToolStripMenuItem.Click += new System.EventHandler(this.nextInstructionToolStripMenuItem_Click);
            // 
            // nextCycleToolStripMenuItem
            // 
            this.nextCycleToolStripMenuItem.Name = "nextCycleToolStripMenuItem";
            this.nextCycleToolStripMenuItem.ShortcutKeyDisplayString = "F6";
            this.nextCycleToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.nextCycleToolStripMenuItem.Text = "Next Cycle";
            this.nextCycleToolStripMenuItem.Click += new System.EventHandler(this.nextCycleToolStripMenuItem_Click);
            // 
            // nextIRQToolStripMenuItem
            // 
            this.nextIRQToolStripMenuItem.Name = "nextIRQToolStripMenuItem";
            this.nextIRQToolStripMenuItem.ShortcutKeyDisplayString = "F7";
            this.nextIRQToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.nextIRQToolStripMenuItem.Text = "Next IRQ";
            this.nextIRQToolStripMenuItem.Click += new System.EventHandler(this.nextIRQToolStripMenuItem_Click);
            // 
            // nextHALTToolStripMenuItem
            // 
            this.nextHALTToolStripMenuItem.Name = "nextHALTToolStripMenuItem";
            this.nextHALTToolStripMenuItem.ShortcutKeyDisplayString = "F12";
            this.nextHALTToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.nextHALTToolStripMenuItem.Text = "Next HALT";
            this.nextHALTToolStripMenuItem.Click += new System.EventHandler(this.nextHALTToolStripMenuItem_Click);
            // 
            // StackView
            // 
            this.StackView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.StackView.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StackView.HideSelection = false;
            this.StackView.Location = new System.Drawing.Point(723, 2);
            this.StackView.MultiSelect = false;
            this.StackView.Name = "StackView";
            this.StackView.Size = new System.Drawing.Size(218, 159);
            this.StackView.TabIndex = 1;
            this.StackView.UseCompatibleStateImageBehavior = false;
            this.StackView.View = System.Windows.Forms.View.Details;
            this.StackView.VirtualListSize = 32767;
            this.StackView.VirtualMode = true;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Stack Pointer";
            this.columnHeader2.Width = 207;
            // 
            // Debugger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 731);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "Debugger";
            this.Text = "Debugger";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListView disassembler;
        private System.Windows.Forms.ColumnHeader bp;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextInstructionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextCycleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextIRQToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextHALTToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private Be.Windows.Forms.HexBox hexBox1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel Status;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.RichTextBox State;
        private System.Windows.Forms.CheckedListBox componentList;
        private System.Windows.Forms.ListView StackView;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}