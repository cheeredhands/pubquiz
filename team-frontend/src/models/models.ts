export interface TeamInfo {
  /** The team Id (Guid) */
  teamId: string;
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
  /** false when the team has logged out / left game */
  isLoggedIn: boolean;
  /** the game Id */
  gameId: string;
}

export interface ApiResponse {
  code: number;
  message: string;
}

export interface WhoAmIResponse {
  userName: string;
  userId: string;
  currentGameId: string;
  userRole: UserRole;
}

export interface SaveTeamMembersResponse extends ApiResponse {
  teamMembers: string;
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
