export interface TeamInfo {
  /** The team Id (Guid) */
  teamId: string;
  /** The Team Name */
  teamName: string;
}

export interface WhoAmIResponse {
  userName: string;
  userId: string;
  currentGameId: string;
  userRole: UserRole;
}

/** The quiz object used  */
export interface Quiz {
  team?: TeamInfo;
  teams: TeamInfo[];
}

export enum UserRole {
  Team,
  QuizMaster,
  Admin
}
