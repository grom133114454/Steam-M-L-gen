namespace ManifestHubDownloader;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.TextBox txtGameId;
    private System.Windows.Forms.Button btnFind;
    private System.Windows.Forms.PictureBox picGame;
    private System.Windows.Forms.Button btnDownloadSteamTools;

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
        this.components = new System.ComponentModel.Container();
        this.txtGameId = new System.Windows.Forms.TextBox();
        this.btnFind = new System.Windows.Forms.Button();
        this.picGame = new System.Windows.Forms.PictureBox();
        this.btnDownloadSteamTools = new System.Windows.Forms.Button();
        ((System.ComponentModel.ISupportInitialize)(this.picGame)).BeginInit();
        this.SuspendLayout();
        // 
        // txtGameId
        // 
        this.txtGameId.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
        this.txtGameId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.txtGameId.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.txtGameId.ForeColor = System.Drawing.Color.White;
        this.txtGameId.Location = new System.Drawing.Point(12, 12);
        this.txtGameId.Name = "txtGameId";
        this.txtGameId.PlaceholderText = "Enter Game ID";
        this.txtGameId.Size = new System.Drawing.Size(200, 29);
        this.txtGameId.TabIndex = 0;
        this.txtGameId.TextChanged += new System.EventHandler(this.txtGameId_TextChanged);
        // 
        // btnFind
        // 
        this.btnFind.BackColor = System.Drawing.Color.FromArgb(45, 45, 45);
        this.btnFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnFind.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.btnFind.ForeColor = System.Drawing.Color.White;
        this.btnFind.Location = new System.Drawing.Point(218, 12);
        this.btnFind.Name = "btnFind";
        this.btnFind.Size = new System.Drawing.Size(100, 29);
        this.btnFind.TabIndex = 1;
        this.btnFind.Text = "Find";
        this.btnFind.UseVisualStyleBackColor = false;
        this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
        // 
        // picGame
        // 
        this.picGame.BackColor = System.Drawing.Color.FromArgb(20, 20, 20);
        this.picGame.Location = new System.Drawing.Point(12, 47);
        this.picGame.Name = "picGame";
        this.picGame.Size = new System.Drawing.Size(306, 141);
        this.picGame.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
        this.picGame.TabIndex = 2;
        this.picGame.TabStop = false;
        // 
        // btnDownloadSteamTools
        // 
        this.btnDownloadSteamTools.BackColor = System.Drawing.Color.FromArgb(45, 45, 45);
        this.btnDownloadSteamTools.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnDownloadSteamTools.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.btnDownloadSteamTools.ForeColor = System.Drawing.Color.White;
        this.btnDownloadSteamTools.Location = new System.Drawing.Point(12, 194);
        this.btnDownloadSteamTools.Name = "btnDownloadSteamTools";
        this.btnDownloadSteamTools.Size = new System.Drawing.Size(306, 29);
        this.btnDownloadSteamTools.TabIndex = 3;
        this.btnDownloadSteamTools.Text = "Download SteamTools";
        this.btnDownloadSteamTools.UseVisualStyleBackColor = false;
        this.btnDownloadSteamTools.Click += new System.EventHandler(this.btnDownloadSteamTools_Click);
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.Color.FromArgb(15, 15, 15);
        this.ClientSize = new System.Drawing.Size(330, 235);
        this.Controls.Add(this.btnDownloadSteamTools);
        this.Controls.Add(this.picGame);
        this.Controls.Add(this.btnFind);
        this.Controls.Add(this.txtGameId);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.Name = "Form1";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Steam M&L Generator";
        ((System.ComponentModel.ISupportInitialize)(this.picGame)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion
}
