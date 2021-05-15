using System;
using System.Linq;

namespace siscode_helper.Utils {
    public static class FileUtils {
        /// <summary>
        /// Similar to openTempFile, but only gets the file name. <br></br>
        /// See : https://hackage.haskell.org/package/base-4.15.0.0/docs/System-IO.html#v:openTempFile
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static string GetTempName(string template) {
            Random r = new();
            string name, ext;
            
            if (template.Contains(".")) {
                var split = template.Split(".");
                name = string.Join(".",split[..^1]);
                ext = split[^1];
            }
            else {
                name = template;
                ext = "";
            }

            return $"{name}{r.Next(10000)}.{ext}";
        }
    }
}
