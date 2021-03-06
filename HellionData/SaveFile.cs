﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace HellionData
{
    public static class SaveFile
    {
        #region Fields

        /// <summary>
        /// Holds the entire Save File Json
        /// </summary>
        internal static JObject saveData;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating if the save file has been loaded.
        /// </summary>
        /// <value><c>true</c> if is loaded; otherwise, <c>false</c>.</value>
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
        static public void LoadLatestSaveFile(string saveFolder)
        {
            var files = new System.IO.DirectoryInfo(saveFolder).GetFileSystemInfos("*.save").OrderBy(f => f.LastWriteTime);

            if (files.Count() != 0)
            {
                var filePath = files.Last().FullName;
				LoadSaveFile(filePath);
            }
            else
            {
                var errorMessage = String.Format("Cannot find the save file in folder: {0}", saveFolder);
                throw new FileNotFoundException(errorMessage);
            }
        }

        /// <summary>
        /// Read the specific save file into memory
        /// </summary>
        /// <param name="saveFile">The full file path of the save file</param>
        static public void LoadSaveFile(string saveFile)
        {
            if (!File.Exists(saveFile))
            {
                var errorMessage = String.Format("Cannot find the save file: {0}", saveFile);
                throw new FileNotFoundException(errorMessage);
            }

            //Console.WriteLine("Reading save file: {0}", saveFile);
            using (StreamReader file = File.OpenText(saveFile))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    saveData = (JObject)JToken.ReadFrom(reader);
					FilePath = saveFile;
					IsLoaded = true;
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
                File.Move(FilePath, FilePath + "-" + DateTime.Now.ToString("yyyyMdhmmsss") + ".backup");
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
