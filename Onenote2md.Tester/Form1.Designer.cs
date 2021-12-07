namespace Onenote2md.Core.Tester
{
    partial class Form1
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
            this.btnGetChildObjectIDs = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pageBox = new System.Windows.Forms.TextBox();
            this.notebookBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnGetSections = new System.Windows.Forms.Button();
            this.logList = new System.Windows.Forms.ListBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.btnGetPages = new System.Windows.Forms.Button();
            this.sectionBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCopy = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.objectBox = new System.Windows.Forms.TextBox();
            this.btnGetObject = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.btnGetPageContent = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.mdPreviewBox = new System.Windows.Forms.TextBox();
            this.btnCloseNotebook = new System.Windows.Forms.Button();
            this.btnGeneratePageMd = new System.Windows.Forms.Button();
            this.txtOutDir = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnGenerateSectionMd = new System.Windows.Forms.Button();
            this.btnGenerateNotebookMd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnGetChildObjectIDs
            // 
            this.btnGetChildObjectIDs.Location = new System.Drawing.Point(253, 89);
            this.btnGetChildObjectIDs.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetChildObjectIDs.Name = "btnGetChildObjectIDs";
            this.btnGetChildObjectIDs.Size = new System.Drawing.Size(117, 30);
            this.btnGetChildObjectIDs.TabIndex = 0;
            this.btnGetChildObjectIDs.Text = "GetChildObjectIDs";
            this.btnGetChildObjectIDs.UseVisualStyleBackColor = true;
            this.btnGetChildObjectIDs.Click += new System.EventHandler(this.BtnGetChildObjectIDs_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 95);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Page Name";
            // 
            // pageBox
            // 
            this.pageBox.Location = new System.Drawing.Point(119, 92);
            this.pageBox.Margin = new System.Windows.Forms.Padding(4);
            this.pageBox.Name = "pageBox";
            this.pageBox.Size = new System.Drawing.Size(116, 23);
            this.pageBox.TabIndex = 2;
            this.pageBox.Text = "Headers";
            // 
            // notebookBox
            // 
            this.notebookBox.Location = new System.Drawing.Point(119, 16);
            this.notebookBox.Margin = new System.Windows.Forms.Padding(4);
            this.notebookBox.Name = "notebookBox";
            this.notebookBox.Size = new System.Drawing.Size(116, 23);
            this.notebookBox.TabIndex = 4;
            this.notebookBox.Text = "trash";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 20);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Notebook Name";
            // 
            // btnGetSections
            // 
            this.btnGetSections.Location = new System.Drawing.Point(253, 13);
            this.btnGetSections.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetSections.Name = "btnGetSections";
            this.btnGetSections.Size = new System.Drawing.Size(117, 30);
            this.btnGetSections.TabIndex = 5;
            this.btnGetSections.Text = "Get Sections";
            this.btnGetSections.UseVisualStyleBackColor = true;
            this.btnGetSections.Click += new System.EventHandler(this.BtnGetSections_Click);
            // 
            // logList
            // 
            this.logList.FormattingEnabled = true;
            this.logList.ItemHeight = 17;
            this.logList.Location = new System.Drawing.Point(14, 366);
            this.logList.Margin = new System.Windows.Forms.Padding(4);
            this.logList.Name = "logList";
            this.logList.Size = new System.Drawing.Size(868, 429);
            this.logList.TabIndex = 6;
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(709, 328);
            this.clearButton.Margin = new System.Windows.Forms.Padding(4);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(88, 30);
            this.clearButton.TabIndex = 7;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // btnGetPages
            // 
            this.btnGetPages.Location = new System.Drawing.Point(253, 51);
            this.btnGetPages.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetPages.Name = "btnGetPages";
            this.btnGetPages.Size = new System.Drawing.Size(117, 30);
            this.btnGetPages.TabIndex = 10;
            this.btnGetPages.Text = "Get Pages";
            this.btnGetPages.UseVisualStyleBackColor = true;
            this.btnGetPages.Click += new System.EventHandler(this.BtnGetPages_Click);
            // 
            // sectionBox
            // 
            this.sectionBox.Location = new System.Drawing.Point(119, 54);
            this.sectionBox.Margin = new System.Windows.Forms.Padding(4);
            this.sectionBox.Name = "sectionBox";
            this.sectionBox.Size = new System.Drawing.Size(116, 23);
            this.sectionBox.TabIndex = 9;
            this.sectionBox.Text = "Section2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 58);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "Section Name";
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(615, 328);
            this.btnCopy.Margin = new System.Windows.Forms.Padding(4);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(88, 30);
            this.btnCopy.TabIndex = 11;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.BtnCopy_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 180);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 17);
            this.label4.TabIndex = 12;
            this.label4.Text = "ObjectID";
            // 
            // objectBox
            // 
            this.objectBox.Location = new System.Drawing.Point(119, 177);
            this.objectBox.Margin = new System.Windows.Forms.Padding(4);
            this.objectBox.Name = "objectBox";
            this.objectBox.Size = new System.Drawing.Size(116, 23);
            this.objectBox.TabIndex = 13;
            this.objectBox.Text = "Headers";
            // 
            // btnGetObject
            // 
            this.btnGetObject.Location = new System.Drawing.Point(253, 173);
            this.btnGetObject.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetObject.Name = "btnGetObject";
            this.btnGetObject.Size = new System.Drawing.Size(117, 30);
            this.btnGetObject.TabIndex = 14;
            this.btnGetObject.Text = "Get";
            this.btnGetObject.UseVisualStyleBackColor = true;
            this.btnGetObject.Click += new System.EventHandler(this.BtnGetObject_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(377, 89);
            this.button6.Margin = new System.Windows.Forms.Padding(4);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(117, 30);
            this.button6.TabIndex = 15;
            this.button6.Text = "LogChildObjects";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // btnGetPageContent
            // 
            this.btnGetPageContent.Location = new System.Drawing.Point(500, 89);
            this.btnGetPageContent.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetPageContent.Name = "btnGetPageContent";
            this.btnGetPageContent.Size = new System.Drawing.Size(117, 30);
            this.btnGetPageContent.TabIndex = 16;
            this.btnGetPageContent.Text = "GetPageContent";
            this.btnGetPageContent.UseVisualStyleBackColor = true;
            this.btnGetPageContent.Click += new System.EventHandler(this.BtnGetPageContent_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(1337, 12);
            this.button8.Margin = new System.Windows.Forms.Padding(4);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(88, 30);
            this.button8.TabIndex = 18;
            this.button8.Text = "Clear";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(500, 127);
            this.button9.Margin = new System.Windows.Forms.Padding(4);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(117, 30);
            this.button9.TabIndex = 19;
            this.button9.Text = "Preview MD";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // mdPreviewBox
            // 
            this.mdPreviewBox.Location = new System.Drawing.Point(906, 48);
            this.mdPreviewBox.Margin = new System.Windows.Forms.Padding(4);
            this.mdPreviewBox.Multiline = true;
            this.mdPreviewBox.Name = "mdPreviewBox";
            this.mdPreviewBox.Size = new System.Drawing.Size(517, 747);
            this.mdPreviewBox.TabIndex = 20;
            // 
            // btnCloseNotebook
            // 
            this.btnCloseNotebook.Location = new System.Drawing.Point(377, 12);
            this.btnCloseNotebook.Margin = new System.Windows.Forms.Padding(4);
            this.btnCloseNotebook.Name = "btnCloseNotebook";
            this.btnCloseNotebook.Size = new System.Drawing.Size(117, 30);
            this.btnCloseNotebook.TabIndex = 21;
            this.btnCloseNotebook.Text = "Close Notebook";
            this.btnCloseNotebook.UseVisualStyleBackColor = true;
            this.btnCloseNotebook.Click += new System.EventHandler(this.BtnCloseNotebook_Click);
            // 
            // btnGeneratePageMd
            // 
            this.btnGeneratePageMd.Location = new System.Drawing.Point(624, 89);
            this.btnGeneratePageMd.Margin = new System.Windows.Forms.Padding(4);
            this.btnGeneratePageMd.Name = "btnGeneratePageMd";
            this.btnGeneratePageMd.Size = new System.Drawing.Size(121, 30);
            this.btnGeneratePageMd.TabIndex = 24;
            this.btnGeneratePageMd.Text = "GeneratePageMd";
            this.btnGeneratePageMd.UseVisualStyleBackColor = true;
            this.btnGeneratePageMd.Click += new System.EventHandler(this.BtnGeneratePageMd_Click);
            // 
            // txtOutDir
            // 
            this.txtOutDir.Location = new System.Drawing.Point(119, 222);
            this.txtOutDir.Margin = new System.Windows.Forms.Padding(4);
            this.txtOutDir.Name = "txtOutDir";
            this.txtOutDir.Size = new System.Drawing.Size(300, 23);
            this.txtOutDir.TabIndex = 25;
            this.txtOutDir.Text = "C:\\Storage\\Repositories\\Onenote2md\\output";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 231);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 17);
            this.label5.TabIndex = 26;
            this.label5.Text = "Out Directory:";
            // 
            // btnGenerateSectionMd
            // 
            this.btnGenerateSectionMd.Location = new System.Drawing.Point(748, 54);
            this.btnGenerateSectionMd.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenerateSectionMd.Name = "btnGenerateSectionMd";
            this.btnGenerateSectionMd.Size = new System.Drawing.Size(152, 30);
            this.btnGenerateSectionMd.TabIndex = 27;
            this.btnGenerateSectionMd.Text = "GenerateSectionMd";
            this.btnGenerateSectionMd.UseVisualStyleBackColor = true;
            this.btnGenerateSectionMd.Click += new System.EventHandler(this.BtnGenerateSectionMd_Click);
            // 
            // btnGenerateNotebookMd
            // 
            this.btnGenerateNotebookMd.Location = new System.Drawing.Point(748, 12);
            this.btnGenerateNotebookMd.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenerateNotebookMd.Name = "btnGenerateNotebookMd";
            this.btnGenerateNotebookMd.Size = new System.Drawing.Size(152, 30);
            this.btnGenerateNotebookMd.TabIndex = 28;
            this.btnGenerateNotebookMd.Text = "GenerateNotebookMd";
            this.btnGenerateNotebookMd.UseVisualStyleBackColor = true;
            this.btnGenerateNotebookMd.Click += new System.EventHandler(this.BtnGenerateNotebookMd_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1438, 816);
            this.Controls.Add(this.btnGenerateNotebookMd);
            this.Controls.Add(this.btnGenerateSectionMd);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtOutDir);
            this.Controls.Add(this.btnGeneratePageMd);
            this.Controls.Add(this.btnCloseNotebook);
            this.Controls.Add(this.mdPreviewBox);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.btnGetPageContent);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.btnGetObject);
            this.Controls.Add(this.objectBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnGetPages);
            this.Controls.Add(this.sectionBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.logList);
            this.Controls.Add(this.btnGetSections);
            this.Controls.Add(this.notebookBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pageBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnGetChildObjectIDs);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "OneNoteParser.Tester";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGetChildObjectIDs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox pageBox;
        private System.Windows.Forms.TextBox notebookBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnGetSections;
        private System.Windows.Forms.ListBox logList;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button btnGetPages;
        private System.Windows.Forms.TextBox sectionBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox objectBox;
        private System.Windows.Forms.Button btnGetObject;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button btnGetPageContent;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.TextBox mdPreviewBox;
        private System.Windows.Forms.Button btnCloseNotebook;
        private System.Windows.Forms.Button btnGeneratePageMd;
        private System.Windows.Forms.TextBox txtOutDir;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnGenerateSectionMd;
        private System.Windows.Forms.Button btnGenerateNotebookMd;
    }
}

