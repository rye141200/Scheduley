export interface ScheduleyJWTPayload {
  email: string;
  unique_name: string;
  role: string;
  CreatedAt: string;
  LastLogin: string;
  nbf: number;
  exp: number;
  iat: number;
}
