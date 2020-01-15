using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ExJwtAuth.Configuratins
{
    public class JwtSecurityConfiguration
    {
        // 署名用鍵。実際には設定等から取得する    
        private readonly static byte[] secret
            = Encoding.UTF8.GetBytes(new String('s', 128));

        public static SymmetricSecurityKey SecurityKey { get; }
            = new SymmetricSecurityKey(secret);

        public static string Issuer { get; } = "ExJwtAuth";
    }
}
