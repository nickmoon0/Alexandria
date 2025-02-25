import { Route } from '@/types/config';

export const paths: Record<string, Route> = {
  entry: {
    path: '/entry/:entryId',
    getHref: (entryId?:string) => `/entry/${entryId}`
  },
  entries: {
    path: '/entries',
    getHref: () => '/entries',
  },
  character: {
    path: '/character/:characterId',
    getHref: (characterId?:string) => `/character/${characterId}`
  },
  characters: {
    path: '/characters',
    getHref: () => '/characters'
  }
} as const;