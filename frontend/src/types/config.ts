export type Route = {
  path: string;
  getHref: (param?:string) => string;
}