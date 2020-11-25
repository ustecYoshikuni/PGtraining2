using System;
using System.Text.RegularExpressions;

namespace PGtraining.FileImportService
{
    internal static class CheckString
    {
        static public bool IsMatch(string target, string checkRule, bool just = false, int min = 0, int max = 0)
        {
            var result = false;

            if ((0 < min) && (string.IsNullOrEmpty(target)))
            {
                return result;
            }

            if (Regex.IsMatch(@target, @checkRule))
            {
                result = true;
            }

            if (0 < max)
            {
                result = (target.Length <= max) ? true : false;

                if ((result) && (just))
                {
                    result = (target.Length == max) ? true : false;
                }
            }
            return result;
        }

        /// <summary>
        /// targetが英数字のみか判定
        /// 文字数が指定されている場合は、min～max
        /// 文字数が指定の文字数でジャストか以下か　true:ジャスト,false:以下OK
        /// </summary>
        /// <param name="target"></param>
        /// <param name="just"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        static public bool IsAlphaNumericOnly(string target, bool just = false, int min = 0, int max = 0)
        {
            var result = false;

            if ((0 < min) && (string.IsNullOrEmpty(target)))
            {
                return result;
            }

            if (Regex.IsMatch(@target, @"[^a-zA-z0-9]"))
            {
                return result;
            }

            if (0 < max)
            {
                result = (target.Length <= max) ? true : false;

                if ((result) && (just))
                {
                    result = (target.Length == max) ? true : false;
                }
            }

            return result;
        }

        static public bool IsAlphaNumericPlus(string target, bool just = false, int min = 0, int max = 0)
        {
            var result = false;

            if ((0 < min) && (string.IsNullOrEmpty(target)))
            {
                return result;
            }

            if (Regex.IsMatch(@target, @"[^a-zA-z0-9_-]"))
            {
                return result;
            }

            if (0 < max)
            {
                result = (target.Length <= max) ? true : false;

                if ((result) && (just))
                {
                    result = (target.Length == max) ? true : false;
                }
            }

            return result;
        }

        static public bool IsKataKana(string target, bool just = false, int min = 0, int max = 0)
        {
            var result = false;
            if ((0 < min) && (string.IsNullOrEmpty(target)))
            {
                return result;
            }

            if (Regex.IsMatch(@target, @"^[\p{IsKatakana}\u31F0-\u31FF\u3099-\u309C]+$"))
            {
                return result;
            }

            if (0 < max)
            {
                result = (target.Length <= max) ? true : false;

                if ((result) && (just))
                {
                    result = (target.Length == max) ? true : false;
                }
            }

            return result;
        }

        static public bool IsDateTime(string target, string format = "")
        {
            if ((format.ToUpper() == "YYYYMMDD"))
            {
                target = YyyymmddToDateString(target);
            }

            if (DateTime.TryParse(target, out var result))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static public string YyyymmddToDateString(string date)
        {
            if (IsAlphaNumericOnly(date.Substring(0, 8), true, 1, 8))
            {
                var dateString = $"{date.Substring(0, 4)}/{date.Substring(4, 2)}/{date.Substring(6, 2)}";
                return dateString;
            }

            return "";
        }
    }
}