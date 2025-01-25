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

export interface Entry {
  id:string,
  name:string,
  description:string,
  createdAtUtc:Date,
  deletedAtUtc:Date,
  createdBy:User,
  tags:Tag[],
};

export interface Tag {
  id:string,
  name:string,
};

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  middleNames?: string;
};