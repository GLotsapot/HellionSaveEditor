using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace HellionSaveEditor
{
    static class SaveFile
    {
        #region Fields

        /// <summary>
        /// Holds the entire saveFileJson
        /// </summary>
        private static JObject saveData;

        #endregion

        #region Properties

        static public bool IsLoaded { get; private set; }

        /// <summary>
        /// The complete file path of the save file that has been loaded
        /// </summary>
        static public string FilePath { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Finds the *.save game with the most recent Date Modified.
        /// This is usually the latest save game, but can be used to force a specific save to load instead
        /// </summary>
        /// <param name="saveFolder">The folder to search for the latest .save file</param>
        /// <returns>The path to the file that was loaded. If no save found, returns null.</returns>
        static public void LoadLatestSaveFile(string saveFolder)
        {
            var files = new System.IO.DirectoryInfo(saveFolder).GetFileSystemInfos("*.save").OrderBy(f => f.LastWriteTime);

            if (files.Count() != 0)
            {
                FilePath = files.Last().FullName;
                LoadSaveFile(FilePath);
                IsLoaded = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveFile"></param>
        static public void LoadSaveFile(string saveFile)
        {
            if (!File.Exists(saveFile))
            {
                var errorMessage = String.Format("Cannot find the save file: {0}", saveFile);
                throw new FileNotFoundException(errorMessage);
            }

            Console.WriteLine("Reading save file: {0}", saveFile);
            using (StreamReader file = File.OpenText(saveFile))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    saveData = (JObject)JToken.ReadFrom(reader);
                }
            }
        }

        static public bool LoadResourceFile()
        {
            //TODO: Create function to load the resource file
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write the save file back to the system, and backup the original (... or not)
        /// </summary>
        /// <param name="backup">Backup the existing file before saving back</param>
        static public void WriteSave(bool backup = true)
        {
            if (backup)
            {
                System.IO.File.Move(FilePath, FilePath + "-" + DateTime.Now.ToString("yyyyMdhmmsss") + ".backup");
            }
            else
            {
                File.Delete(FilePath);
            }
            
            using (StreamWriter sWriter = File.CreateText(FilePath))
            {
                using (JsonTextWriter jWriter = new JsonTextWriter(sWriter))
                {
                    jWriter.Formatting = Formatting.Indented;
                    //jWriter.Indentation = 4;

                    saveData.WriteTo(jWriter);
                }
            }
        }

        #endregion
    }
}
