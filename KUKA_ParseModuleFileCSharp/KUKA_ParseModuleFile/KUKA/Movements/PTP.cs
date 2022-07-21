using WarningHelper;
using ParseModuleFile.KUKA.DataTypes;
using ParseModuleFile.KUKA.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.Movements
{
    public class PTP : Movement
    {
        private string pdatName;
        private PDAT pdat;

        public string PDATName { get { return pdatName; } set { Set(ref pdatName, value); } }
        public PDAT PDAT { get { return pdat; } set { Set(ref pdat, value); } }

        public PTP(oldFold fold)
		{
            pdat = new PDAT();
			this.FDAT = new FDAT(-1, -1);
			//;FOLD PTP home1 Vel=100 % Pstart_end
			//;FOLD PTP p1 CONT Vel=100 % Pp1 Tool[1]:xGun1 Base[3]:010FX011_G11_LI

			if (fold.Application != null && fold.Application is Applications.Swp) {
				Applications.Swp x = (Applications.Swp) fold.Application;
				if (x.Action == SwpAction.Spotpoint) {
					PointName = "SWP" + x.SpotNo.ToString() + "_" + x.TypeId.ToString();
                    pdatName = "SWP" + x.SpotNo.ToString() + "_" + x.TypeId.ToString();
				}
			}

			if (string.IsNullOrEmpty(this.PointName))
				this.PointName = getReValue("PTP (\\w*)", fold.Name).ToUpperInvariant();
			if (this.PointName.StartsWith("HOME", StringComparison.OrdinalIgnoreCase)) {
				string x = this.PointName.Substring(4, 1);
				if (new List<string> {
					"1",
					"2",
					"3",
					"4",
					"5"
				}.Contains(x) && !int.TryParse(x, NumberStyles.Integer, CultureInfo.InvariantCulture, out base.homeNum))
					fold.Warnings.Add(fold.LineStart, WarningType.Program_Paths, Level.Failure, "Could not get home number", fold.Name);
			}
            if (string.IsNullOrEmpty(pdatName))
                this.pdatName = getReValue("% (\\w*)", fold.Name).ToUpperInvariant();
			if (fold.Name.Contains("Tool[") || fold.Name.Contains("TOOL["))
				this.FDAT.TOOL_NO = getReInteger("Tool\\[(\\d*)\\]", fold.Name, fold);
			if (fold.Name.Contains("Base[") || fold.Name.Contains("BASE["))
				this.FDAT.BASE_NO = getReInteger("Base\\[(\\d*)\\]", fold.Name, fold);
			this.PDAT.VEL = (int) getReDouble("Vel[=:]((?:[-+]?[0-9]*\\.?[0-9]+(?:[eE][-+]?[0-9]+)?)|).?%", fold.Name, fold);

			string pdat_s = "";
			string p_name_s = "";
			bool pdat_found = false;
			bool fdat_found = false;
			bool ptp_found = false;
			bool bas_found = false;
			foreach (string line in fold.Contents.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None)) {

				string aline = line.TrimStart();
                if (aline.StartsWith("PDAT_ACT"))
                {
						pdat_found = true;
						pdat_s = getReValue("PDAT_ACT\\s?=\\s?(.*)", aline).ToUpperInvariant();
                        if (!pdat_s.Equals("P" + pdatName))
							fold.Warnings.Add(fold.LineStart, WarningType.Program_Paths, Level.Failure, "Wrong PDAT name", this.PDATName + " (from fold declaration), " + pdat_s + " (in fold body) in " + this.PointName);
                        pdatName = pdat_s;
                }
                else if (aline.StartsWith("FDAT_ACT"))
                {
						fdat_found = true;
						this.FDATName = getReValue("FDAT_ACT\\s?=\\s?(.*)", aline).ToUpperInvariant();
                }
                else if (aline.StartsWith("PTP "))
                {
						ptp_found = true;
						p_name_s = getReValue("PTP\\s(\\w*)", aline).ToUpperInvariant();
						if (!p_name_s.Equals("X" + this.PointName))
							fold.Warnings.Add(fold.LineStart, WarningType.Program_Paths, Level.Failure, "Wrong point name", this.PointName + " (from fold declaration), " + p_name_s + " (in fold body)");
						this.PointName = p_name_s;
						Enums.Approximate_Positioning approx = Enums.Approximate_Positioning.NONE;
                    if(aline.Contains("C_DIS"))
                    {
								approx = Enums.Approximate_Positioning.C_DIS;
                    }
                    else if(aline.Contains("C_ORI"))
                    {
								approx = Enums.Approximate_Positioning.C_ORI;
                    }
                    else if(aline.Contains("C_VEL"))
                    {
								approx = Enums.Approximate_Positioning.C_VEL;
                    }
						if (this.Approx == Enums.Approximate_Positioning.NoNONE & approx == Enums.Approximate_Positioning.NONE)
							fold.Warnings.Add(fold.LineStart, WarningType.Program_Paths, Level.Failure, "Wrong CONT usage in " + this.PointName);
                }
                else if (aline.StartsWith("BAS"))
                {
						bas_found = true;
						double velocity = getReDouble("BAS\\s?\\(#PTP_PARAMS,\\s?((?:[-+]?[0-9]*\\.?[0-9]+(?:[eE][-+]?[0-9]+)?)|)\\)", aline, fold);
						if (!(velocity == this.PDAT.VEL))
							fold.Warnings.Add(fold.LineStart, WarningType.Program_Paths, Level.Failure, "Wrong velocity in fold delcaration: " + velocity, this.PointName);
						this.PDAT.VEL = (int)velocity;
                }
			}
			if (!pdat_found)
				fold.Warnings.Add(fold.LineStart, WarningType.Program_Paths, Level.Failure, "Did not found PDAT_ACT assigment in fold body", this.PointName);
			if (!fdat_found)
				fold.Warnings.Add(fold.LineStart, WarningType.Program_Paths, Level.Failure, "Did not found FDAT_ACT assigment in fold body", this.PointName);
			if (!ptp_found)
				fold.Warnings.Add(fold.LineStart, WarningType.Program_Paths, Level.Failure, "Did not found PTP instruction in fold body", this.PointName);
			if (!bas_found)
				fold.Warnings.Add(fold.LineStart, WarningType.Program_Paths, Level.Failure, "Did not found BAS instruction in fold body", this.PointName);
		}

        public override string ToString()
        {
            return "PTP " + PointName;
        }
    }
}
