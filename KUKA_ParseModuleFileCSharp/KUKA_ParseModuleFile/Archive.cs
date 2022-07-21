using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;

namespace ParseModuleFile
{
    public class Archive : NotifyPropertyChanged
    {
        #region fields
        private KUKA.Robot robot;
        private List<string> excludeList;
        private List<string> includeList;
        private ObservableCollection<string> fileList;

        #endregion fields

        #region properties
        /// <summary>
        /// The robot object.
        /// </summary>
        public KUKA.Robot Robot { get { return robot; } set { Set(ref robot, value); } }
        /// <summary>
        /// List of excluded files.
        /// </summary>
        public List<string> ExcludeList { get { return excludeList; } }
        /// <summary>
        /// List of included files overiding list of exluded files.
        /// </summary>
        public List<string> IncludeList { get { return includeList; } }
        /// <summary>
        /// A list of scanned files.
        /// </summary>
        public ObservableCollection<string> FileList { get { return fileList; } }
        /// <summary>
        /// The path to the backup file of the robot.
        /// </summary>
        public string OriginalPath { get; private set; }

        #endregion properties

        #region constructors
        public Archive(string mypath, List<string> excludeList, List<string> includeList)
        {
            this.excludeList = excludeList;
            this.includeList = includeList;
            Robot = new KUKA.Robot();
            OriginalPath = mypath;
            fileList = new ObservableCollection<string>();
            Dictionary<string, string> data = new Dictionary<string, string>();
            if (System.IO.File.Exists(mypath))
            {
                data = OpenToMemory(Robot, mypath);
            }
        }

        #endregion constructors

        #region static methods
        #endregion static methods

        #region methods
        private Dictionary<string, string> OpenToMemory(KUKA.Robot robot, string path)
        {
            robot.Path = path;
            Dictionary<string, string> val = new Dictionary<string, string>();
            //try
            //{
            using (ZipArchive archive = ZipFile.OpenRead(path))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    bool high_prio = false;
                    bool visible = false;
                    if (ShouldCopyArchive(entry.FullName, ref high_prio, ref visible))
                    {
                        using (var stream = entry.Open())
                        {
                            robot.AddFile(entry.FullName.ToUpperInvariant(), entry.Name.ToUpperInvariant(), stream);
                        }
                        if (true)//visible
                        {
                            if (high_prio)
                            {
                                fileList.Insert(0, entry.FullName);
                            }
                            else
                            {
                                fileList.Add(entry.FullName);
                            }
                        }
                    }
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}

            return val;
        }

        private bool ShouldCopyArchive(string filename, ref bool high_prio, ref bool visible)
        {
            visible = true;
            // forced true
            high_prio = true;

            if (excludeList != null)
            {
                CultureInfo culture = new CultureInfo("en");
                foreach (string exclude in excludeList)
                    if (culture.CompareInfo.IndexOf(filename, exclude, CompareOptions.IgnoreCase) > -1)
                    //if (filename.Contains(exclude))
                    {
                        if (includeList != null)
                        {
                            bool includeItem = false;
                            foreach (string include in includeList)
                            {
                                if (culture.CompareInfo.IndexOf(filename, include, CompareOptions.IgnoreCase) > -1)
                                {
                                    includeItem = true;
                                    break;
                                }
                            }
                            if (!includeItem) return false;
                        }
                        else return false;
                    }
            }
            if (filename.EndsWith(".dat")) high_prio = true;
            else high_prio = false;
            return true;

            if (filename.EndsWith("KRC/R1/System/$config.dat", StringComparison.OrdinalIgnoreCase))
                return true;
            if (filename.EndsWith("KRC/R1/Mada/$machine.dat", StringComparison.OrdinalIgnoreCase))
                return true;
            if (filename.EndsWith("KRC/R1/BMW_App/A01_plc_User.dat", StringComparison.OrdinalIgnoreCase))
                return true;
            if (filename.EndsWith("KRC/R1/BMW_App/a03_grp.dat", StringComparison.OrdinalIgnoreCase))
                return true;
            if (filename.EndsWith("KRC/R1/BMW_App/a03_grp_user.dat", StringComparison.OrdinalIgnoreCase))
                return true;
            if (filename.EndsWith("KRC/R1/BMW_App/A04_swp_global.dat", StringComparison.OrdinalIgnoreCase))
                return true;
            if (filename.EndsWith("KRC/R1/BMW_App/A04_swp_user.dat", StringComparison.OrdinalIgnoreCase))
                return true;
            if (filename.EndsWith("KRC/R1/BMW_App/a02_tch.dat", StringComparison.OrdinalIgnoreCase))
                return true;
            if (filename.EndsWith("KRC/R1/BMW_App/a02_tch_global.dat", StringComparison.OrdinalIgnoreCase))
                return true;
            if (filename.EndsWith("KRC/R1/BMW_App/a02_tch_user.dat", StringComparison.OrdinalIgnoreCase))
                return true;
            if (filename.EndsWith("ArchiveInfo.xml", StringComparison.OrdinalIgnoreCase))
                return true;

            high_prio = false;
            // FALSE
            if (filename.StartsWith("c/krc", StringComparison.OrdinalIgnoreCase))
                return false;
            if (filename.StartsWith("krc/r1/tp", StringComparison.OrdinalIgnoreCase))
                return false;
            if (filename.StartsWith("KRC/STEU/", StringComparison.OrdinalIgnoreCase))
                return false;
            if (filename.StartsWith("krc/r1/system", StringComparison.OrdinalIgnoreCase))
                return false;
            if (filename.StartsWith("krc/r1/bmw_init", StringComparison.OrdinalIgnoreCase))
                return false;
            if (filename.StartsWith("krc/r1/bmw_app", StringComparison.OrdinalIgnoreCase))
                return false;
            if (filename.EndsWith("krc/r1/bmw_utilities/application_ini.src", StringComparison.OrdinalIgnoreCase))
                return false;
            if (filename.EndsWith("KRC/R1/BMW_Utilities/Appl_OnActivate.src", StringComparison.OrdinalIgnoreCase))
                return false;
            if (filename.EndsWith("KRC/R1/BMW_Utilities/Appl_stopm_ini.src", StringComparison.OrdinalIgnoreCase))
                return false;
            if (filename.EndsWith("KRC/R1/BMW_Utilities/Appl_stopm_restart.src", StringComparison.OrdinalIgnoreCase))
                return false;
            if (filename.EndsWith("KRC/R1/BMW_Utilities/Appl_stopm_stop.src", StringComparison.OrdinalIgnoreCase))
                return false;
            if (filename.EndsWith("KRC/R1/BMW_Utilities/masref.src", StringComparison.OrdinalIgnoreCase))
                return false;
            if (filename.EndsWith("KRC/R1/BMW_Utilities/masref_bmw.src", StringComparison.OrdinalIgnoreCase))
                return false;
            if (filename.EndsWith("KRC/R1/Program/masref_user.src", StringComparison.OrdinalIgnoreCase))
                return false;
            if (filename.EndsWith("KRC/R1/Program/tm_useraction.src", StringComparison.OrdinalIgnoreCase))
                return false;
            if (filename.Contains("select_abortprog"))
                return false;
            if (filename.Contains("a03_grp_special"))
                return false;

            // TRUE
            if (filename.EndsWith(".src", StringComparison.OrdinalIgnoreCase))
                return true;

            visible = false;
            if (filename.EndsWith(".dat", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        #endregion methods
    }
}
