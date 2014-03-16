using System;
using System.Runtime.Caching;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Net.Http.Formatting;

namespace WebApiCacheEx1.ViewModels
{
    public class ToDo
    {

        private static ObjectCache ToDoCache = MemoryCache.Default;
        private static IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<Hubs.CacheNotificationHub>();

        static ToDo() { }

        public async static void CacheToDoItem(Models.ToDo toDoItem)
        {
            CacheItemPolicy policy = CacheItemPolicyFactory();
            System.Reflection.MethodBase currentMethod = System.Reflection.MethodInfo.GetCurrentMethod();
            string cacheNotificationMessage = "To-do item has been cached by the server handling the original request.";

            try
            {
                List<string> cacheMemberServerList = new List<string>();
                string key = Guid.NewGuid().ToString();
                string cacheMemberServers = ConfigurationManager.AppSettings.Get("cacheDistributionPoints");

                // Cache here first, and notify client.
                //
                CacheItem itemToCache = new CacheItem(key, toDoItem);

                ToDoCache.Add(itemToCache, policy);
                PushCacheNotificationMessage(String.Format("{0}  Key: [{1}].", cacheNotificationMessage, key));

                // Notify other servers/sites, via HTTP POST, that this item was cached, and instruct them to cache it too.
                //
                if (!String.IsNullOrWhiteSpace(cacheMemberServers))
                {
                    cacheMemberServerList = cacheMemberServers.Split(',').ToList();
                }

                using (var client = new HttpClient())
                {
                    Models.CachedToDo cachedToDoItem = new Models.CachedToDo { Key = key, ToDoItem = toDoItem };

                    foreach (string member in cacheMemberServerList)
                    {
                        var memberServer = member.Trim();
                        client.DefaultRequestHeaders.Accept.Clear();

                        var response = await client.PostAsync(memberServer +"/api/cacheitem", cachedToDoItem, new JsonMediaTypeFormatter());

                        if (response.IsSuccessStatusCode)
                        {
                            string cachedKey = await response.Content.ReadAsStringAsync();

                            // Notify the client ...
                            //
                            PushCacheNotificationMessage(String.Format("To-do item with key [{0}] has been cached by member server [{1}].", cachedKey, memberServer));

                            // Write it to the log, if a TextWriterTraceListener, or XmlWriterTraceListener is attached.
                            //
                            System.Diagnostics.Trace.WriteLine(String.Format("{0}.{1}: Response status code is [{2}].", currentMethod.DeclaringType, currentMethod.Name, response.StatusCode));
                        }
                    }
                }

            }
            catch (System.Exception generalException)
            {
                // Write it to the log, if a TextWriterTraceListener, or XmlWriterTraceListener is attached.
                //
                System.Diagnostics.Trace.WriteLine(String.Format("{0}.{1}: {2}", currentMethod.DeclaringType, currentMethod.Name, generalException.Message));
            }
        }

        public static string CacheDistributedToDoItem(Models.CachedToDo cachedToDoItem)
        {
            string cacheNotificationMessage = "To-do item has been cached by a cache member server.";
            CacheItemPolicy policy = CacheItemPolicyFactory();
            CacheItem itemToCache = new CacheItem(cachedToDoItem.Key, cachedToDoItem.ToDoItem);

            ToDoCache.Add(itemToCache, policy);
            PushCacheNotificationMessage(String.Format("{0}  Key: [{1}].", cacheNotificationMessage, cachedToDoItem.Key));

            return itemToCache.Key;
        }

        public static CacheItemPolicy CacheItemPolicyFactory()
        {
            return new CacheItemPolicy { AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddSeconds(30)) };
        }

        public static void PushCacheNotificationMessage(string cacheNotificationMessage)
        {
            hubContext.Clients.All.cacheNotification("0", cacheNotificationMessage);
        }
    }
}