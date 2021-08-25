import { UserRole, GameState } from './models';
import { QuizViewModel } from './viewModels';

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
    recoveryCode: string;
  }

export interface SaveTeamMembersResponse extends ApiResponse {
    teamMembers: string;
  }

export interface NavigateItemResponse extends ApiResponse {
    quizItemId: string;
  }

export interface ImportZippedExcelQuizResponse extends ApiResponse {
  quizViewModels: QuizViewModel[];
}
