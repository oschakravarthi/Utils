using SubhadraSolutions.Utils.Contracts;

namespace SubhadraSolutions.Utils
{
    public class CompanyInfo : INamed
    {
        public const string DefaultSectionName = "CompanyInfo";
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string CompanyUrl { get; set; }
        public string PrivacyStatementUrl { get; set; }
    }
}