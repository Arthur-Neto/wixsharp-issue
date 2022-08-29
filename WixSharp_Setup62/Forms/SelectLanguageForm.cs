using System.Globalization;
using System.Windows.Forms;

namespace WixSharp_Setup62.Forms
{
    public class SelectLanguageForm : Form
    {
        public CultureInfo SelectedCulture { get; set; }

        private ComboBox cbLanguages;
        private Label lblSelectLanguage;
        private Button btnOK;

        public SelectLanguageForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            cbLanguages = new ComboBox();
            btnOK = new Button();
            lblSelectLanguage = new Label();
            SuspendLayout();
            // 
            // cbLanguages
            // 
            cbLanguages.FormattingEnabled = true;
            cbLanguages.Location = new System.Drawing.Point(86, 51);
            cbLanguages.Name = "cbLanguages";
            cbLanguages.Size = new System.Drawing.Size(197, 21);
            cbLanguages.TabIndex = 0;
            // 
            // btnOK
            // 
            btnOK.Location = new System.Drawing.Point(209, 78);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(75, 23);
            btnOK.TabIndex = 2;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += new System.EventHandler(btnOK_Click);
            // 
            // lblSelectLanguage
            // 
            lblSelectLanguage.AutoSize = true;
            lblSelectLanguage.Location = new System.Drawing.Point(78, 25);
            lblSelectLanguage.Name = "lblSelectLanguage";
            lblSelectLanguage.Size = new System.Drawing.Size(205, 13);
            lblSelectLanguage.TabIndex = 3;
            lblSelectLanguage.Text = "Select Language:";
            // 
            // SelectLanguageForm
            // 
            AcceptButton = btnOK;
            ClientSize = new System.Drawing.Size(296, 112);
            Controls.Add(lblSelectLanguage);
            Controls.Add(btnOK);
            Controls.Add(cbLanguages);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "SelectLanguageForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Select Language";
            Load += new System.EventHandler(SelectLanguageForm_Load);
            ResumeLayout(false);
            PerformLayout();

        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            SelectedCulture = (CultureInfo)cbLanguages.SelectedItem;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void SelectLanguageForm_Load(object sender, System.EventArgs e)
        {
            cbLanguages.Items.Add(Program.DefaultLanguage);
            foreach (var supportedLanguage in Program.SupportedLanguages)
            {
                cbLanguages.Items.Add(supportedLanguage);
            }
            cbLanguages.DisplayMember = nameof(CultureInfo.DisplayName);
            cbLanguages.SelectedIndex = 0;
        }
    }
}
