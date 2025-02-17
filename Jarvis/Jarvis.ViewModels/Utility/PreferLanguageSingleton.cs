using Jarvis.db.Models;
using Jarvis.Shared.StatusEnums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Jarvis.ViewModels.Utility
{
    public class PreferLanguageSingleton
    {
        Translator translator = null;

        private static PreferLanguageSingleton instance = null;
        private static readonly object padlock = new object();
        //public readonly IJarvisUOW _UoW;

        PreferLanguageSingleton()
        {
            //_UoW = new JarvisUOW();
        }

        public static PreferLanguageSingleton Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new PreferLanguageSingleton();
                    }
                    return instance;
                }
            }
        }

        public string GetLanguage(string key_name,int lang_type)
        {
            try
            {
                string response = GetLanguageKeyByName(key_name, lang_type).Result;
                return response;
            }catch(Exception e)
            {
                return null;
            }

        }

        public List<PreferLanguageMaster> PreferLanguageMaster { get; set; }

        public async Task<string> GetLanguageKeyByName(string key_name, int lang_type)
        {
            string name = null;
            if (lang_type == (int)Language.spanish)
            {
                name = PreferLanguageMaster
                       .Where(x => x.key_name == key_name)
                       .Select(x => x.spanish_name)
                       .FirstOrDefault();

                if(name == null)
                {
                    if(translator == null)
                    {
                        translator = new Translator();
                    }
                    name = translator.TranslateText(key_name);
                    //var date = DateTime.Parse("Monday, January 09, 2017");
                    //var culture = new CultureInfo("es-ES");
                    //var result = key_name.ToString("D", culture);
                }

                //if(name == null)
                //{
                //    var toLanguage = "en";//English
                //    var fromLanguage = "es";//Spanish
                //    var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={toLanguage}&dt=t&q={HttpUtility.UrlEncode(key_name)}";
                //    var webClient = new WebClient
                //    {
                //        Encoding = System.Text.Encoding.UTF8
                //    };
                //    var result = webClient.DownloadString(url);
                //    try
                //    {
                //        result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
                //        //AddTranslatorMaster.Instance.AddMasterTable(key_name, result);
                //        //PreferLanguageMaster preferLanguage = new PreferLanguageMaster();
                //        //preferLanguage.key_name = key_name;
                //        //preferLanguage.spanish_name = result;
                //        //_UoW.BaseGenericRepository<PreferLanguageMaster>().Insert(preferLanguage);
                //        return result;
                //    }
                //    catch
                //    {
                //        return null;
                //    }
                //}
            }
            return name;

            //string name = null;
            //if (lang_type == (int)Language.spanish)
            //{
            //    name = PreferLanguageMaster
            //           .Where(x => x.key_name == key_name)
            //           .Select(x => x.spanish_name)
            //           .FirstOrDefault();
            //}
            //return name;
        }
    }
}
