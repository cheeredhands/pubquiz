import { ResultCode } from './ResultCode';

export interface User {
  /** The user Id */
  userId: string;
  /** The user name */
  userName: string;
  /** false when the user has logged out / left game */
  isLoggedIn: boolean;
  /** the current game Id */
  currentGameId: string;
  /** the game ids this user is involved in */
  gameIds: string[];
}

export interface Game {
  id: string;
  title: string;
  state: GameState;
  totalQuestionCount: number;
  totalQuizItemCount: number;
  currentSectionQuizItemCount: number;
  currentSectionIndex: number;
  currentSectionId: string;
  currentQuizItemId: string;
  currentQuizItemType: QuizItemType;
  currentQuizItemTitle: string;
  currentQuizItemIndexInSection: number;
  currentQuizItemIndexInTotal: number;
  currentQuestionIndexInTotal: number;
}

export interface Team {
  /** The team Id */
  teamId: string;
  /** The game Id */
  gameId: string;
  /** The Team Name */
  teamName: string;
  /** Team member names */
  memberNames: string;
  /** the current game Id */
  currentGameId: string;
  /** false when the user has logged out / left game */
  isLoggedIn: boolean;
}

export interface QuizItemRef {
  id: string;
  title: string;
  itemType: QuizItemType;
  body: string;
}

export interface QuizItem {
  id: string;
  title: string;
  body: string;
  media: []// MediaObject[];
  quizItemType: QuizItemType;
  maxScore: number;
  interactions: []; // Interaction[];
}

export interface Interaction {
  id: string;
  text: string;
  interactionType: InteractionType;
}

/** Type of the question */
export enum QuizItemType {
  /** Multiple options, one correct answer */
  MultipleChoice = 'MultipleChoice',
  /** Multiple options, more than one answer to be chosen for max score */
  MultipleResponse = 'MultipleResponse',
  /** One line answer, often automatically scoreable */
  ShortAnswer = 'ShortAnswer',
  /** Multiline answer, usually not automatically scoreable */
  ExtendedText = 'ExtendedText',
  /** Mixed (multiple interaction at the question level) */
  Mixed = 'Mixed',
  /** An informational quiz item, so not a question. Can be used as a divider between rounds or as a header and footer of the quiz. */
  Information = 'Information'
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

export enum GameState {
  Closed = 'Closed',
  Open = 'Open',
  Running = 'Running',
  Paused = 'Paused',
  Finished = 'Finished'
}

export enum InteractionType {
  MultipleChoice = 'MultipleChoice',
  MultipleResponse = 'MultipleResponse',
  ShortAnswer = 'ShortAnswer',
  ExtendedText = 'ExtendedText'
}

export enum UserRole {
  Team = 'Team',
  QuizMaster = 'QuizMaster',
  Admin = 'Admin'
}
