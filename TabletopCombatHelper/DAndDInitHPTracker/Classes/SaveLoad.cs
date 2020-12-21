using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace DAndDInitHPTracker.Classes
{
    public class SaveLoad
    {
        SaveFileDialog _saveFileDialog = new SaveFileDialog();
        OpenFileDialog _openFileDialog = new OpenFileDialog();
        Stream _stream;

        public SaveLoad()
        {
            _saveFileDialog.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
            _saveFileDialog.FilterIndex = 0;
            _saveFileDialog.RestoreDirectory = true;

            _openFileDialog.InitialDirectory = "c:\\";
            _openFileDialog.Filter = "txt files (*.json)|*.json|All files (*.*)|*.*";
            _openFileDialog.FilterIndex = 0;
            _openFileDialog.RestoreDirectory = true;
        }

        public bool Save(string fileContent)
        {
            if (_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = File.CreateText(_saveFileDialog.FileName))
                {
                    sw.Write(fileContent);
                }
                return true;
            }
            return false;

        }

        public string Load()
        {

            if (_openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamReader sr = File.OpenText(_openFileDialog.FileName))
                {
                    string s;
                    while ((s = sr.ReadToEnd()) != null)
                    {
                        return s;
                    }
                }
            }
            return String.Empty;
        }
    }
}
