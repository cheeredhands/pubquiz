import { GameState, Answer, Team } from './models';
import { GameViewModel, QmLobbyViewModel } from './viewModels';

export interface TeamConnectionChangedMessage {
        /** The team Id */
        teamId: string;
        /** The game Id */
        gameId: string;
        /** The Team Name */
        name: string;
        /** Connection count */
        connectionCount: number;
}

export interface TeamRegisteredMessage {
    /** The team Id */
    teamId: string;
    /** The game Id */
    gameId: string;
    /** The Team Name */
    name: string;
    /** Team member names */
    memberNames: string;
}

export interface QmTeamRegisteredMessage {
    team: Team;
    /** The game Id */
    gameId: string;
}

export interface TeamDeletedMessage {
    /** The team Id */
    teamId: string;
    /** The game Id */
    gameId: string;
}

export interface TeamLoggedOutMessage {
    /** The team Id */
    teamId: string;
    /** The game Id */
    gameId: string;
    /** The Team Name */
    name: string;
}

export interface ItemNavigatedMessage {
    gameId: string;
    newSectionId: string;
    newSectionTitle: string;
    newQuizItemId: string;
    newSectionIndex: number;
    newSectionQuizItemCount: number;
    newQuizItemIndexInSection: number;
    newQuizItemIndexInTotal: number;
    newQuestionIndexInTotal: number;
  }

export interface TeamMembersChangedMessage {
    /** The team Id */
    teamId: string;
    /** The current game Id */
    gameId: string;
    /** The Team Name */
    name: string;
    /** Team member names */
    memberNames: string;
}

export interface TeamNameUpdatedMessage {
    /** The team Id */
    teamId: string;
    /** The current game Id */
    gameId: string;
    /** The Team Name */
    oldName: string;
    /** The Team Name */
    name: string;
}

export interface GameStateChangedMessage {
    gameId: string;
    /** The old game state */
    oldGameState: GameState;
    /** The new game state */
    newGameState: GameState;
  }

export interface InteractionResponseAddedMessage {
      teamId: string;
      quizItemId: string;
      interactionId: number;
      response: string;
  }

export interface InteractionCorrectedMessage {
     gameId: string;
     teamId: string;
     quizItemId: string;
     interactionId: string;
     outcome: boolean;
}

export interface AnswerScoredMessage {
    teamId: string;
    gameId: string;
    quizItemId: string;
    interactionId: number;
    totalTeamScore: number;
    scorePerQuizSection: Record<string, number>;
    answer: Answer;
  }

export interface GameSelectedMessage {
    userId: string;
    gameId: string;
    newGameId: string;
    qmLobbyViewModel: QmLobbyViewModel;
}

export interface GameCreatedMessage {
    userId: string;
    qmGameViewModel: GameViewModel;
}
