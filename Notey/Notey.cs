using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Notey
{
    public partial class Notey : Form
    {
        private string _filePath = String.Empty; // The full path of the file (C:\Users\user\Documents\untitled.not)
        private string _fileName = String.Empty; // The name of the file (untitled.not)

        private bool _fileEdited = false; // Has the file been edited?

        private string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                Text = (_fileEdited ? "*" : "") + value + " - Notey";
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
                    Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
                };
                saveFileDialog.ShowDialog();
                if (saveFileDialog.FileName != "")
                {
                    _filePath = saveFileDialog.FileName;
                    _fileEdited = false;
                    FileName = saveFileDialog.FileName;
                    textBox.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.PlainText);
                }
            }
            else
            {
                _filePath = filePath;
                _fileEdited = false;
                FileName = filePath;
                textBox.SaveFile(filePath, RichTextBoxStreamType.PlainText);
            }
        }

        private void Open(string filePath = null)
        {
            if (String.IsNullOrWhiteSpace(filePath))
            {
                OpenFileDialog openFileDialog = new()
                {
                    Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
                };
                openFileDialog.ShowDialog();
                if (openFileDialog.FileName != "")
                {
                    _filePath = openFileDialog.FileName;
                    _fileEdited = false;
                    FileName = openFileDialog.SafeFileName;
                    textBox.LoadFile(openFileDialog.FileName, RichTextBoxStreamType.PlainText);
                }
            }
            else
            {
                _filePath = filePath;
                _fileEdited = false;
                FileName = filePath;
                textBox.LoadFile(filePath, RichTextBoxStreamType.PlainText);
            }
        }
        
        #endregion
        public Notey(string[] args)
        {
            InitializeComponent();
        }

        private void Notey_Load(object sender, EventArgs e) // When Notey loads, set the text box to the correct size.
        {
            ResizeTextBox();
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
    }
}