import React from 'react';
import { useEffect, useState } from 'react';
import { useParams } from 'react-router';
import { Entry } from '@/types/app';
import { getEntry, GetEntryOptions } from '@/features/entries/api/get-entry';
import MediaViewer from '@/components/MediaViewer';
import TagList from '@/features/tags/components/TagList';
import TextArea from '@/components/Input/TextArea';
import Button from '@/components/Buttons/Button';
import { createComment } from '@/features/comments/api/create-comment';
import { getComments } from '@/features/comments/api/get-comments';
import { useToast } from '@/hooks/ToastContext';
import { ToastType } from '@/components/Toast';
import MetadataTag from '@/components/MetadataTag';
import CommentList from '@/features/comments/components/CommentList';

const EntryRoute = () => {
  const [entry, setEntry] = useState<Entry | null>(null);
  const [commentValue, setCommentValue] = useState<string | null>(null);

  const { entryId } = useParams();
  const { showToast } = useToast();

  const retrieveEntry = async (entryId:string) => {
    try {
      const response = await getEntry({
        entryId,
        options: [
          GetEntryOptions.IncludeComments,
          GetEntryOptions.IncludeDocument,
          GetEntryOptions.IncludeTags,
          GetEntryOptions.IncludeCharacters
        ]
      });
  
      setEntry(response);
    } catch (error) {
      console.error(error);
      showToast('Failed to retrieve entry', ToastType.Error);
    }
  };

  const retrieveComments = async (entryId:string) => {
    try {
      const response = await getComments({entryId});
      setEntry((prevEntry) => {
        if (!prevEntry) return null;
  
        return {...prevEntry,
        comments: response
        };
      });
    } catch (error) {
      console.error(error);
      showToast('Failed to retrieve comments', ToastType.Error);
    }
  };

  const addComment = async () => {
    try {
      if (entryId) {
        await createComment({ content:commentValue ?? '', entryId });
        retrieveComments(entryId);
        setCommentValue(null);
      };
    } catch (error) {
      console.error(error);
      showToast('Failed to add comment', ToastType.Error);
    }
  };

  const textAreaKeyDown = (e:React.KeyboardEvent<HTMLTextAreaElement>) => {
    console.log(e.key);
    if (e.key.toLowerCase() === 'enter' && !e.shiftKey) {
      e.preventDefault();
      addComment();
    }
  };

  useEffect(() => {
    if (entryId) {
      retrieveEntry(entryId);
    }
  }, [entryId]);

  if (!entry) {
    return <div>Loading...</div>;
  };

  return (
    <div className='grid grid-cols-2'>
      
      <div>
        <h1 className='text-2xl font-bold text-gray-800'>{entry.name}</h1>
      </div>

      <div></div>

      <div>
        { entry.tags && <TagList tagListClassName={'pt-1'} tags={entry.tags} /> }
        <p className='text-lg text-gray-700 pb-3'>{entry.description}</p>
      </div>
      
      <div className='flex items-start'>
        <p className="text-lg text-gray-700 self-end pb-3">Comments</p>
      </div>
      
      <div className='pr-4 flex flex-col items-start'>
        <div>
          <MediaViewer 
            className='max-h-[65vh]'
            documentId={entry.document.id} />
        </div>
        {entry?.createdBy && (
          <div className='w-full py-4'>
            <MetadataTag
              createdBy={entry.createdBy}
              createdAtUtc={entry.createdAtUtc}
              id={entry.id} />
          </div>
        )}
      </div>

      <div>
        <div className='flex gap-2 items-start'>
          <TextArea
            className='flex-1 resize-none'
            value={commentValue ?? ''}
            onChange={setCommentValue}
            onKeyDown={textAreaKeyDown}
          />
          <Button className='self-start' onClick={addComment}>Post</Button>
        </div>
        <div className='mt-4'>
          <CommentList 
              className='max-h-[65vh]'
              comments={entry.comments}/>
        </div>
      </div>
    </div>
  );
};

export default EntryRoute;