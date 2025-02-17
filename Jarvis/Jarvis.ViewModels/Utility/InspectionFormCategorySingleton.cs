using Jarvis.db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.ViewModels.Utility
{
    public class InspectionFormCategorySingleton
    {
        Translator translator = null;

        private static InspectionFormCategorySingleton instance = null;
        private static readonly object padlock = new object();

        InspectionFormCategorySingleton()
        {
        }

        public static InspectionFormCategorySingleton Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new InspectionFormCategorySingleton();
                    }
                    return instance;
                }
            }
        }

        public List<InspectionAttributeCategory> InspectionFormCategoryMaster { get; set; }

        public string GetCategoryName(int category_id)
        {
            string name = InspectionFormCategoryMaster
                      .Where(x => x.category_id == category_id)
                      .Select(x => x.name)
                      .FirstOrDefault();
            return name;
        }
    }
}
