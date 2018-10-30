export interface TeamInfo {
  /** The team Id (Guid) */
  teamId: string;
  /** The Team Name */
  teamName: string;
}

/** The quiz object used  */
export interface Quiz {
  team?: TeamInfo;
  teams: TeamInfo[];
}
