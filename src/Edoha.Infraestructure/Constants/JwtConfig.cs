using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Infraestructure.Constants
{
    public static class JwtConfig
    {
        public static string Key = "\"S3nh4Muit0F0rt3eS3gur4d3NoM1n1mo32\"";
        public static string Issuer = "EdohaIssuer";
        public static string Audience = "EdohaAudience";
        public static int ExpirationMinutes = 600;
        public static bool ExpiresAtSessionEnding = true;
    }
}
