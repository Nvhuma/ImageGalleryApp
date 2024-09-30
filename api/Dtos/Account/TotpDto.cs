using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Account
{
    public class TotpDto
    {
         public string UserName { get; set; }
    public string Password { get; set; }
    public string TotpCode { get; set; }
    }
}