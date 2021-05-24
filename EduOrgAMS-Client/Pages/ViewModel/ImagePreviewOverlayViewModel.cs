using System;

namespace EduOrgAMS.Client.Pages.ViewModel
{
    public class ImagePreviewOverlayViewModel : PageViewModel
    {
        private string _imageSource;
        public string ImageSource
        {
            get
            {
                return _imageSource;
            }
            set
            {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        public ImagePreviewOverlayViewModel()
            : base(typeof(ImagePreviewOverlayPage))
        {

        }
    }
}
