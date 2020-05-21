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
    qmTeamFeed: TeamFeedViewModel;
    qmTeamRanking: TeamRankingViewModel;
    game: Game;
    currentQuizItem: QuizItem;
    // teamfeed
    // game info
    // current quiz itm
    // ranking
}

export interface TeamFeedViewModel {
    teams: Team[];
}

export interface TeamRankingViewModel {
    teams: Team[];
}