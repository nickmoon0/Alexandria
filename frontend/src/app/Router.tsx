import React, { useMemo } from 'react';
import { createBrowserRouter } from 'react-router';
import { RouterProvider } from 'react-router';
import { paths } from '@/config/paths';
import EntryTableRoute from '@/app/routes/entries/EntryTable';
import EntryRoute from '@/app/routes/entries/Entry';
import CharacterRoute from '@/app/routes/characters/Character';
import CharacterTableRoute from '@/app/routes/characters/CharacterTable';
import Layout from '@/app/Layout';

const createAppRouter = () => 
  createBrowserRouter([
    {
      path: '/',
      element: <Layout />,
      children: [
        {
          path: paths.home.path,
          element: <EntryTableRoute />
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
          element: <CharacterTableRoute />
        }
      ]
    },
  ]);

export const AppRouter = () => {
  const router = useMemo(() => createAppRouter(), []);
  return <RouterProvider router={router} />;
};