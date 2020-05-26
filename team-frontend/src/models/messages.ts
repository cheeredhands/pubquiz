import { GameState, Answer, Team } from './models';

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

  export interface AnswerScoredMessage {
      teamId: string;
      gameId: string;
      quizItemId: string;
      interactionId: number;
      totalTeamScore: number;
      scorePerQuizSection: Record<string, number>;
      answer: Answer;
  }
  