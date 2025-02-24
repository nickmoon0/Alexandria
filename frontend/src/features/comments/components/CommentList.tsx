import React from 'react';
import CommentBlock from '@/features/comments/components/CommentBlock';
import { Comment } from '@/types/app';

export interface CommentListProps {
  comments: Comment[];
  className?: string;
}

const CommentList = ({ comments, className }: CommentListProps) => {
  const sortedComments = [...comments].sort((a, b) => 
    new Date(b.createdAtUtc).getTime() - new Date(a.createdAtUtc).getTime()
  );

  return (
    <div className={`overflow-y-auto p-2 space-y-2 bg-gray-100 rounded-lg shadow-md scrollbar-thin scrollbar-thumb-gray-400 scrollbar-track-gray-200 ${className}`}>
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
