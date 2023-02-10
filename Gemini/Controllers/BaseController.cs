using System;
using log4net;
using Gemini.Models;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using System.IO;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using Gemini.Controllers.Common;

namespace Gemini.Controllers
{
    public class BaseController : Controller
    {
        public ILog Log = LogManager.GetLogger(typeof(BaseController));

        public ResponseObj DataReturn;

        public BaseController()
        {
            DataReturn = new ResponseObj();
        }

        #region Database

        protected static GeminiEntities DataContext = new GeminiEntities();

        public static GeminiEntities DataGemini
        {
            get { return DataContext ?? (DataContext = new GeminiEntities()); }
        }

        public bool SaveData()
        {
            try
            {
                return DataGemini.SaveChanges() > 0;
            }
            catch (DbEntityValidationException dbEx)
            {
                var messageError = "";
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Log.Debug(validationError);

                        messageError = messageError + validationError.PropertyName + " " + validationError.ErrorMessage;
                    }
                }
                var argEx = new ArgumentException(messageError, dbEx);
                throw argEx;
            }
        }

        #endregion

        #region En/Decrypt

        public static string Key = "CoreworkByDuong2022";

        public static string Encrypt(string clearText)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(Key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(Key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        #endregion

        #region GenCode

        private static Random random = new Random();

        public static string GenerateCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var code = string.Empty;

        GenCode:
            code = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());

            var lstCode = VerifyCodeCacheManager.VerifyCode.Select(x => x.Value);
            if (lstCode.Contains(code))
            {
                goto GenCode;
            }

            return code;
        }

        #endregion

        public string GetKeyFromValue_Token(string valueVar)
        {
            foreach (string keyVar in TokenCacheManager.TokenCached.Keys)
            {
                if (TokenCacheManager.TokenCached[keyVar] == valueVar)
                {
                    return keyVar;
                }
            }
            return null;
        }

        #region FileIO

        public void VerifyDir(string path)
        {
            try
            {
                var list = path.Split(new string[] { "\\" }, StringSplitOptions.None);
                var directory = path.Replace("\\" + list[list.Count() - 1], "");
                DirectoryInfo dir = new DirectoryInfo(directory);
                if (!dir.Exists)
                {
                    dir.Create();
                }
            }
            catch { }
        }

        public void WriteFileFromStream(Stream stream, string toFile)
        {
            using (FileStream fileToSave = new FileStream(toFile, FileMode.Create))
            {
                stream.CopyTo(fileToSave);
            }
        }

        #endregion

        #region Unicode

        private static readonly string[] VietnameseSigns = new string[]
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };

        public static string RemoveSign4VietnameseString(string str)
        {
            if (str.Contains(","))
            {
                str = str.Replace(" , ", " ");
                str = str.Replace(", ", " ");
                str = str.Replace(" ,", " ");
                str = str.Replace(",", "");
            }
            if (str.Contains("-"))
            {
                str = str.Replace(" - ", " ");
                str = str.Replace("- ", " ");
                str = str.Replace(" -", " ");
                str = str.Replace("-", " ");
            }
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            str = str.Replace(" ", "-");
            str = str.ToLower();
            return str;
        }

        #endregion
    }
}