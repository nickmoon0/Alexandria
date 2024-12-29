export interface Character {
  id: string;
  firstName: string;
  lastName: string;
  middleNames?: string;
  description?: string;
  user?: User;
  createdBy: User;
  createdOnUtc: Date;
};

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  middleNames?: string;
};