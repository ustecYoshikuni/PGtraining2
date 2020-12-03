using System;
using System.Text.RegularExpressions;

namespace PGtraining.FileImportService
{
    internal static class CheckString
    {
        static public bool IsMatch(string target, string checkRule, int min = 0, int max = 0)
        {
            if (string.IsNullOrEmpty(target))
            {
                return false;
            }

            if (target.Length < min || max < target.Length)
            {
                return false;
            }

            return Regex.IsMatch(@target, @checkRule);
        }

        /// <summary>
        /// targetが英数字のみか判定
        /// 文字数が指定されている場合は、min～max
        /// </summary>
        /// <param name="target"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        static public bool IsAlphaNumericOnly(string target, int min = 0, int max = 0)
        {
            if (string.IsNullOrEmpty(target))
            {
                return false;
            }

            if (target.Length < min || max < target.Length)
            {
                return false;
            }

            return Regex.IsMatch(@target, @"^([a-zA-Z0-9]*$)");
        }

        static public bool IsAlphaNumericPlus(string target, int min = 0, int max = 0)
        {
            if (string.IsNullOrEmpty(target))
            {
                return false;
            }

            if (target.Length < min || max < target.Length)
            {
                return false;
            }

            return Regex.IsMatch(@target, @"^([a-zA-z0-9_-]*$)");
        }

        static public bool IsKataKana(string target, int min = 0, int max = 0)
        {
            if (string.IsNullOrEmpty(target))
            {
                return false;
            }

            if (target.Length < min || max < target.Length)
            {
                return false;
            }

            return true;
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
            if (IsAlphaNumericOnly(date.Substring(0, 8), 8, 8))
            {
                var dateString = $"{date.Substring(0, 4)}/{date.Substring(4, 2)}/{date.Substring(6, 2)}";
                return dateString;
            }

            return "";
        }
    }
}