import { GameState, QuizItemType, Team, Game } from './models';

export interface TeamLobbyViewModel {
    team: Team;
    otherTeamsInGame: Team[];
}

export interface QmLobbyViewModel {
    userId: string;
    game: Game;
    teamsInGame: Team[];
}

export interface QmInGameViewModel {
    teamFeed: {};
    game: Game;
    // teamfeed
    // game info
    // current quiz itm
    // ranking
}