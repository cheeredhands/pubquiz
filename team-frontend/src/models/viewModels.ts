import { GameState, QuizItemType, Team, Game, QuizItem } from './models';

export interface TeamLobbyViewModel {
    game: Game;
    otherTeamsInGame: Team[];
}

export interface QmLobbyViewModel {
    userId: string;
    game: Game;
    teamsInGame: Team[];
}

export interface QmInGameViewModel {
    qmTeamFeed: {};
    qmTeamRanking: {};
    game: Game;
    currentQuizItem: QuizItem;
    // teamfeed
    // game info
    // current quiz itm
    // ranking
}