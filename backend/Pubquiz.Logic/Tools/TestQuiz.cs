using System;
using System.Collections.Generic;
using Pubquiz.Domain.Models;
using Pubquiz.Persistence.Extensions;

namespace Pubquiz.Logic.Tools
{
    public static class TestQuiz
    {
        public static Question GetMCQuestion()
        {
            var mcQuestion = new Question
            {
                Id = Guid.Parse("EB42AF2C-DD76-4470-8480-8EC6DD8203DA").ToShortGuidString(),
                Body = "Question body",
                Title = "Multiple choice",
                MaxScore = 1,
                QuestionType = QuestionType.MultipleChoice
            };

            mcQuestion.Interactions.Add(new Interaction(1)
            {
                Text = "Choose",
                ChoiceOptions = new List<ChoiceOption>
                {
                    new ChoiceOption(1, "Option 1"),
                    new ChoiceOption(2, "Option 2"),
                    new ChoiceOption(3, "Correct option 3"),
                    new ChoiceOption(4, "Option 4")
                },
                Solution = new Solution(new[] {3}),
                InteractionType = InteractionType.MultipleChoice,
                MaxScore = 1
            });

            return mcQuestion;
        }

        public static Question GetMRQuestion()
        {
            var mrQuestion = new Question
            {
                Id = Guid.Parse("F39D1BD5-60E9-4615-8512-553149CDC28C").ToShortGuidString(),
                Body = "Question body",
                Title = "Multiple response",
                MaxScore = 1,
                QuestionType = QuestionType.MultipleResponse
            };

            mrQuestion.Interactions.Add(new Interaction(1)
            {
                Text = "Choose",
                ChoiceOptions = new List<ChoiceOption>
                {
                    new ChoiceOption(1, "Option 1"),
                    new ChoiceOption(2, "Option 2"),
                    new ChoiceOption(3, "Correct option 3"),
                    new ChoiceOption(4, "Corrent option 4")
                },
                Solution = new Solution(new[] {3, 4}),
                InteractionType = InteractionType.MultipleResponse,
                MaxScore = 1
            });

            return mrQuestion;
        }

        public static Question GetSAQuestion()
        {
            var mrQuestion = new Question
            {
                Id = Guid.Parse("6423EA00-9984-40DC-8D76-499447C3EA3B").ToShortGuidString(),
                Body = "Question body",
                Title = "Short answer with one solution",
                MaxScore = 1,
                QuestionType = QuestionType.ShortAnswer
            };

            mrQuestion.Interactions.Add(new Interaction(1)
            {
                Text = "What is the right answer? (hint: 'answer')",
                Solution = new Solution(new[] {"answer"}),
                InteractionType = InteractionType.ShortAnswer,
                MaxScore = 1
            });

            return mrQuestion;
        }

        public static Question GetSAWithMultipleSolutionsQuestion()
        {
            var mrQuestion = new Question
            {
                Id = Guid.Parse("DD550891-8EF3-4313-BF0F-871D96A3BFC5").ToShortGuidString(),
                Body = "Question body",
                Title = "Short answer with multiple solutions",
                MaxScore = 1,
                QuestionType = QuestionType.ShortAnswer
            };

            mrQuestion.Interactions.Add(new Interaction(1)
            {
                Text = "What is the right answer? (hint: 'answer')",
                Solution = new Solution(new[] {"answer", "ansver"}),
                InteractionType = InteractionType.ShortAnswer,
                MaxScore = 1
            });

            return mrQuestion;
        }

        public static Question GetMultipleSAQuestion()
        {
            var multipleSAQuestion = new Question
            {
                Id = Guid.Parse("4B431B8E-9C58-4849-AB0E-3070767CFC81").ToShortGuidString(),
                Body = "Question body",
                Title = "Test question",
                MaxScore = 3,
                QuestionType = QuestionType.ShortAnswer
            };

            multipleSAQuestion.Interactions.Add(new Interaction(1)
            {
                Text = "1. What is the right answer? (hint: 'answer1')",
                Solution = new Solution(new[] {"answer1"}),
                InteractionType = InteractionType.ShortAnswer,
                MaxScore = 1
            });

            multipleSAQuestion.Interactions.Add(new Interaction(2)
            {
                Text = "2. What is the right answer? (hint: 'answer2')",
                Solution = new Solution(new[] {"answer2"}),
                InteractionType = InteractionType.ShortAnswer,
                MaxScore = 2
            });
            return multipleSAQuestion;
        }

        public static Question GetETQuestion()
        {
            var etQuestion = new Question
            {
                Id = Guid.Parse("88681DFF-61E7-4289-8CCD-835D02BF0FA8").ToShortGuidString(),
                Body = "Question body",
                Title = "Extended text (no solution, manually corrected)",
                MaxScore = 1,
                QuestionType = QuestionType.ExtendedText
            };

            etQuestion.Interactions.Add(new Interaction(1)
            {
                Text = "Name all of the right answers.",
                InteractionType = InteractionType.ExtendedText,
                MaxScore = 1
            });

            return etQuestion;
        }

        public static List<Question> GetQuestions()
        {
            return new List<Question>
            {
                GetMCQuestion(),
                GetMRQuestion(),
                GetSAQuestion(),
                GetSAWithMultipleSolutionsQuestion(),
                GetMultipleSAQuestion(),
                GetETQuestion()
            };
        }

        public static Quiz GetQuiz()
        {
            var quizSection1 = new QuizSection
            {
                Id = Guid.Parse("CFD8977B-4F15-4F7C-9E18-DE1238B97327").ToShortGuidString(),
                Title = "First quiz section",
                QuizItems = new List<QuizItem>
                {
                    new QuizItem(GetMCQuestion().Id, ItemType.Question),
                    new QuizItem(GetMRQuestion().Id, ItemType.Question),
                    new QuizItem(GetSAQuestion().Id, ItemType.Question)
                }
            };

            var quizSection2 = new QuizSection
            {
                Id = Guid.Parse("41E0F686-CEF2-493E-A5FD-1D1DD75BAEEB").ToShortGuidString(),
                Title = "Second quiz section",
                QuizItems = new List<QuizItem>
                {
                    new QuizItem(GetSAWithMultipleSolutionsQuestion().Id, ItemType.Question),
                    new QuizItem(GetMultipleSAQuestion().Id, ItemType.Question),
                    new QuizItem(GetETQuestion().Id, ItemType.Question)
                }
            };

            var quiz = new Quiz
            {
                Id = Guid.Parse("ACC39DF3-B1A5-4F97-BC12-AA59BC580418").ToShortGuidString(),
                Title = "Testquiz"
            };
            quiz.QuizSections.Add(quizSection1);
            quiz.QuizSections.Add(quizSection2);

            return quiz;
        }
    }
}