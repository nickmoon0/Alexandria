import { Tag } from '@/types/app';

export interface ClickableTagProps {
  tag:Tag;
  className?:string;
  onClick?: (e:React.MouseEvent<HTMLDivElement>) => void;
};



const ClickableTag = ({ tag, className, onClick }:ClickableTagProps) => {
  return (
    <span
      onClick={onClick}
      className={`px-3 py-1 cursor-pointer text-sm font-medium text-white bg-blue-600 rounded-full shadow-md hover:bg-blue-700 transition ${className}`}
    >
      {tag.name}
    </span>
  );
};

export default ClickableTag;