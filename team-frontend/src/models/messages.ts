import { GameState } from './models';

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
  