using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;

namespace Cleverscape.UTorrentClient.WebClient.ServiceDefinition
{
    class JsonContentTypeMapper: WebContentTypeMapper
    {
        public override WebContentFormat
                   GetMessageFormatForContentType(string contentType)
        {
            if (contentType.ToLower() == "text/plain" || contentType == "text/javascript")
            {
                return WebContentFormat.Json;
            }
			else if (contentType.ToLower() == "text/html")
			{
				return WebContentFormat.Raw;
			}
			else
			{
				return WebContentFormat.Default;
			}
        }
    }
}