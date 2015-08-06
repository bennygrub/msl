using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySelfie
{
    public static class FileExtension
    {
        public static byte[] getFileBytes(this HttpPostedFileBase file)
        {
            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
            {
                string fileName = file.FileName;
                string fileContentType = file.ContentType;
                byte[] fileBytes = new byte[file.ContentLength];
                file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                return fileBytes;
            }

            return new byte[1];
        }
        public static string getFileType(this HttpPostedFileBase file)
        {
            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
            {
                return file.ContentType;
            }

            return "";
        }
    }
}