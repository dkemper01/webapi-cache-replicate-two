using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiCacheEx1.Models
{
    public class CachedToDo
    {
        public string Key { get; set; }
        public ToDo ToDoItem { get; set; }
    }
}