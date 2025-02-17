using Amazon;
using Amazon.Translate;
using Amazon.Translate.Model;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Translation.V2;
using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Jarvis.ViewModels.Utility
{
    public class Translator
    {
        private readonly DBContextFactory _context;

        internal DbSet<PreferLanguageMaster> dbSet;

        public Translator()
        {
            this._context = new DBContextFactory();
            this.dbSet = _context.Set<PreferLanguageMaster>();
        }

        public virtual async Task<bool> Update(PreferLanguageMaster entity)
        {
            bool IsSuccess = false;
            try
            {
                dbSet.Update(entity);
                var response = await _context.SaveChangesAsync();
                if (response > 0)
                {
                    IsSuccess = true;
                }
                else
                {
                    IsSuccess = false;
                }
            }
            catch (Exception e)
            {
                IsSuccess = false;
                throw e;
            }
            return IsSuccess;
        }

        public class TranslatedCommentViewModel
        {
            public string SubmitterName { get; set; }
            public string CommentText { get; set; }
            public string TargetLangauge { get; set; }
            public TranslateTextResponse TranslateResponse { get; set; }
        }

        public string TranslateText(string key_name)
        {
            try
            {
                //AmazonTranslateClient translate = new AmazonTranslateClient(ConfigurationManager.AppSettings["AWS_ACCESS_KEY"], ConfigurationManager.AppSettings["AWS_SECRET_KEY"], RegionEndpoint.USWest1);
                AmazonTranslateClient translate = new AmazonTranslateClient(ConfigurationManager.AppSettings["AWS_ACCESS_KEY"], ConfigurationManager.AppSettings["AWS_SECRET_KEY"]);
                var request = new TranslateTextRequest() { Text = key_name, SourceLanguageCode = "en", TargetLanguageCode = "es" };
                var model = new TranslatedCommentViewModel()
                {
                    CommentText = key_name,
                    SubmitterName = "",
                    TargetLangauge = "es",
                    TranslateResponse = translate.TranslateTextAsync(request).Result // Make the actual call to Amazon Translate
                };

                string result = model.TranslateResponse.TranslatedText;

                //Console.OutputEncoding = System.Text.Encoding.UTF8;
                //var credential = GoogleCredential.FromFile(@"..\Jarvis\GoogleCreadintial\identityserver-23fd62b85a57.json");
                //TranslationClient client = TranslationClient.Create(credential);
                //var response = client.TranslateText(
                //    text: key_name,
                //    targetLanguage: "es",  // Spanish
                //    sourceLanguage: "en");  // English
                //Console.WriteLine(response.TranslatedText);
                //var result = response.TranslatedText;

                //var toLanguage = "es";//Spanish
                //var fromLanguage = "en";//English
                //var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={toLanguage}&dt=t&q={HttpUtility.UrlEncode(key_name)}";
                //var webClient = new WebClient
                //{
                //    Encoding = System.Text.Encoding.UTF8
                //};
                //var result = webClient.DownloadString(url);

                try
                {
                    //result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
                    //AddTranslatorMaster.Instance.AddMasterTable(key_name, result);
                    var alreadyexistkey = _context.PreferLanguageMaster.Where(x => x.key_name == key_name).FirstOrDefault();
                    if (alreadyexistkey == null)
                    {
                        PreferLanguageMaster preferLanguage = new PreferLanguageMaster();
                        preferLanguage.key_name = key_name;
                        preferLanguage.spanish_name = result;
                        Update(preferLanguage);
                    }
                    return result;
                }
                catch
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}