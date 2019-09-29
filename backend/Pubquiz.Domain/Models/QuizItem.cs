using System;
using System.Collections.Generic;
using System.Text;
using Pubquiz.Persistence;

namespace Pubquiz.Domain.Models
{
    public class QuizItem : Model
    {
        public ItemType ItemType { get; set; }

        public QuizItem(string id, ItemType itemType)
        {
            Id = id;
            ItemType = itemType;
        }

        public QuizItem()
        {
            
        }
    }

    public enum ItemType
    {
        Question,
        Information
    }
}