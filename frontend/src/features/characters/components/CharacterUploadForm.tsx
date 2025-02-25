import Button from '@/components/Buttons/Button';
import TextArea from '@/components/Input/TextArea';
import TextInput from '@/components/Input/TextInput';
import Popup from '@/components/Popup';
import { ToastType } from '@/components/Toast';
import { useToast } from '@/hooks/ToastContext';
import React, { useState } from 'react';
import { createCharacter } from '@/features/characters/api/create-character';

export interface CharacterUploadFormProps {
  onClose: () => void;
}

const CharacterUploadForm = ({ onClose }: CharacterUploadFormProps) => {
  const [firstNameValue, setFirstNameValue] = useState<string | null>(null);
  const [lastNameValue, setLastNameValue] = useState<string | null>(null);
  const [middleNameValue, setMiddleNameValue] = useState<string | null>(null);
  const [descriptionValue, setDescriptionValue] = useState<string | null>(null);

  const { showToast } = useToast();

  const handleUpload = async () => {
    try {
      await createCharacter({
        firstName: firstNameValue ?? '',
        lastName: lastNameValue ?? '',
        middleNames: middleNameValue,
        description: descriptionValue
      });

      onClose();
    } catch (error) {
      console.error(error);
      showToast('Failed to create character', ToastType.Error);
    }
  };

  const CreateControl = (
    <Button onClick={handleUpload}>
      Create
    </Button>
  );

  return (
    <div>
      <Popup 
        title={'New Character'}
        onClose={onClose}
        controlChildren={CreateControl}
        controlChildrenOnLeft={false}
      >
        <div className='grid grid-cols-1 gap-2 pt-4'>
          <TextInput value={firstNameValue ?? ''} onChange={setFirstNameValue} placeholder='First Name' className='w-full' />
          <TextInput value={lastNameValue ?? ''} onChange={setLastNameValue} placeholder='Last Name' className='w-full' />
          <TextInput value={middleNameValue ?? ''} onChange={setMiddleNameValue} placeholder='Middle Names' className='w-full' />
          <TextArea value={descriptionValue ?? ''} onChange={setDescriptionValue} placeholder='Entry Description' className='w-full' />
        </div>
      </Popup>
    </div>
  );
};

export default CharacterUploadForm;