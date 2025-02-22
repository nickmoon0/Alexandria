import { Tag } from '@/types/app';

export interface TagListProps {
  tags:Tag[];
  tagListClassName?:string;
  tagClassName?:string;
  onClick?: (tag:Tag) => void;
}

export interface ClickableTagProps {
  tag:Tag;
  className?:string;
  onClick?: (e:React.MouseEvent<HTMLDivElement>) => void;
};

export const ClickableTag = ({ tag, className, onClick }:ClickableTagProps) => {
  return (
    <span
      onClick={onClick}
      className={`px-3 py-1 cursor-pointer text-sm font-medium text-white bg-blue-600 rounded-full shadow-md hover:bg-blue-700 transition ${className}`}
    >
      {tag.name}
    </span>
  );
};

const TagList = ({ tags, tagListClassName, tagClassName, onClick }:TagListProps) => {
  if (!tags || tags.length === 0) return null;

  return (
    <div className={`flex flex-wrap gap-2 pb-2 ${tagListClassName}`}>
      {tags.map((tag) => (
        <ClickableTag 
          key={tag.id}
          tag={tag}
          className={tagClassName}
          onClick={(e:React.MouseEvent<HTMLDivElement>) => {
            e.stopPropagation();
            if (onClick) {
              onClick(tag);
            }
          }} />
      ))}
    </div>
  );
};

export default TagList;
