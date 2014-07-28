using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiCacheEx1.Controllers
{
    public class WarmupController : ApiController
    {
        // GET: api/Warmup
        public bool Get()
        {
            return true;
        }
    }
}
