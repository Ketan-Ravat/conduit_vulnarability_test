using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Jarvis.ViewModels
{
    public class UploadFileViewModel
    {
        public IFormFile file { get; set; }
        public string fileName { get; set; }
        public string sessionId { get; set; }
        public string fileType { get; set; }
        public UploadFileViewModel()
        {
            fileType = "img";
        }
    }
}
