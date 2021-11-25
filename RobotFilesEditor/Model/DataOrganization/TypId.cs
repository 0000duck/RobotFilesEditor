namespace RobotFilesEditor.Model.DataOrganization
{
    public class TypId
    {
        public string OldTypIds { get; set; }
        public string NewTypIds { get; set; }

        public TypId(string old)
        {
            OldTypIds = old;
            NewTypIds = "";
        }
    }
}