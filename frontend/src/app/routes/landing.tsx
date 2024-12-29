import { api } from '@/lib/api-client';
import { Character } from '@/types/app';
import { useEffect, useState } from 'react';
import { useAuth } from 'react-oidc-context';

const LandingRoute = () => {
  const auth = useAuth();
  const [character, setCharacter] = useState<Character | null>(null);

  useEffect(() => {
    const fetchCharacter = async () => {
      const response = await api.get<Character>('/character/05893921-2d03-4b36-af44-401b2ba4021a');
      setCharacter(response.data);
    };

    fetchCharacter();
  }, []);

  return (
    <div>
      <h1 className='text-3xl font-bold underline'>Landing</h1>
      <p>{ auth?.isAuthenticated ? 'Authenticated' : 'Not Authenticated' }</p>

      <table className='table-auto text-left w-1/2'>
        <thead>
          <tr>
            <th>ID</th>
            <th>First Name</th>
            <th>Last Name</th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td>{character?.id}</td>
            <td>{character?.firstName}</td>
            <td>{character?.lastName}</td>
          </tr>
        </tbody>
      </table>
    </div>
  );
};

export default LandingRoute;