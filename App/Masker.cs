namespace Nih.Masker
{
    using System;
    using System.Text.RegularExpressions;

    public static class MaskService
    {
        private const string EmailPtn = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$"; 
        
        public static string Email(this string email, string ptn = "###*", string maskCharacter = "*")
        {
            if (!Regex.IsMatch(email, MaskService.EmailPtn))
            {
                throw new ArgumentException($"Is invalid email address {email}.");
            }

            var splintedByEmailMark = email.Split('@');
            return $"{MaskService.Do(splintedByEmailMark[0], ptn, maskCharacter)}@{splintedByEmailMark[1]}";
        }
        
        private static string Do(string origin, string ptn, string maskCharacter)
        {
            var maskPtnGroups = MaskService.GenMaskPtnGroups(ptn, origin)
                                           .Match(origin);
            var masked = Regex.Replace(maskPtnGroups.Groups["mask"]
                                                    .Value,
                                       @".",
                                       maskCharacter);
            return $"{maskPtnGroups.Groups["head"].Value}{masked}{maskPtnGroups.Groups["tail"].Value}";
        }

        private static Regex GenMaskPtnGroups(string ptn, string origin)
        {
            var (head, mask, tail) = MaskService.GetMaskPtnGroupSizes(ptn, origin);
            return new
                Regex($"(?<head>.{MaskService.GenCountString(head)})(?<mask>.{MaskService.GenCountString(mask)})(?<tail>.{MaskService.GenCountString(tail)})");
        }

        private static string GenCountString(int number) => "{" + number + "}";
        private static (int hCnt, int mCnt, int tCnt) GetMaskPtnGroupSizes(string ptn, string origin)
        {
            var (ptnLen, originLen) = (ptn.Length, origin.Length);
            ptn = ptnLen > originLen
                      ? ptn.Substring(0, originLen - 1)
                      : ptn;
            var ptnGroups = new Regex(@"(?<head>[#|0]*)(?<mask>\**)(?<tail>[#|0]*)").Match(ptn);
            var (hLen, mLen, tLen) = (ptnGroups.Groups["head"]
                                               .Value.Length,
                                      ptnGroups.Groups["mask"]
                                               .Value.Length,
                                      ptnGroups.Groups["tail"]
                                               .Value.Length);
            mLen = ptnLen < originLen
                       ? originLen - hLen - tLen
                       : mLen;
            return (hLen, mLen, tLen);
        }
    }
}