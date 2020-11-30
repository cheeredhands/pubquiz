import { UserRole, GameState, QuizRef } from './models';

export interface ApiResponse {
    errorCode: string;
    code: string;
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
    name: string;
    memberNames: string;
    recoveryCode: string;
  }

export interface WhoAmIResponse extends ApiResponse {
    userName: string;
    userId: string;
    name: string;
    memberNames: string;
    currentGameId: string;
    gameState: GameState;
    userRole: UserRole;
  }

export interface SaveTeamMembersResponse extends ApiResponse {
    teamMembers: string;
  }

export interface NavigateItemResponse extends ApiResponse {
    quizItemId: string;
  }

export interface ImportZippedExcelQuizResponse extends ApiResponse {
  quizRefs: QuizRef[];
}
