using log4net.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogMongoDB
{
    public class MongoAppenderField
    {
        /// <summary>
        /// Gets or sets the name of the log field
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the log layout type that will format the final log entry
        /// </summary>
        public IRawLayout Layout { get; set; }

        /// <summary>
        /// Gets or sets the log format value
        /// </summary>
        public string Value { get; set; }
    }
}