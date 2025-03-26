import { TableRow } from '@/components/Table';

export interface Character extends TableRow {
  id: string;
  firstName: string;
  lastName: string;
  middleNames?: string;
  description?: string;
  user?: User;
  createdBy: User;
  createdOnUtc: Date;
  tags:Tag[];
};

export interface Comment extends TableRow {
  id:string;
  content:string;
  createdAtUtc:Date;
  deletedAtUtc:Date;
  createdBy:User;
};

export interface Document extends TableRow {
  id: string
};

export interface Entry extends TableRow {
  id:string;
  name:string;
  description:string;
  createdAtUtc:Date;
  deletedAtUtc:Date;
  createdBy:User;
  document:Document;
  characters:Character[];
  comments:Comment[];
  tags:Tag[];
};

export interface Tag extends TableRow {
  id:string;
  name:string;
};

export interface User extends TableRow {
  id: string;
  firstName: string;
  lastName: string;
  middleNames?: string;
};