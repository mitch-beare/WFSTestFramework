using System;
using System.IO;

namespace HealthCheckUI.Model
{
    public static class HelperFunctions
    {
        public static string getValidatedPath(string path)
        {
            path = path.Trim();
            if (!path.EndsWith("\\"))
            {
                path += ("\\");
            }
            try
            {
                Path.GetFullPath(path);
            }
            catch
            {
                return null;
            }
            return path;
        }

        public static bool isValidURL(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }
            if (!isvalidURI(url))
            {
                return false;
            }
            return true;
        }

        public static bool isvalidURI(string text)
        {
            Uri uriResult;
            return Uri.TryCreate(text, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}