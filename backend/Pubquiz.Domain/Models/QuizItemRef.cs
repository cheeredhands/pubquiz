using System;
using System.Collections.Generic;
using System.Text;
using Pubquiz.Persistence;

namespace Pubquiz.Domain.Models
{
    public class QuizItemRef : Model
    {
        public ItemType ItemType { get; set; }

        public QuizItemRef(string id, ItemType itemType)
        {
            Id = id;
            ItemType = itemType;
        }

        public QuizItemRef()
        {
            
        }
    }

    public enum ItemType
    {
        Question,
        Information
    }
}