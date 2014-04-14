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
    class BlenderResourceDictionary
    {
        private const String regexPattern1 = "(<Model3DGroup>(.*?)</Model3DGroup>)|(<MaterialGroup(.*?)</MaterialGroup>)|(<Model3DGroup x:key=\"(.*?)\">(.*?)</Model3DGroup)";
        private const String regexPattern2 = "<Model3DGroup>";
        private const String regexPattern3 = "<Model3DGroup.Transform>(.*?)</Model3DGroup.Transform>";
        private const String resDictOpen = "<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">";
        private const String resDictClose = "</ResourceDictionary>";

        public ResourceDictionary resourceDictionary;
        public List<String> keyList;

        public BlenderResourceDictionary(String filepath) : this(filepath, true)
        {
        }

        public BlenderResourceDictionary(String filepath, bool removeTransformations)
        {
            string text = File.ReadAllText(filepath);         //read the text file
            Regex r = new Regex(regexPattern1, RegexOptions.Singleline);     //prepare the regex for materials and objects
            MatchCollection matches = r.Matches(text);      //apply the regex
            StringBuilder sb = new StringBuilder();
            foreach (Match m in matches)
            {
                sb.Append(m.Value);                     //append all regex matches to a string
            }
            text = sb.ToString();

            r = new Regex(regexPattern2, RegexOptions.Singleline);      //prepare regex for inserting key names
            Match match = r.Match(text);
            for (int i = 0; match.Success; i++)
            {
                text = text.Insert(match.Index + 13, " x:Key=\"Model" + i.ToString() + "\"");       //insert key name where needed
                match = r.Match(text);
            }

            if (removeTransformations)
            {
                r = new Regex(regexPattern3, RegexOptions.Singleline);      //remove all translations, scaling and rotation
                text = r.Replace(text, "");
            }

            text = text.Insert(0, resDictOpen);     //insert valid tags for a resource dictionary
            text += resDictClose;                   //insert a closing tag for a resource dictionary

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
