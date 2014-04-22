using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace KinectStudienarbeitWpf
{
    /// <summary>
    /// Creates a Wpf Ressource Dictionary out of a Blender-exported file
    /// </summary>
    class BlenderResourceDictionary
    {
        private const String regexPattern_materialsAndModels = "(<Model3DGroup(.*?)</Model3DGroup>)|(<MaterialGroup(.*?)</MaterialGroup>)|(<Model3DGroup x:key=\"(.*?)\">(.*?)</Model3DGroup)";
        private const String regexPattern_modelWithoutKey = "<Model3DGroup>";
        private const String regexPattern_modelTransformations = "<Model3DGroup.Transform>(.*?)</Model3DGroup.Transform>";
        private const String regexPattern_emptyModels = "<Model3DGroup>(\\s*?)</Model3DGroup>";
        private const String regexPattern_modelMaterial = " Material=\"(.*?)\"";
        private const String replaceNameWithKey_Name = "<Model3DGroup x:Name";
        private const String replaceNameWithKey_Key = "<Model3DGroup x:Key";
        private const String tag_resDictOpen = "<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">";
        private const String tag_resDictClose = "</ResourceDictionary>";

        public ResourceDictionary resourceDictionary;
        public List<String> keyList;

        /// <summary>
        /// Creates a Resource Dictionary, removes all Transformations
        /// </summary>
        /// <param name="filepath">Path to the Blender-exported file</param>
        public BlenderResourceDictionary(String filepath) : this(filepath, true)
        {
        }

        /// <summary>
        /// Creates a Resource Dictionary
        /// </summary>
        /// <param name="filepath">Path to the Blender-exported file</param>
        /// <param name="removeTransformations">True if all transformations shall be removed, false otherwise</param>
        public BlenderResourceDictionary(String filepath, bool removeTransformations)
        {
            string text = File.ReadAllText(filepath);         //read the text file
            Regex r = new Regex(regexPattern_materialsAndModels, RegexOptions.Singleline);     //prepare the regex for materials and objects
            MatchCollection matches = r.Matches(text);      //apply the regex
            StringBuilder sb = new StringBuilder();
            foreach (Match m in matches)
            {
                sb.Append(m.Value);                     //append all regex matches to a string
            }
            text = sb.ToString();

            if (removeTransformations)
            {
                r = new Regex(regexPattern_modelTransformations, RegexOptions.Singleline);      //remove all translations, scaling and rotation
                text = r.Replace(text, "");
            }

            r = new Regex(regexPattern_emptyModels, RegexOptions.Singleline);
            text = r.Replace(text, "");                                 //delete empty models (can appear in Blender exported xaml)

            r = new Regex(regexPattern_modelWithoutKey, RegexOptions.Singleline);      //prepare regex for inserting key names
            Match match = r.Match(text);
            for (int i = 0; match.Success; i++)
            {
                text = text.Insert(match.Index + 13, " x:Key=\"Model" + i.ToString() + "\"");       //insert key name where needed
                match = r.Match(text);
            }

            text = text.Replace(replaceNameWithKey_Name, replaceNameWithKey_Key);

            r = new Regex(regexPattern_modelMaterial, RegexOptions.Singleline);      //set BackMaterial of a model to its Material (fixes some problems with Blender exoprt to xaml)
            matches = r.Matches(text);
            for (int i = 0; i < matches.Count; i++)
            {
                text = text.Insert(matches[i].Index + matches[i].Length, " BackMaterial" + matches[i].Value.Substring(9));  //write the BackMaterial String after the Material
                matches = r.Matches(text);          //update all matches to get updated indeces
            }

            text = text.Insert(0, tag_resDictOpen);     //insert valid tags for a resource dictionary
            text += tag_resDictClose;                   //insert a closing tag for a resource dictionary

            int tmpFileEnum = 0;
            String tmpFilePath;
            while (File.Exists((tmpFilePath = Directory.GetCurrentDirectory() + "\\tmp" + tmpFileEnum.ToString() + ".xaml")))
            {
                tmpFileEnum++;                  //look for a free name for a temporary file
            }
            StreamWriter sw = File.CreateText(tmpFilePath);     //create a new temporary file with the resource dictionary
            sw.Write(text);
            sw.Close();
            sw.Dispose();

            resourceDictionary = new ResourceDictionary();      //create a new resource dictionary
            resourceDictionary.Source = new Uri(tmpFilePath);   //read the temp file
            File.Delete(tmpFilePath);                           //delete the temp file

            keyList = new List<string>();
            foreach (String s in resourceDictionary.Keys)
            {
                keyList.Add(s);                                 //make a list of keys for easier use
            }

        }
    }
}
