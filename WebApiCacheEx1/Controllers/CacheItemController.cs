using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiCacheEx1.Controllers
{
    public class CacheItemController : ApiController
    {
        // POST api/<controller>
        public string Post([FromBody]Models.CachedToDo toDoItem)
        {
            return ViewModels.ToDo.CacheDistributedToDoItem(toDoItem);
        }

    }
}