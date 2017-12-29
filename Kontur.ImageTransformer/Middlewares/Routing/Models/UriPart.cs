using Kontur.ImageTransformer.Middlewares.Routing.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Kontur.ImageTransformer.Middlewares.Routing.Models
{
    internal class UriPart
    {
        public UriPartType Type { get; set; }
        public string Value { get; set; }

        public UriPart(string uriPart)
        {
            if (Regex.IsMatch(uriPart, @"\A<\w*>\z"))
            {
                this.Type = UriPartType.Dynamic;
                this.Value = uriPart.Substring(1, uriPart.Length - 2);
            }
            else if (Regex.IsMatch(uriPart, @"\A[\w\d\-%._~]*\z")) //RFC 3986 section 2.3 Unreserved Characters (January 2005)
            {
                this.Type = UriPartType.Static;
                this.Value = uriPart;
            }
            else
                throw new ArgumentException("Unsupported or invalid part of URI", "uriPart");
        }
    }
}
