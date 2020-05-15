import { ResultCode } from './ResultCode';

export interface TeamInfo {
  /** The team Id (Guid) */
  teamId: string;
  /** The current game Id */
  currentGameId: string;
  /** The Team Name */
  teamName: string;
  /** false when the team has logged out / left game */
  isLoggedIn: boolean;
  /** Team member names */
  memberNames: string;
}

export interface UserInfo {
  /** The team Id (Guid) */
  userId: string;
  /** The Team Name */
  userName: string;
  /** false when the user has logged out / left game */
  isLoggedIn: boolean;
  /** the current game Id */
  currentGameId: string;
  /** the game ids this user is involved in */
  gameIds: string[];
}

export interface GameInfo {
  gameId: string;
  gameTitle: string;
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

export interface QuizItemInfo {
  id: string;
  title: string;
  itemType: QuizItemType;
  body: string;
}

export interface QuizItem {
  id: string;
  title: string;
  body: string;
  media: []//MediaObject[];
  quizItemType: QuizItemType;
  maxScore: number;
   interactions: []; //Interaction[];
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

export interface ItemNavigationInfo {
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

export interface GameStateChanged {
  /** The old game state */
  oldGameState: GameState;
  /** The new game state */
  newGameState: GameState;
}

export interface ApiResponse {
  errorCode: ResultCode;
  code: ResultCode;
  message: string;
}

export interface LoginResponse extends ApiResponse {
  jwt: string;
  userId: string;
  userName: string;
  currentGameId: string;
  gameIds: string[];
}

export interface RegisterForGameResponse extends ApiResponse {
  jwt: string;
  teamId: string;
  gameId: string;
  teamName: string;
  memberNames: string;
}

export interface WhoAmIResponse extends ApiResponse {
  userName: string;
  userId: string;
  currentGameId: string;
  userRole: UserRole;
}

export interface SaveTeamMembersResponse extends ApiResponse {
  teamMembers: string;
}

export interface NavigateItemResponse extends ApiResponse {
  quizItemId: string;
}

export interface TeamLobbyViewModel {
  userId: string;
  team: TeamInfo;
  otherTeamsInGame: TeamInfo[];
}

export interface QmLobbyViewModel {
  userId: string;
  currentGame: GameViewModel;
  teamsInGame: TeamInfo[];
}

export interface GameViewModel {
  gameId: string;
  gameTitle: string;
  state: GameState;
}

export enum UserRole {
  Team = 'Team',
  QuizMaster = 'QuizMaster',
  Admin = 'Admin'
}
