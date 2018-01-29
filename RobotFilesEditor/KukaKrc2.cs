using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RobotFilesEditor
{
    public class KukaKrc2 
    {
        //private FilesFilter _productionCopiedFiles;
        //private FilesFilter _serviceCopiedFiles;
        //private FilesFilter _copiedOlpDataFiles;
        //private FilesFilter _copiedGlobalDataFiles;
        //private FilesFilter _removingDataFiles;

        //private string _destinationPath;
        //private string _sourcePath;
        //private string _controlerFolder;

        //public KukaKrc2(string sourcePath, string destinationPath)
        //{
        //    _productionCopiedFiles = new FilesFilter();
        //    _serviceCopiedFiles = new FilesFilter();
        //    _copiedOlpDataFiles = new FilesFilter();
        //    _copiedGlobalDataFiles = new FilesFilter();
        //    _removingDataFiles = new FilesFilter();
            
        //    _controlerFolder = @"KRC2" + DateTime.Now.ToString("_yyyyMMdd");

        //    if (destinationPath.Contains(_controlerFolder))
        //    {
        //        _destinationPath = destinationPath;
        //    }
        //    else
        //    {
        //        _destinationPath = Path.Combine(destinationPath, _controlerFolder);
        //        Directory.CreateDirectory(_destinationPath);
        //    }

        //    _sourcePath = sourcePath;        
        //}

        //#region Configuration

        //public void LoadConfigurationSettingsForControler()
        //{
        //    var fs = new Serializer.FilesSerialization();
        //    SingleControler controler=fs.GetControlerConfigration("KRC2");

        //    _productionCopiedFiles =controler._productionCopiedFiles;
        //    _serviceCopiedFiles = controler._serviceCopiedFiles;
        //    _copiedOlpDataFiles = controler._copiedOlpDataFiles;
        //    _copiedGlobalDataFiles = controler._copiedGlobalDataFiles;
        //    _removingDataFiles = controler._removingDataFiles;
        //}

        //public List<string>GetGroupedFiles()
        //{
        //    List<string> gropuedFiles = new List<string>();
        //    List<string> extensions = new List<string>();

        //    extensions.AddRange(_productionCopiedFiles?.FileExtensions);   
        //    extensions.AddRange(_serviceCopiedFiles?.FileExtensions);
        //    extensions.AddRange(_copiedOlpDataFiles?.FileExtensions);
        //    extensions.AddRange(_copiedGlobalDataFiles?.FileExtensions);
        //    extensions.AddRange(_removingDataFiles?.FileExtensions);
           
        //    extensions = extensions.Distinct().ToList();

        //    foreach(string e in extensions)
        //    {
        //        List<string>files = Directory.GetFiles(_sourcePath, $"*{e}").ToList();

        //        files=files.ConvertAll(x =>x=Path.GetFileName(x));

        //        gropuedFiles.Add(e);
        //        gropuedFiles.AddRange(files.OrderBy(x=>x));
        //        gropuedFiles.Add("");
        //    }

        //    gropuedFiles.Distinct();

        //    return gropuedFiles;
        //}

        //public void CreateDestinationFolders()
        //{
        //    if (Directory.Exists(_destinationPath) == false)
        //    {
        //        var result = Directory.CreateDirectory(_destinationPath);
        //    }

        //    if (_productionCopiedFiles?.DestinationFolder != null)
        //    {
        //        Directory.CreateDirectory(Path.Combine(_destinationPath, _productionCopiedFiles.DestinationFolder));
        //    }

        //    if (_serviceCopiedFiles?.DestinationFolder != null)
        //    {
        //        Directory.CreateDirectory(Path.Combine(_destinationPath, _productionCopiedFiles.DestinationFolder));
        //    }

        //    if (_serviceCopiedFiles?.DestinationFolder != null)
        //    {
        //        Directory.CreateDirectory(Path.Combine(_destinationPath, _productionCopiedFiles.DestinationFolder));
        //    }

        //    if (_copiedOlpDataFiles?.DestinationFolder != null)
        //    {
        //        Directory.CreateDirectory(Path.Combine(_destinationPath, _productionCopiedFiles.DestinationFolder));
        //    }

        //    if (_copiedGlobalDataFiles?.DestinationFolder != null)
        //    {
        //        Directory.CreateDirectory(Path.Combine(_destinationPath, _productionCopiedFiles.DestinationFolder));
        //    }
        //}

        //public void RefreshDestinationPath(string path)
        //{
        //    if (Directory.Exists(path))
        //    {
        //        if (path.Contains(_controlerFolder))
        //        {
        //            _destinationPath = path;
        //        }
        //        else
        //        {
        //            _destinationPath = Path.Combine(path, _controlerFolder);
        //        }
        //    }         
        //}

        //public void RefreshSourcePath(string path)
        //{
        //    if(Directory.Exists(path))
        //    {
        //        _sourcePath = path;
        //    }

        //}
        
        //#endregion Configuration


        //#region CopyFiles
        //public void MoveProductionFiles()
        //{
        //    string []allFilesAtSourcePath= Directory.GetFiles(_sourcePath);
        //    List<string> movingFiles = new List<string>();
        //    string sPattern = @"^([a-zA-Z0-9]+_){2}[a-zA-Z0-9]+\.[a-z]+";

        //    if (Directory.Exists(_sourcePath)==false || Directory.Exists(_destinationPath)==false)
        //    {
        //        throw new DirectoryNotFoundException();
        //    }

        //    if (string.IsNullOrEmpty(_productionCopiedFiles.DestinationFolder))
        //    {
        //        throw new NullReferenceException("Production Copied FilesFilters destination directory not found!");
        //    }

        //    try
        //    {  
        //        foreach (string f in allFilesAtSourcePath)
        //        {
        //            string fn = Path.GetFileName(f);
                    

        //            if (System.Text.RegularExpressions.Regex.IsMatch(fn, sPattern))
        //            {
        //                if (_productionCopiedFiles.FileExtensions.Exists(x => fn.Contains(x)) && _productionCopiedFiles.ContainsAtName.Exists(x => fn.Contains(x)))
        //                { 
        //                    movingFiles.Add(f);
        //                }
        //            }                   
        //        }

        //        string destDir = Path.Combine(_destinationPath, _productionCopiedFiles.DestinationFolder);

        //        if (Directory.Exists(destDir) == false)
        //        {
        //            Directory.CreateDirectory(destDir);
        //        }

        //        foreach (string ef in movingFiles)
        //        {
        //            var destinationFile = Path.Combine(destDir, Path.GetFileName(ef));
        //            File.Copy(ef, destinationFile);
                    
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        //    }
            
           
        //}
        
        //public void MoveServicesFiles()
        //{
        //    string[] getedFiles = Directory.GetFiles(_sourcePath);
        //    List<string> movingFiles = new List<string>();
        //    string sPattern = @"^A\d{2}_*\.*";

        //    try
        //    {
        //        if (_serviceCopiedFiles.DestinationFolder == null)
        //            throw new NullReferenceException("Services Copied FilesFilters destination directory not found!");

        //        foreach (string f in getedFiles)
        //        {
        //            if(System.Text.RegularExpressions.Regex.IsMatch(Path.GetFileName(f), sPattern))
        //            {
        //                if (_serviceCopiedFiles.FileExtensions.Exists(x => Path.GetFileName(f).Contains(x)) && _serviceCopiedFiles.ContainsAtName.Exists(x => Path.GetFileName(f).Contains(x)))//uściślić tylko do Glue_070ZG01_G16.src i Glue_070ZG01_G16.dat
        //                {
        //                    movingFiles.Add(f);
        //                }
        //            }                   
        //        }

        //        string destDir = Path.Combine(_destinationPath, _serviceCopiedFiles.DestinationFolder);

        //        if (Directory.Exists(destDir) == false)
        //        {
        //            Directory.CreateDirectory(destDir);
        //        }

        //        foreach (string ef in movingFiles)
        //        {
        //            File.Copy(ef, Path.Combine(destDir, Path.GetFileName(ef)));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        //    }
        //}

        //#endregion CopyFiles


        //#region CopyData

        //public void OlpFilesDataCopy()
        //{
        //    throw new NotImplementedException();
        //}

        //public void GlobalFilesDataCopy()
        //{
        //    throw new NotImplementedException();
        //}

        //#endregion CopyData

        //#region DeleteFiles 
        //public void DeleteFiles()
        //{
        //    throw new NotImplementedException();
        //}
        //#endregion DeleteFiles

        //#region IsPossible
        //public bool IsPossibleCopyProductionFiles()
        //{
        //    return (_productionCopiedFiles.FileExtensions?.Count > 0 && _productionCopiedFiles.DestinationFolder != null);           
        //}
        //public bool IsPossibleCopyServicesFiles()
        //{
        //    return (_serviceCopiedFiles.FileExtensions?.Count > 0 && _serviceCopiedFiles.DestinationFolder != null);
        //}
        //public bool IsPossibleOlpFilesDataCopy()
        //{
        //    return (_copiedOlpDataFiles.FileExtensions?.Count > 0 && _copiedOlpDataFiles.DestinationFolder != null);
        //}
        //public bool IsPossibleGlobalFilesDataCopy()
        //{
        //    return (_copiedGlobalDataFiles.FileExtensions?.Count > 0 && _copiedGlobalDataFiles.DestinationFolder != null);
        //}
        //public bool IsPossibleDeleteFiles()
        //{
        //    return (_removingDataFiles.FileExtensions?.Count > 0);
        //}
        //#endregion IsPossible

    }
}
