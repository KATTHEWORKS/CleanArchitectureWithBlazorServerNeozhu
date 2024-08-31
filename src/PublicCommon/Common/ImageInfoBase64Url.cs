namespace PublicCommon.Common
    {
    public class ImageInfoBase64Url : IEquatable<ImageInfoBase64Url>
        {
        public ImageCategory? Category { get; set; }//may be not required
        public string? Url { get; set; }
        public string? Base64 { get; set; }

        public string? FileName { get; set; }

        //below are only UI purpose
        // public int? Index { get; set; }//may be required later ,not now

        //[JsonIgnore]//not required for backend so ignoring
        //public bool? IsProcessing { get; set; }

        //[JsonIgnore]//not required for backend so ignoring
        //public string? Warning { get; set; }

        public ImageInfoBase64Url() { } // Default constructor is must
        public ImageInfoBase64Url(string? url)
            {
            if (url != null && !url.IsNullOrEmptyString())
                {
                if (url.StartsWith(CONSTANTS.Base64ImagePrefix))
                    Base64 = url;
                else Url = IsUrl(url) ? url : null;
                }
            }
        public ImageInfoBase64Url(string? url, ImageCategory imageCategory) : this(url)
            {
            Category = imageCategory;
            }

        public ImageInfoBase64Url? BeforePostingCorrection()
            {
            if (!IsNullOrDefault(this))
                {
                if (IsBase64(Base64)) Url = null;
                if (!string.IsNullOrEmpty(Url) && !IsUrl(Url)) Url = null;
                if (!IsNullOrDefault(this)) return this;
                }
            return null;
            }

        public string GetFileNameToUpload(int id = 0) => this.FileName ?? ($"{id}.{CONSTANTS.DefaultImageExtension}");

        public static bool IsBase64(string? str) => !string.IsNullOrEmpty(str) && str.StartsWith(CONSTANTS.Base64ImagePrefix);

        public static bool IsBase64(ImageInfoBase64Url? image) => IsBase64(image?.Base64);
        //!IsNullOrDefault(image) may be not necessary

        public static bool IsUrl(string? str) => !string.IsNullOrEmpty(str) && !str.StartsWith(CONSTANTS.Base64ImagePrefix)
            && Uri.IsWellFormedUriString(str, UriKind.Absolute)
            //&& str != "PublicCommon.Common.ImageInfoBase64Url" 
            && str != nameof(PublicCommon.Common.ImageInfoBase64Url);

        public static bool IsUrl(ImageInfoBase64Url? image) => IsUrl(image?.Url);
        public static bool IsNullOrDefault(ImageInfoBase64Url? image) => (image == null ||
        (!IsUrl(image) && !IsBase64(image)));
        //IEquatable<CardProduct> implementation
        public bool Equals(ImageInfoBase64Url? image)//compares including id
            {//usage bool isEqual1 = person1.Equals(person2);
            if (image == null) return false; // Not the same type
            return Url == image.Url && Base64 == image.Base64; // Compare properties
            }

        public static bool Equals(ImageInfoBase64Url? source, ImageInfoBase64Url? other)//compares including id
            {//usage bool isEqual1 = person1.Equals(person2);
            if (source == null && other == null) return true;
            if (source == null || other == null) return false;

            return source.Url == other.Url && source.Base64 == other.Base64; // Compare properties
            }

        //tod add validation for is url and is base64
        public static string? GetUrl(ImageInfoBase64Url? image) => IsUrl(image) ? image!.Url : null;
        public static string? GetBase64(ImageInfoBase64Url? image) => IsBase64(image) ? image!.Base64 : null;

        public static void LoadUploadDelete(ImageInfoBase64Url? ImageN, string imageName, string? existingImageUrl, List<ImageInfo> listToUpload, List<ImageInfo> listToDelete)
            {
            if (ImageInfoBase64Url.IsBase64(ImageN)) listToUpload.Add(new ImageInfo(ImageN!, imageName));
            if (!string.IsNullOrEmpty(existingImageUrl) && existingImageUrl != ImageN?.Url)
                listToDelete.Add(new ImageInfo(existingImageUrl, imageName));
            }

        }
    // Class to hold image data and name
    public class ImageInfo
        {
        public ImageInfo()
            {

            }
        public ImageInfo(ImageInfoBase64Url imageData, string imageName)
            {
            ImageData = imageData;
            ImageName = imageName;
            }
        public ImageInfo(string url, string imageName)
            {
            Url = url;
            ImageName = imageName;
            }
        public ImageInfoBase64Url? ImageData { get; set; }
        public string? ImageName { get; set; }
        public string? Url { get; set; }//uploaded result
        public bool? DeletedStatus { get; set; }
        }
    public enum ImageCategory
        {
        CardMainImage,
        CardDetailImage
        }
    }
