using WarningHelper;
using ParseModuleFile.KUKA.DataTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.Movements
{

    namespace KUKA.Movements
    {
        public class LIN : Movement
        {
            private string ldatName;
            private LDAT ldat;
            public string LDATName { get { return ldatName; } set { Set(ref ldatName, value); } }
            public LDAT LDAT { get { return ldat; } set { Set(ref ldat, value); } }

            public LIN(oldFold fold)
            {
                this.LDAT = new LDAT();
                this.FDAT = new FDAT(-1, -1);
                //LIN g_beforeTipChgPos1G1 CONT Vel=0.5 m/s Lg_beforeTipChgPos1G1 Tool[1]:cGun_1 Base[31]:Tip_Dresser_1;%{PE}%R 8.3.21,%MKUKATPBASIS,%CMOVE,%VLIN,%P 1:LIN, 2:g_beforeTipChgPos1G1, 3:C_DIS C_DIS, 5:0.5, 7:Lg_beforeTipChgPos1G1
                //$BWDSTART = FALSE
                //LDAT_ACT = LLg_beforeTipChgPos1G1
                //FDAT_ACT = Fg_beforeTipChgPos1G1
                //BAS(#CP_PARAMS,0.5)
                //LIN Xg_beforeTipChgPos1G1 C_DIS C_DIS

                if (fold.Application != null)
                {
                    if (fold.Application is Applications.Swp)
                    {
                        Applications.Swp x = (Applications.Swp)fold.Application;
                        if (x.Action == Enums.SwpAction.Spotpoint)
                        {
                            this.PointName = "SWP" + x.SpotNo.ToString() + "_" + x.TypeId.ToString();
                            ldatName = "SWP" + x.SpotNo.ToString() + "_" + x.TypeId.ToString();
                        }
                    }
                }
                if (string.IsNullOrEmpty(this.PointName))
                    this.PointName = getReValue("LIN (\\w*)", fold.Name).ToUpperInvariant();
                if (this.PointName.StartsWith("HOME", StringComparison.OrdinalIgnoreCase))
                {
                    string x = fold.Name.Substring(8, 1);
                    if (new List<string> {
					"1",
					"2",
					"3",
					"4",
					"5"
				}.Contains(x) && !int.TryParse(x, NumberStyles.Integer, CultureInfo.InvariantCulture, out base.homeNum))
                        fold.Warnings.Add(fold.LineStart, WarningType.Program_Paths, Level.Failure, "Could not get home number", fold.Name);
                }
                if (string.IsNullOrEmpty(ldatName))
                {
                    string s = getReValue("m/s (\\w*)", fold.Name);
                    if (s != null)
                        ldatName = s.ToUpperInvariant();
                }
                if (fold.Name.Contains("Tool[") || fold.Name.Contains("TOOL["))
                    this.FDAT.TOOL_NO = getReInteger("Tool\\[(\\d*)\\]", fold.Name, fold);
                if (fold.Name.Contains("Base[") || fold.Name.Contains("BASE["))
                    this.FDAT.BASE_NO = getReInteger("Base\\[(\\d*)\\]", fold.Name, fold);
                this.LDAT.VEL = getReDouble("Vel[=:]((?:[-+]?[0-9]*\\.?[0-9]+(?:[eE][-+]?[0-9]+)?)|).?m/s", fold.Name, fold);
                this.Approx = fold.Name.Contains(" CONT ") ? Enums.Approximate_Positioning.NoNONE : Enums.Approximate_Positioning.NONE;

                string ldat_s = "";
                string p_name_s = "";
                bool ldat_found = false;
                bool fdat_found = false;
                bool lin_found = false;
                bool bas_found = false;
                foreach (string line in fold.Contents.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                {
                    string aline = line.TrimStart();
                    if (aline.StartsWith("LDAT_ACT"))
                    {
                        ldat_found = true;
                        ldat_s = getReValue("LDAT_ACT\\s?=\\s?(.*)", aline).ToUpperInvariant();
                        if (!ldat_s.Equals("L" + ldatName))
                            fold.Warnings.Add(fold.LineStart, WarningType.Program_Paths, Level.Failure, "Wrong LDAT name", ldatName + " (from fold declaration), " + ldat_s + " (in fold body) in " + this.PointName);
                        ldatName = ldat_s;
                    }
                    else if (aline.StartsWith("FDAT_ACT"))
                    {
                        fdat_found = true;
                        this.FDATName = getReValue("FDAT_ACT\\s?=\\s?(.*)", aline).ToUpperInvariant();
                    }
                    else if (aline.StartsWith("LIN"))
                    {
                        lin_found = true;
                        p_name_s = getReValue("LIN\\s(\\w*)", aline).ToUpperInvariant();
                        if (!p_name_s.Equals("X" + this.PointName))
                            fold.Warnings.Add(fold.LineStart, WarningType.Program_Paths, Level.Failure, "Wrong point name", this.PointName + " (from fold declaration), " + p_name_s + " (in fold body)");
                        this.PointName = p_name_s;
                        Enums.Approximate_Positioning approx = Enums.Approximate_Positioning.NONE;
                        if (aline.Contains("C_DIS"))
                        {
                            approx = Enums.Approximate_Positioning.C_DIS;
                        }
                        else if (aline.Contains("C_ORI"))
                        {
                            approx = Enums.Approximate_Positioning.C_ORI;
                        }
                        else if (aline.Contains("C_VEL"))
                        {
                            approx = Enums.Approximate_Positioning.C_VEL;
                        }
                        if (this.Approx == Enums.Approximate_Positioning.NoNONE & approx == Enums.Approximate_Positioning.NONE)
                            fold.Warnings.Add(fold.LineStart, WarningType.Program_Paths, Level.Failure, "Wrong CONT usage in " + this.PointName);
                        this.Approx = approx;
                    }
                    else if (aline.StartsWith("BAS"))
                    {
                        bas_found = true;
                        double velocity = getReDouble("BAS\\s?\\(#CP_PARAMS,\\s?((?:[-+]?[0-9]*\\.?[0-9]+(?:[eE][-+]?[0-9]+)?)|)\\)", line, fold);
                        if (!(velocity == this.LDAT.VEL))
                            fold.Warnings.Add(fold.LineStart, WarningType.Program_Paths, Level.Failure, "Wrong velocity in fold delcaration: " + velocity, this.PointName);
                        this.LDAT.VEL = velocity;
                    }
                }
                if (!ldat_found)
                    fold.Warnings.Add(fold.LineStart, WarningType.Program_Paths, Level.Failure, "Did not found LDAT_ACT assigment in fold body", this.PointName);
                if (!fdat_found)
                    fold.Warnings.Add(fold.LineStart, WarningType.Program_Paths, Level.Failure, "Did not found FDAT_ACT assigment in fold body", this.PointName);
                if (!lin_found)
                    fold.Warnings.Add(fold.LineStart, WarningType.Program_Paths, Level.Failure, "Did not found LIN instruction in fold body", this.PointName);
                if (!bas_found)
                    fold.Warnings.Add(fold.LineStart, WarningType.Program_Paths, Level.Failure, "Did not found BAS instruction in fold body", this.PointName);
            }

            public override string ToString()
            {
                return "LIN " + PointName;
            }
        }
    }
}
