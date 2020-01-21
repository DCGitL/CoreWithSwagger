using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWithSwagger.Helper
{
    public class AppSettings
    {
        public string Secret { get; set; }

        public TimeSpan TokenLifeTime { get; set; }
        public string DatabaseConnectionstring { get; set; }

    }

   
}
