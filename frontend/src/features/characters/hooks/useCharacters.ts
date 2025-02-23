import { ToastType } from '@/components/Toast';
import { useToast } from '@/hooks/ToastContext';
import { getCharacter } from '@/features/characters/api/get-character';

export const useCharacters = () => {
  const { showToast } = useToast();
  
  const retrieveCharacter = async (characterId:string) => {
    try {
      const character = await getCharacter({ characterId });
      return character;
    } catch (error) {
      console.error(error);
      showToast('Failed to retrieve character', ToastType.Error);
    }
  };

  return {
    retrieveCharacter
  };
};