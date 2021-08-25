import { GameState, QuizItemType, Team, Game, QuizItem, InteractionType, ChoiceOption, MediaObject } from './models';

export interface GameViewModel {
    id: string;
    title: string;
    quizId: string;
    quizTitle: string;
    inviteCode: string;
    gameState: GameState;
  }

export interface QuizViewModel {
    id: string;
    title: string;
  }

export interface TeamViewModel {
    /** The team Id */
    id: string;
    /** The Team Name */
    name: string;
    /** Team member names */
    memberNames: string;
    /** false when the user has logged out / left game */
    isLoggedIn: boolean;
    /** number of signalr connections (concurrent users logged in for the same team) */
    connectionCount: number;
}

// export interface QmTeamViewModel{

// }

export interface TeamLobbyViewModel {
    game: Game;
    otherTeamsInGame: TeamViewModel[];
}

export interface InteractionViewModel {
    id: number;
    text: string;
    interactionType: InteractionType;
    choiceOptions: ChoiceOption[];
    maxScore: number;
    response: string;
    chosenOptions: number[];
    chosenOption: number;
}

export interface QuizItemViewModel {
    id: string;
    title: string;
    body: string;
    media: [];// MediaObject[];
    quizItemType: QuizItemType;
    maxScore: number;
    interactions: InteractionViewModel[];
    mediaObjects: MediaObject[];
}

export interface TeamInGameViewModel {
    game: Game;
    quizItemViewModel: QuizItemViewModel;
}

export interface QmLobbyViewModel {
    userId: string;
    game: Game;
    teamsInGame: Team[];
    quizViewModels: QuizViewModel[];
    gameViewModels: GameViewModel[];
}

export interface QmInGameViewModel {
    game: Game;
    currentQuizItem: QuizItem;
    teams: Team[];
}

export interface TeamFeedViewModel {
    teams: Team[];
}

export interface TeamRankingViewModel {
    teams: Team[];
}
