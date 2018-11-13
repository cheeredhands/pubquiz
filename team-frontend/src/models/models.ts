export interface TeamInfo {
  /** The team Id (Guid) */
  teamId: string;
  /** The Team Name */
  teamName: string;
  /** Team member names */
  memberNames: string;
}

export interface WhoAmIResponse {
  userName: string;
  userId: string;
  currentGameId: string;
  userRole: UserRole;
}

export interface ApiResponse {
  code: number;
  message: string;
}

export interface TeamLobbyViewModel {
  userId: string;
  team: TeamInfo;
  otherTeamsInGame: TeamInfo[];
}

/** The quiz object used  */
export interface Quiz {
  title: string;
}

export enum UserRole {
  Team,
  QuizMaster,
  Admin
}
