using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace Notey
{
    public partial class Notey : Form
    {
        private const string Version = "0.00"; // The current version number of the software.
        
        private const string UpdatePath = "https://www.ollierobinson.co.uk/Notey/UpdateInfo.json";
        private const string DownloadPath = "https://github.com/ohtrobinson/Notey";
        
        private string _filePath = String.Empty; // The full path of the file (C:\Users\user\Documents\untitled.not)
        private string _fileName = String.Empty; // The name of the file (untitled.not)

        private bool _fileEdited = false; // Has the file been edited?

        private string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                Text = (_fileEdited ? "*" : "") + value + " - Notey"; // Set the title of notey.
            }
        }

        #region Reusable Methods

        private void ResizeTextBox() // Resize the text box so that it fits correctly.
        {
            textBox.Height = ClientSize.Height - toolBar.Height;
        }

        private void Save(string filePath = null) // Save a file.
        {
            if (string.IsNullOrWhiteSpace(filePath)) // If there is no file path provided, open the save file dialog.
            {
                SaveFileDialog saveFileDialog = new() // Create a new save dialog.
                {
                    Filter = "Notey documents (*.not)|*.not|Rich-text format documents (*.rtf)|*.rtf|Text files (*.txt)|*.txt|All files (*.*)|*.*"
                };
                saveFileDialog.ShowDialog();
                if (saveFileDialog.FileName != "")
                {
                    _filePath = saveFileDialog.FileName;
                    _fileEdited = false;
                    FileName = _filePath.Split(@"\").Last();
                    textBox.SaveFile(saveFileDialog.FileName, saveFileDialog.FileName.EndsWith(".not") || saveFileDialog.FileName.EndsWith(".rtf") ? RichTextBoxStreamType.RichText : RichTextBoxStreamType.PlainText);
                }
            }
            else
            {
                _filePath = filePath;
                _fileEdited = false;
                FileName = _fileName;
                textBox.SaveFile(filePath, filePath.EndsWith(".not") || filePath.EndsWith(".rtf") ? RichTextBoxStreamType.RichText : RichTextBoxStreamType.PlainText);
            }
        }

        private void Open(string filePath = null)
        {
            if (_fileEdited)
            {
                DialogResult result = SaveChangesPrompt();
                if (result == DialogResult.Cancel) return;
                if (result == DialogResult.Yes) Save(_filePath);
            }
            if (String.IsNullOrWhiteSpace(filePath))
            {
                OpenFileDialog openFileDialog = new()
                {
                    Filter = "Openable files (*.not;*.rtf;*.txt)|*.not;*.rtf;*.txt|(Notey documents (*.not)|*.not|Rich-text format documents (*.rtf)|*.rtf|Text files (*.txt)|*.txt|All files (*.*)|*.*"
                };
                openFileDialog.ShowDialog();
                if (openFileDialog.FileName != "")
                {
                    textBox.LoadFile(openFileDialog.FileName, openFileDialog.FileName.EndsWith(".not") || openFileDialog.FileName.EndsWith(".rtf") ? RichTextBoxStreamType.RichText : RichTextBoxStreamType.PlainText);
                    _filePath = openFileDialog.FileName;
                    _fileEdited = false;
                    FileName = openFileDialog.SafeFileName;
                }
            }
            else
            {
                textBox.LoadFile(_filePath, _filePath.EndsWith(".not") || _filePath.EndsWith(".rtf") ? RichTextBoxStreamType.RichText : RichTextBoxStreamType.PlainText);
                _filePath = filePath;
                _fileEdited = false;
                FileName = _filePath.Split(@"\").Last();
            }
        }

        private DialogResult SaveChangesPrompt()
        {
            DialogResult result = MessageBox.Show($"Would you like to save changes to {_fileName}?", "Notey",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            return result;
        }

        private async Task CheckForUpdates() // Check for updates.
        {
            try
            {
                HttpClient client = new();
                string content = await client.GetStringAsync(UpdatePath); // Look for this website.
                string version = (string) JObject.Parse(content)["version"];
                if (version != Version)
                {
                    DialogResult result =
                        MessageBox.Show("An update is available for Notey. Would you like to download it?",
                            "Update Available!",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            Process.Start(new ProcessStartInfo("cmd", $"/c start {DownloadPath}")
                                {CreateNoWindow = true});
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString(), "An error has occured.");
                        }
                    }

                    updateButton.Visible = true;
                }
            }
            catch (Exception) { }
        }
        
        #endregion
        public Notey(string[] args)
        {
            InitializeComponent();
            FileName = "Untitled";
            if (args.Length > 0) Open(args[0]);
        }

        private void Notey_Load(object sender, EventArgs e) // When Notey loads, set the text box to the correct size.
        {
            ResizeTextBox();
        }
        
        private void Notey_Shown(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        private void Notey_Resize(object sender, EventArgs e) // When Notey is resized, set the text box to the new correct size.
        {
            ResizeTextBox();
        }

        private void fileSaveAs_Click(object sender, EventArgs e) // When the save as button is clicked
        {
            Save();
        }

        private void fileSave_Click(object sender, EventArgs e) // When the save button is clicked
        {
            Save(_filePath);
        }

        private void fileOpen_Click(object sender, EventArgs e) // When the open button is clicked
        {
            Open();
        }

        private void textBox_TextChanged(object sender, EventArgs e) // Run whenever the text box text is changed.
        {
            if (_fileEdited) return;
            _fileEdited = true;
            FileName = _fileName;
        }

        private void Notey_FormClosing(object sender, FormClosingEventArgs e) // Run whenever the form is closing.
        {
            if (!_fileEdited) return; // If the file has not been edited, just close the form.
            DialogResult result = SaveChangesPrompt(); // Otherwise, send a prompt asking if the user wants to save changes.
            if (result == DialogResult.Cancel) e.Cancel = true;
            if (result == DialogResult.Yes) Save();
        }

        private void fileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Notey_KeyDown(object sender, KeyEventArgs e) // Deals with the keypresses
        {
            if ((e.Modifiers & Keys.Control) != 0) // This section only responds to the ctrl key.
            {
                switch (e.KeyCode)
                {
                    case Keys.S: // Save
                        Save(_filePath);
                        break;
                    case Keys.O: // Open
                        Open();
                        break;
                }
            }
        }

        private void formatWordWrap_CheckedChanged(object sender, EventArgs e)
        {
            textBox.WordWrap = formatWordWrap.Checked;
        }

        private void formatFont_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new()
            {
                Font = textBox.Font,
                ShowEffects = false
            };
            if (fontDialog.ShowDialog() != DialogResult.Cancel)
            {
                textBox.Font = fontDialog.Font;
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            CheckForUpdates();
        }
    }
}