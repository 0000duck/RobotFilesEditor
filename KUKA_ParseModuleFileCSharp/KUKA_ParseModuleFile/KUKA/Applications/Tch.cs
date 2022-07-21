using WarningHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.Applications
{
    public class Tch : Application
    {


        public override void Enumerate(oldFolds list, ProgramBaseInfo _baseinfo)
        {
        }

        public override void Test(oldFolds list, ProgramBaseInfo _baseinfo)
        {
            int i = 0;
            foreach (oldFold item in list)
            {
                if (item.Application == null)
                    continue;
                if (!(item.Application is Tch))
                    continue;
                i += 1;
            }

            if (i > 0)
                _warnings.Add(-2, WarningType.Intern, Level.Failure, "Toolchanging not yet functional...");
        }

        public Tch(Warnings warnings)
        {
            _warnings = warnings;
        }

        public Tch(oldFold fold)
        {
            _warnings = fold.Warnings;
        }

        public override string ToString()
        {
            return "TCh";
        }
    }
}
