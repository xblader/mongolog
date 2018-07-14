using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LogMongoDB.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            LogErrorEntry test = new LogErrorEntry
            {
                ID = "CY47",
                TimeStamp = DateTime.Now
            };
            // log4net.LogManager.GetLogger("test").Debug("test");
            log4net.LogManager.GetLogger("test").Debug(test);
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }

    internal class DadosLog
    {
        public string User { get; internal set; }
        public string StackTrace { get; internal set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
