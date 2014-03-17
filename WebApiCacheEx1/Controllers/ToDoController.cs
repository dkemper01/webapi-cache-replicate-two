using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WebApiCacheEx1.Controllers
{
    public class ToDoController : ApiController
    {
        // GET api/<controller>
        public IList<Models.ToDo> Get()
        {
            return new List<Models.ToDo>();
        }

        // POST api/<controller>
        public async Task<bool> Post([FromBody]Models.ToDo toDoItem)
        {
            return await ViewModels.ToDo.CacheToDoItem(toDoItem);
        }

        // PUT api/<controller>/5
        public void Put(string key, [FromBody]Models.ToDo toDoItem)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(string key)
        {
        }
    }
}