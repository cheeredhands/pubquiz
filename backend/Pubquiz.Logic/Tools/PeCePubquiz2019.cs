using System;
using System.Collections.Generic;
using System.Reflection;
using Pubquiz.Domain.Models;
using Pubquiz.Persistence.Extensions;

namespace Pubquiz.Logic.Tools
{
    public class PeCePubquiz2019
    {
        public List<QuizItem> QuizItems { get; set; }

        public PeCePubquiz2019(string mediaBaseUrl)
        {
            QuizItems = new List<QuizItem>
            {
                GetIntroItem(mediaBaseUrl),
                GetS1Q1(),
                GetS1Q2(),
                GetS1Q3(),
                GetS1Q4(mediaBaseUrl),
                GetS2Q1(),
                GetS2Q2(),
                GetS2Q3(mediaBaseUrl)
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
                    new QuizItemRef(QuizItems[3]),
                    new QuizItemRef(QuizItems[4])
                }
            };

            var quizSection2 = new QuizSection
            {
                Id = Guid.NewGuid().ToShortGuidString(),
                Title = "Vanalles 2",
                QuizItemRefs = new List<QuizItemRef>
                {
                    new QuizItemRef(QuizItems[5]),
                    new QuizItemRef(QuizItems[6]),
                    new QuizItemRef(QuizItems[7])
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

        private static QuizItem GetIntroItem(string baseUrl)
        {
            var introItem = new QuizItem
            {
                Id = Guid.NewGuid().ToShortGuidString(),
                Body =
                    "Gebruik van apparatuur met een<br>scherm is (uiteraard) niet toegestaan.<br>Over de uitslag kan worden gecorrespondeerd,<br>nmaar de quizmaster heeft altijd gelijk.",
                Title = "Huisregels",
                QuizItemType = QuizItemType.Information,
                MediaObjects = new List<MediaObject>
                    {new MediaObject($"{baseUrl}/mediaobjects/welcome.jpg", MediaType.Image) {Title = "Welcome image"}}
            };

            return introItem;
        }

        private static QuizItem GetS1Q1()
        {
            var question = new QuizItem
            {
                Id = Guid.NewGuid().ToShortGuidString(),
                Body = "Wat bereken je met een integraalberekening?",
                Title = "Vanalles 1 - vraag 1",
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
                Title = "Vanalles 1 - vraag 2",
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
                Title = "Vanalles 1 - vraag 3",
                MaxScore = 2,
                QuizItemType = QuizItemType.ShortAnswer
            };

            question.Interactions.Add(new Interaction(0)
            {
                Text = "Hoeveel hazen had Eliud Kipchoge bij zich tijdens zijn recordpoging?",
                Solution = new Solution(new[] {"7", "zeven"}, levenshteinTolerance:0),
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
        
        private static QuizItem GetS1Q4(string baseUrl)
        {
            var question = new QuizItem
            {
                Id = Guid.NewGuid().ToShortGuidString(),
                Body = "(filmpje)",
                Title = "Vanalles 1 - vraag 4",
                MaxScore = 2,
                QuizItemType = QuizItemType.ShortAnswer,
                MediaObjects = new List<MediaObject>{new MediaObject($"{baseUrl}/mediaobjects/s1q4.mp4", MediaType.Video)}
            };

            question.Interactions.Add(new Interaction(0)
            {
                Text = "Hoe heet de componist?",
                Solution = new Solution(new[] {"Brahms", "Johannes Brahms"}),
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
                Title = "Vanalles 2 - vraag 1",
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
                Title = "Vanalles 2  vraag 2",
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

        private static QuizItem GetS2Q3(string baseUrl)
        {
            var question = new QuizItem
            {
                Id = Guid.NewGuid().ToShortGuidString(),
                Body = "(audiofragment)",
                Title = "Vanalles 2 - vraag 3",
                MaxScore = 3,
                QuizItemType = QuizItemType.ShortAnswer,
                MediaObjects = new List<MediaObject> {new MediaObject($"{baseUrl}/mediaobjects/s2q3.mp3", MediaType.Audio)}
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