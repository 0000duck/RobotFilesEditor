using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RobotFilesEditor.Model.Operations.FANUC
{
    public class FanucValidateCommentsMethods : CommonLibrary.VirtualAndAbstractMethods
    {
        public FanucValidateCommentsMethods()
        {
            Execute();
        }

        public override void Execute()
        {
            try
            {
                MessageBox.Show("Select directory with OLP files.", "Select dir", MessageBoxButton.OK, MessageBoxImage.Information);
                string dir = CommonLibrary.CommonMethods.SelectDirOrFile(true);
                if (string.IsNullOrEmpty(dir))
                    return;
                List<string> files = Directory.GetFiles(dir, "*.ls", SearchOption.AllDirectories).ToList();
                var fileCommentValidator = new FanucFilesValidator(files);
                MessageBox.Show("Select directory to save files.", "Select dir", MessageBoxButton.OK, MessageBoxImage.Information);
                string dirToSave = CommonLibrary.CommonMethods.SelectDirOrFile(true);
                if (string.IsNullOrEmpty(dirToSave))
                    return;
                if (!Directory.Exists(Path.Combine(dirToSave, "CommentFixedOLP")))
                    Directory.CreateDirectory(Path.Combine(dirToSave, "CommentFixedOLP"));
                foreach (var file in fileCommentValidator.FilesAndContent)
                {
                    var fileStr = File.Create(Path.Combine(dirToSave, "CommentFixedOLP", Path.GetFileName(file.Key)));
                    fileStr.Close();
                    File.WriteAllText(Path.Combine(dirToSave, "CommentFixedOLP", Path.GetFileName(file.Key)), GetFileContenetFANUC(file.Value));
                }
                var dialogSuccess = System.Windows.Forms.MessageBox.Show("Successfuly saved at: " + Path.Combine(dirToSave, "CommentFixedOLP") + ".\r\nWould you like to open directory?", "Success", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                if (dialogSuccess == System.Windows.Forms.DialogResult.Yes)
                    Process.Start(Path.Combine(dirToSave, "CommentFixedOLP"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetFileContenetFANUC(FanucRobotPath files)
        {
            string result = string.Empty;
            foreach (var line in files.InitialSection)
                result += line + "\r\n";
            foreach (var line in files.ProgramSection)
                result += line + "\r\n";
            foreach (var line in files.DeclarationSection)
                result += line + "\r\n";
            return result;
        }

    }
}
