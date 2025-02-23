import React from 'react';
import CommentBlock from '@/features/comments/components/CommentBlock';
import { Comment } from '@/types/app';

export interface CommentListProps {
  comments: Comment[];
}

const CommentList = ({ comments }: CommentListProps) => {
  const sortedComments = [...comments].sort((a, b) => 
    new Date(b.createdAtUtc).getTime() - new Date(a.createdAtUtc).getTime()
  );

  return (
    <div className='flex-grow max-h-80 overflow-y-auto p-2 space-y-2 bg-gray-100 rounded-lg shadow-md scrollbar-thin scrollbar-thumb-gray-400 scrollbar-track-gray-200'>
      {sortedComments.length > 0 ? (
        sortedComments.map((comment) => (
          <CommentBlock key={comment.id} comment={comment} />
        ))
      ) : (
        <p className='text-gray-500 text-center'>No comments yet</p>
      )}
    </div>
  );
};

export default CommentList;
