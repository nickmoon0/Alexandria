import { useEffect, useState } from 'react';
import { useParams } from 'react-router';
import { Entry } from '@/types/app';
import { getEntry, GetEntryOptions } from '@/features/entries/api/get-entry';
import { formatDateTime } from '@/lib/helpers';
import MediaViewer from '@/components/MediaViewer';
import TagList from '@/features/tags/components/TagList';
import TextArea from '@/components/Input/TextArea';
import Button from '@/components/Button';
import CommentBlock from '@/features/comments/components/CommentBlock';
import { createComment } from '@/features/comments/api/create-comment';
import { getComments } from '@/features/comments/api/get-comments';
import { useToast } from '@/hooks/ToastContext';
import { ToastType } from '@/components/Toast';

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
      showToast('Failed to add comment', ToastType.Error);
    }
  };

  const textAreaKeyDown = (e:React.KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === 'enter' && !e.shiftKey) {
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
    <div className='grid grid-cols-8 gap-4'>
      <div></div>
      <div className="col-span-3">
        <h1 className='text-2xl font-bold text-gray-800'>{entry.name}</h1>
        { entry.tags && <TagList tags={entry.tags} /> }
        <p className='text-gray-600 pb-3'>{entry.description}</p>
        <div className="flex items-start">
          <MediaViewer documentId={entry.document.id} />
        </div>
        <div>
          <p className='text-gray-600 mt-4 mb-2'><span className='font-medium'>Created By: </span>{entry.createdBy.firstName} {entry.createdBy.lastName}</p>
          <p className='text-gray-600 mb-2'><span className='font-medium'>Created At: </span>{formatDateTime(entry.createdAtUtc)}</p>
          <p className='text-gray-600 mb-2'><span className='font-medium'>Entry ID: </span>{entry.id}</p>
        </div>
      </div>
      <div></div>
      <div className="flex flex-col col-span-2">
        <h1 className="text-2xl font-bold text-gray-800">Comments</h1>
        { entry.comments && 
          <ul>
            { entry.comments
              .sort((a,b) => new Date(b.createdAtUtc).getTime() - new Date(a.createdAtUtc).getTime())
              .map(comment => 
            <li key={comment.id}>
              <CommentBlock comment={comment}/>
            </li>) }
          </ul>
        }
        <div className="mt-auto flex items-end gap-2">
          <TextArea 
            value={commentValue ?? ''}
            onChange={setCommentValue}
            onKeyDown={textAreaKeyDown} />
          <Button onClick={() => addComment()}>Send</Button>
        </div>
      </div>
    </div>
  );

};

export default EntryRoute;