using ParseModuleFile.ANTLR.SrcParser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA
{
    public class Programs : ObservableCollection<ProcedureDefinition>
    {

        public ProcedureDefinition this[string key]
        {
            get
            {
                foreach (ANTLR.SrcParser.ProcedureDefinition prg in this)
                {
                    if (key.Equals(prg.Name))
                        return prg;
                }
                return null;
            }
        }

        public bool HasProgram(string ProgramName)
        {
            foreach (ProcedureDefinition prg in this)
            {
                if (ProgramName.Equals(prg.Name))
                    return true;
            }
            return false;
        }

        public ProcedureDefinition ByName(string ProgramName)
        {
            foreach (ProcedureDefinition prg in this)
            {
                if (ProgramName.Equals(prg.Name))
                    return prg;
            }
            return null;
        }

    }
}
