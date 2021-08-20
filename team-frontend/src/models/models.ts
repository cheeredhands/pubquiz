import { GameViewModel, QuizViewModel } from './viewModels';

export enum GameState {
  Closed = 'Closed',
  Open = 'Open',
  Running = 'Running',
  Reviewing = 'Reviewing',
  Paused = 'Paused',
  Finished = 'Finished'
}

export interface User {
  /** The user Id */
  userId: string;
  /** The user name */
  userName: string;
  /** false when the user has logged out / left game */
  isLoggedIn: boolean;
  /** the current game Id */
  currentGameId: string;
  /** the quizzes this user is an owner of  */
  quizViewModels: QuizViewModel[];
  /** the games this user is involved in */
  gameViewModels: GameViewModel[];
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

export interface Game {
  id: string;
  title: string;
  quizTitle: string;
  state: GameState;
  totalQuestionCount: number;
  totalQuizItemCount: number;
  currentSectionQuizItemCount: number;
  currentSectionIndex: number;
  currentSectionId: string;
  currentSectionTitle: string;
  currentQuizItemId: string;
  currentQuizItemType: QuizItemType;
  currentQuizItemTitle: string;
  currentQuizItemIndexInSection: number;
  currentQuizItemIndexInTotal: number;
  currentQuestionIndexInTotal: number;
}

export interface QuizItemRef {
  id: string;
  title: string;
  itemType: QuizItemType;
  body: string;
}

export interface Solution {
  choiceOptionIds: number[];
  responses: string[];
}

export interface ChoiceOption {
  id: string;
  text: string;
}

export enum InteractionType {
  MultipleChoice = 'MultipleChoice',
  MultipleResponse = 'MultipleResponse',
  ShortAnswer = 'ShortAnswer',
  ExtendedText = 'ExtendedText'
}

export interface Interaction {
  id: number;
  text: string;
  interactionType: InteractionType;
  choiceOptions: ChoiceOption[];
  maxScore: number;
  solution: Solution;
}

export enum MediaType {

  Image = 'Image',
  Video = 'Video',
  Audio = 'Audio',
  Markdown = 'Markdown'
}

export interface MediaDimensions {
  originalWidth: number;
  originalHeight: number;
  width: number;
  height: number;
  durationInSeconds: number;
}

export interface MediaObject {
  id: string;
  title: string;
  uri: string;
  text: string;
  dimensions: MediaDimensions;
  mediaType: MediaType;
  mimeType: string;
  isSolution: boolean;
  teamVisible: boolean;
}

export interface QuizItem {
  id: string;
  title: string;
  body: string;
  mediaObjects: MediaObject[];
  quizItemType: QuizItemType;
  maxScore: number;
  interactions: Interaction[];
}

export interface InteractionResponse {
  interactionId: number;
  choiceOptionIds: number[];
  response: string;

  flaggedForManualCorrection: boolean;
  /** true if the answer was manually corrected */
  manuallyCorrected: boolean;
  /** wether or not the answer  was deemed correct by the manual scorer */
  manualCorrectionOutcome: boolean;
  /** awarded score for this part of the question (interaction) */
  awardedScore: number;
}

export interface Answer {
  quizSectionId: string;
  quizItemId: string;
  interactionResponses: InteractionResponse[];
  totalScore: number;
  flaggedForManualCorrection: boolean;
}

export enum UserRole {
  Team = 'Team',
  QuizMaster = 'QuizMaster',
  Admin = 'Admin'
}

export interface Team {
  /** The team Id */
  id: string;
  /** The game Id */
  gameId: string;
  /** The Team Name */
  name: string;
  /** Team member names */
  memberNames: string;
  /** recovery code */
  recoveryCode: string;
  /** false when the user has logged out / left game */
  isLoggedIn: boolean;
  /** number of signalr connections (concurrent users logged in for the same team) */
  connectionCount: number;
  /** Score per quiz section */
  scorePerQuizSection: Record<string, number>;
  totalScore: number;
  answers: Record<string, Answer>;
}
