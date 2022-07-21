using WarningHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParseModuleFile.KUKA.DataTypes;
using System.IO;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;
using Antlr4.Runtime;

namespace ParseModuleFile.KUKA
{
    public class Robot : MyNotifyPropertyChanged
    {
        #region fields
        private Programs programs = new Programs();
        private ArchiveInfo.ArchiveInfo archiveInfo;
        private string path;
        private string currentFile;

        private VariablesContainer m_VariableContainer = new VariablesContainer();
        private DataTypesContainer m_DataTypes = new DataTypesContainer();
        #endregion // fields

        #region properties
        public IList<IToken> foldTokens { get; set; }
        /// <summary>
        /// The path to the backup file of the robot.
        /// </summary>
        public string Path { get { return path; } set { Set(ref path, value); } }
        /// <summary>
        /// List of programs.
        /// </summary>
        public Programs Programs { get { return programs; } set { Set(ref programs, value); } }
        /// <summary>
        /// Basic backup information.
        /// </summary>
        public ArchiveInfo.ArchiveInfo ArchiveInfo { get { return archiveInfo; } set { Set(ref archiveInfo, value); } }
        /// <summary>
        /// List of variables.
        /// </summary>
        public VariablesContainer VariableContainer { get { return m_VariableContainer; } set { Set(ref m_VariableContainer, value); } }
        /// <summary>
        /// List of datatypes.
        /// </summary>
        public DataTypesContainer DataTypes { get { return m_DataTypes; } set { Set(ref m_DataTypes, value); } }
        #endregion // properties

        #region constructors
        public Robot()
        {
            ArchiveInfo = new ArchiveInfo.ArchiveInfo();
            foldTokens = new List<IToken>();
        }

            //if (_appl.A01_Plc && Homes != null)
            //{
            //    for (int i = 1; i <= Homes.Count; i++)
            //    {
            //        if (Homes.ContainsKey(i) & Program.GlobalE6Axis.ContainsKey("XHOME" + i.ToString()))
            //        {
            //            Home home = Homes[i];
            //            home.Position = Program.GlobalE6Axis["XHOME" + i.ToString()];
            //            Homes[i] = home;
            //        }
            //    }
            //}
        #endregion // constructors

        #region methods

        public void AddFile(string fullName, string fileName, Stream stream)
        {
            this.currentFile = fullName;
            Warnings warnings = new Warnings();
            switch (System.IO.Path.GetExtension(fileName))
            {
                case ".DAT":
                    AntlrInputStream input = new AntlrInputStream(stream);
                    ANTLR.kukaLexer lexer = new ANTLR.kukaLexer(input);
                    CommonTokenStream tokens = new CommonTokenStream(lexer);
                    ANTLR.kukaParser parser = new ANTLR.kukaParser(tokens);
                    parser.RemoveErrorListeners();
                    parser.AddErrorListener(new ANTLR.ErrorListener(Path,fileName));
                    ANTLR.kukaVisitor visitor = new ANTLR.kukaVisitor();
                    visitor.Visit(parser.prog(),this);
                    if (m_VariableContainer.ContainsKey(visitor.Module))
                    {
                        m_VariableContainer[visitor.Module].Items.AddRange(visitor.DataList.Items);
                    }
                    else
                        m_VariableContainer.Add(visitor.Module, visitor.DataList);
                    break;
                case ".SRC":
/*                    Parser p = new Parser(fileName, stream, warnings);
                    if (p.Src.Folds != null)
                    {
                        Program prg = new Program(fileName, p, variables, warnings);
                        prg.ReadToEnd();
                        programs.Add(prg);
                    }*/
                    
                    AntlrInputStream srcInput = new AntlrInputStream(stream);
                    ANTLR.kukaSrcLexer srcLexer = new ANTLR.kukaSrcLexer(srcInput);
                    CommonTokenStream srcFoldTokens = new CommonTokenStream(srcLexer, ANTLR.kukaSrcLexer.FOLDS);
                    while (srcFoldTokens.La(1) != ANTLR.kukaSrcLexer.Eof)
                    {
                        foldTokens.Add(srcFoldTokens.Lt(1));
                        srcFoldTokens.Consume();
                    }
                    srcLexer.Reset();
                    CommonTokenStream srcTokens = new CommonTokenStream(srcLexer);
                    ANTLR.kukaSrcParser srcParser = new ANTLR.kukaSrcParser(srcTokens);
                    srcParser.RemoveErrorListeners();
                    srcParser.AddErrorListener(new ANTLR.ErrorListener(Path, fileName));
                    ANTLR.kukaSrcVisitor srcVisitor = new ANTLR.kukaSrcVisitor();
                    programs.Add ((ANTLR.SrcParser.ProcedureDefinition)srcVisitor.Visit(srcParser.mainRoutine()));
                    
                    break;
                case ".OLP":
                    break;
                case ".XML":
                    if (System.IO.Path.GetFileName(fileName).ToUpperInvariant() == "ARCHIVEINFO.XML")
                        using (var reader = new StreamReader(stream))
                        {
                            ArchiveInfo = new ArchiveInfo.ArchiveInfo(reader.ReadToEnd(), false);
                        }
                    break;
            }
            //Program prg = new Program(act_file);


            //// Load programs to robot
            //if (prg.Parser.Src.Folds != null | prg.Warnings.Count > 0)
            //{
            //    prg.ReadToEnd();
            //    Programs.Add(prg);
            //}
        }

        public override string ToString()
        {
            if (ArchiveInfo != null)
                return ArchiveInfo.Name;
            return "unknown";
        }

        #endregion // methods
    }
}
