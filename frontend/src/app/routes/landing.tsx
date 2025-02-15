import { EntriesTable } from '@/features/entries/components/EntriesTable';
import { useAuth } from 'react-oidc-context';

const LandingRoute = () => {
  const auth = useAuth();

  return (
    <div className='grid grid-cols-3 gap-4'>
      <div></div>
      <EntriesTable />
      <div></div>
    </div>
  );
};

export default LandingRoute;