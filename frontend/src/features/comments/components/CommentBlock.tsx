import { formatDateTime } from "@/lib/helpers";
import { Comment } from "@/types/app";

export interface CommentBlockProps {
  comment:Comment;
};

const CommentBlock = ({comment}:CommentBlockProps) => {
  return (
    <div className="bg-blue-600 text-white rounded-lg shadow-md hover:bg-blue-700 transition p-2 my-2">
      <p className="text-sm">{comment.createdBy.firstName} {comment.createdBy.lastName} ({formatDateTime(comment.createdAtUtc)})</p>
      <p className="text-base">{comment.content}</p>
    </div>
  );
};

export default CommentBlock;