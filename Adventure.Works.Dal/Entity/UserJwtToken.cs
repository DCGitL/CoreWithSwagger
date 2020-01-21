using System;
using System.Collections.Generic;
using System.Text;

namespace Adventure.Works.Dal.Entity
{
    public class UserJwtToken
    {
        public string  AccessToken { get; set; }

        public DateTimeOffset  AccessTokenExpiration { get; set; }

        public string RefreshToken { get; set; }
    }
}
