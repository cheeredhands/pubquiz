using System;
using System.Collections.Generic;
using System.Text;
using Pubquiz.Persistence;

namespace Pubquiz.Domain.Models
{
    public class QuizItem : Model
    {
        public ItemType ItemType { get; set; }

        public QuizItem(Guid id, ItemType itemType)
        {
            Id = id;
            ItemType = itemType;
        }
    }

    public enum ItemType
    {
        Question,
        Information
    }
}