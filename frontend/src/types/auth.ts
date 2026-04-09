export type LoginRequest = {
  username: string;
  password: string;
};

export type LoginResponse = {
  accessToken: string;
  expiresAtUtc: string;
  username: string;
  fullName: string;
  role: string;
};

export type AuthUser = {
  username: string;
  fullName: string;
  role: string;
};