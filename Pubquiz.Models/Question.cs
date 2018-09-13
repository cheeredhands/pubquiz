using System;
using System.Collections.Generic;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Pubquiz.Models
{
    public class Question
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public List<Media> Media { get; set; }
        public InteractionType InteractionType { get; set; }
        public int MaxScore { get; set; }
        public List<Interaction> Interactions { get; set; }

        public Question()
        {
            Media = new List<Media>();
        }
    }

    public class Interaction
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public int MaxScore { get; set; }
        public List<ChoiceOption> ChoiceOptions { get; set; }
        public InteractionType InteractionType { get; set; }
        public Solution Solution { get; set; }

        public Interaction()
        {
            ChoiceOptions = new List<ChoiceOption>();
        }
    }

    public class Solution
    {
        public List<Guid> ChoiceOptionIds { get; set; }
        public List<string> Responses { get; set; }
        public int LevenshteinTolerance { get; set; }
        public bool FlagIfWithinTolerance { get; set; }

        public Solution(List<Guid> optionIds)
        {
            ChoiceOptionIds = optionIds;
        }

        public Solution(List<string> responses, int levenshteinTolerance = 0, bool flagIfWithinTolerance = false)
        {
            Responses = responses;
            LevenshteinTolerance = levenshteinTolerance;
            FlagIfWithinTolerance = flagIfWithinTolerance;
        }
    }

    public class ChoiceOption
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        // maybe later? public Media Media { get; set; }
    }

    public enum InteractionType
    {
        MC, // Multiple choice
        MR, // Multiple response
        SA, // Short answer
        ET, // Extended text
    }

    public class Media
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Uri { get; }
        public Dimensions Dimensions { get; set; }
        public MediaType MediaType { get; }

        public Media(string uri, MediaType mediaType)
        {
            Uri = uri;
            MediaType = mediaType;
        }
    }

    public enum MediaType
    {
        Image,
        Video,
        Audio
    }

    public class Dimensions
    {
        public int OriginalWidth { get; set; }
        public int OriginalHeight { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int DurationInSeconds { get; set; }
    }
}