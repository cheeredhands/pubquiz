using System;

namespace Pubquiz.Domain.Models
{
    public class MediaObject
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Uri { get; set; }
        public string Text { get; set; }
        public MediaDimensions Dimensions { get; set; }
        public MediaType MediaType { get; set; }
        public bool IsSolution { get; set; }
        public string MimeType { get; set; }
        /// <summary>
        /// Indicates whether media object are visible to teams.
        /// </summary>
        public bool TeamVisible { get; set; }
        
        public MediaObject(string uri, MediaType mediaType)
        {
            Id = Guid.NewGuid();
            Uri = uri;
            MediaType = mediaType;
            TeamVisible = true;
        }

        public MediaObject()
        {
            Id = Guid.NewGuid();
            TeamVisible = true;
        }
    }
}