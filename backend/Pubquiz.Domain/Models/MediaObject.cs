using System;

namespace Pubquiz.Domain.Models
{
    public class MediaObject
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Uri { get; set; }
        public MediaDimensions Dimensions { get; set; }
        public MediaType MediaType { get; set; }
        public string MimeType { get; set; }
        
        public MediaObject(string uri, MediaType mediaType)
        {
            Id = Guid.NewGuid();
            Uri = uri;
            MediaType = mediaType;
        }

        public MediaObject()
        {
        }
    }
}