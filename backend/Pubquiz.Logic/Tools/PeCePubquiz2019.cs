using System;
using System.Collections.Generic;
using Pubquiz.Domain.Models;
using Pubquiz.Persistence.Extensions;

namespace Pubquiz.Logic.Tools
{
    public class PeCePubquiz2019
    {
        public List<QuizItem> QuizItems { get; set; }

        public PeCePubquiz2019()
        {
            QuizItems = new List<QuizItem>
            {
                GetIntroItem(),
                GetS1Q1(),
                GetS1Q2(),
                GetS1Q3(),
                GetS2Q1(),
                GetS2Q2(), 
                GetS2Q3()
            };
        }

        public Quiz GetQuiz()
        {
            var quizSectionIntro = new QuizSection
            {
                Id = Guid.NewGuid().ToShortGuidString(),
                Title = "Introductie",
                QuizItemRefs = new List<QuizItemRef>
                {
                    new QuizItemRef(QuizItems[0])
                }
            };
            var quizSection1 = new QuizSection
            {
                Id = Guid.NewGuid().ToShortGuidString(),
                Title = "Vanalles 1",
                QuizItemRefs = new List<QuizItemRef>
                {
                    new QuizItemRef(QuizItems[1]),
                    new QuizItemRef(QuizItems[2]),
                    new QuizItemRef(QuizItems[3])
                }
            };

            var quizSection2 = new QuizSection
            {
                Id = Guid.NewGuid().ToShortGuidString(),
                Title = "Vanalles 2",
                QuizItemRefs = new List<QuizItemRef>
                {
                    new QuizItemRef(QuizItems[4]),
                    new QuizItemRef(QuizItems[5]),
                    new QuizItemRef(QuizItems[6])
                }
            };

            var quiz = new Quiz
            {
                Id = Guid.NewGuid().ToShortGuidString(),
                Title = "PéCé-pubquiz 2019"
            };
            quiz.QuizSections.Add(quizSectionIntro);
            quiz.QuizSections.Add(quizSection1);
            quiz.QuizSections.Add(quizSection2);

            return quiz;
        }

        private static QuizItem GetIntroItem()
        {
            var introItem = new QuizItem
            {
                Id = Guid.NewGuid().ToShortGuidString(),
                Body =
                    "Gebruik van apparatuur met een<br>scherm is (uiteraard) niet toegestaan.<br>Over de uitslag kan worden gecorrespondeerd,<br>nmaar de quizmaster heeft altijd gelijk.",
                Title = "Huisregels",
                QuizItemType = QuizItemType.Information
            };

            return introItem;
        }

        private static QuizItem GetS1Q1()
        {
            var question = new QuizItem
            {
                Id = Guid.NewGuid().ToShortGuidString(),
                Body = "Wat bereken je met een integraalberekening?",
                Title = "Vanalles 1 - 1",
                MaxScore = 1,
                QuizItemType = QuizItemType.ShortAnswer
            };

            question.Interactions.Add(new Interaction(0)
            {
                Text = "Beschrijving",
                Solution = new Solution(new[] {"oppervlakte onder een grafiek"}),
                InteractionType = InteractionType.ShortAnswer,
                MaxScore = 1
            });

            return question;
        }

        private static QuizItem GetS1Q2()
        {
            var question = new QuizItem
            {
                Id = Guid.NewGuid().ToShortGuidString(),
                Body =
                    "Bij Cito gebruiken we VPN-software genaamd Pulse Secure om thuis te werken. Waar staat VPN voor?",
                Title = "Vanalles 1 - 2",
                MaxScore = 1,
                QuizItemType = QuizItemType.ShortAnswer
            };

            question.Interactions.Add(new Interaction(0)
            {
                Text = "Betekenis",
                Solution = new Solution(new[] {"virtual private network"}),
                InteractionType = InteractionType.ShortAnswer,
                MaxScore = 1
            });

            return question;
        }

        private static QuizItem GetS1Q3()
        {
            var question = new QuizItem
            {
                Id = Guid.NewGuid().ToShortGuidString(),
                Body = "(filmpje)",
                Title = "Vanalles 1 - 3",
                MaxScore = 2,
                QuizItemType = QuizItemType.ShortAnswer
            };

            question.Interactions.Add(new Interaction(0)
            {
                Text = "Hoeveel hazen had Eliud Kipchoge bij zich tijdens zijn recordpoging?",
                Solution = new Solution(new[] {"7", "zeven"}),
                InteractionType = InteractionType.ShortAnswer,
                MaxScore = 1
            });
            question.Interactions.Add(new Interaction(1)
            {
                Text = "Wat is de naam van de sponsor van het evenement?",
                Solution = new Solution(new[] {"INEOS"}),
                InteractionType = InteractionType.ShortAnswer,
                MaxScore = 1
            });
            return question;
        }

        private static QuizItem GetS2Q1()
        {
            var question = new QuizItem
            {
                Id = Guid.NewGuid().ToShortGuidString(),
                Body = "Wat is de naam van de droogste champagnesoort?",
                Title = "Vanalles 2 - 1",
                MaxScore = 1,
                QuizItemType = QuizItemType.MultipleChoice
            };

            question.Interactions.Add(new Interaction(0)
            {
                Text = "Choose",
                ChoiceOptions = new List<ChoiceOption>
                {
                    new ChoiceOption(0, "Brut"),
                    new ChoiceOption(1, "Sec"),
                    new ChoiceOption(2, "Doux"),
                },
                Solution = new Solution(new[] {1}),
                InteractionType = InteractionType.MultipleChoice,
                MaxScore = 1
            });

            return question;
        }

        private static QuizItem GetS2Q2()
        {
            var question = new QuizItem
            {
                Id = Guid.NewGuid().ToShortGuidString(),
                Body = "Hoe heet de schrijver van ‘The Hitchhiker's Guide to the Galaxy’?",
                Title = "Vanalles 2 - 2",
                MaxScore = 1,
                QuizItemType = QuizItemType.ShortAnswer
            };

            question.Interactions.Add(new Interaction(0)
            {
                Text = "Naam",
                Solution = new Solution(new[] {"Douglas Adams"}),
                InteractionType = InteractionType.ShortAnswer,
                MaxScore = 1
            });

            return question;
        }

        private static QuizItem GetS2Q3()
        {
            var question = new QuizItem
            {
                Id = Guid.NewGuid().ToShortGuidString(),
                Body = "(audiofragment)",
                Title = "Vanalles 2 - 3",
                MaxScore = 3,
                QuizItemType = QuizItemType.ShortAnswer
            };

            question.Interactions.Add(new Interaction(0)
            {
                Text = "Titel",
                Solution = new Solution(new[] {"Personal Jesus"}),
                InteractionType = InteractionType.ShortAnswer,
                MaxScore = 1
            });
            question.Interactions.Add(new Interaction(1)
            {
                Text = "Uitvoerende artiest",
                Solution = new Solution(new[] {"Johnny Cash"}),
                InteractionType = InteractionType.ShortAnswer,
                MaxScore = 1
            });
            question.Interactions.Add(new Interaction(2)
            {
                Text = "Originele artiest",
                Solution = new Solution(new[] {"Depeche Mode"}),
                InteractionType = InteractionType.ShortAnswer,
                MaxScore = 1
            });

            return question;
        }
    }
}