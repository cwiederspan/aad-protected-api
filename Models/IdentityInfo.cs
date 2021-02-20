using System;

namespace AadProtectedApi.Models {

    public class IdentityInfo {

        public DateTime Date { get; set; }

        public string Audience { get; set; }

        public string Issuer { get; set; }

        public string Scope { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        public string PreferredUsername { get; set; }
    }
}
