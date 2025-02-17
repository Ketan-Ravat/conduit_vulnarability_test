using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Westwind.Utilities;

namespace Jarvis.ViewModels.ViewModels
{
    public class ApiError
    {

        public string message { get; set; }
        public object detail { get; set; }

        public ValidationErrorCollection errors { get; set; }

        public ApiError(string message)
        {
            this.message = message;
        }

        public ApiError(ModelStateDictionary modelState)
        {
            if (modelState != null && modelState.Any(m => m.Value.Errors.Count > 0))
            {
                detail = modelState.Where(x => x.Value.Errors.Count > 0)
                   .Select(x => new { x.Key, x.Value.Errors }).ToArray();
                message = "Please correct the specified errors and try again.";
            }
        }
    }
}
