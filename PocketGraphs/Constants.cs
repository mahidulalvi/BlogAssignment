﻿using System.Collections.Generic;
using System.Web;

namespace PocketGraphs
{
    public static class Constants
    {
        public static readonly List<string> AllowedFileExtensions =
            new List<string> { ".jpg", ".jpeg", ".png" };

        public static readonly string UploadFolder = "~/Upload/";

        public static readonly string MappedUploadFolder = HttpContext.Current.Server.MapPath(UploadFolder);
    }
}