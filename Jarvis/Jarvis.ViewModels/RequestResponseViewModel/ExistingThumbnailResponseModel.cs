using Jarvis.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.ViewModels.RequestResponseViewModel
{
    public class ExistingThumbnailResponseModel
    {
        public ListViewModel<string> thumbnailImages { get; set; }
        public ListViewModel<string> originalImages { get; set; }
        public ExistingThumbnailResponseModel()
        {
            thumbnailImages = new ListViewModel<string>();
            originalImages = new ListViewModel<string>();
        }
    }
}
