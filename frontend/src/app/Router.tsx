import React, { useMemo } from 'react';
import { createBrowserRouter, Navigate } from 'react-router';
import { RouterProvider } from 'react-router';
import { paths } from '@/config/paths';
import EntriesRoute from '@/app/routes/entries/Entries';
import EntryRoute from '@/app/routes/entries/Entry';
import CharacterRoute from '@/app/routes/characters/Character';
import CharactersRoute from '@/app/routes/characters/Characters';
import Layout from '@/app/Layout';

const createAppRouter = () => 
  createBrowserRouter([
    {
      path: '/',
      element: <Layout />,
      children: [
        {
          index: true,
          element: <Navigate to={paths.entries.path} replace />
        },
        {
          path: paths.entries.path,
          element: <EntriesRoute />
        },
        {
          path: paths.entry.path,
          element: <EntryRoute />
        },
        {
          path: paths.character.path,
          element: <CharacterRoute />
        },
        {
          path: paths.characters.path,
          element: <CharactersRoute />
        }
      ]
    },
  ]);

export const AppRouter = () => {
  const router = useMemo(() => createAppRouter(), []);
  return <RouterProvider router={router} />;
};