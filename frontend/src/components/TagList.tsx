import { Tag } from '@/types/app';

export interface TagListProps<T extends (...args: unknown[]) => void> {
  tags:Tag[];
  tagListClassName?:string;
  tagClassName?:string;
  onClick?: T;
}

export interface ClickableTagProps<T extends (...args: unknown[]) => void> {
  tag:Tag;
  className?:string;
  onClick?: T;
};

export const ClickableTag = <T extends (...args: unknown[]) => void>({ tag, className, onClick }:ClickableTagProps<T>) => {
  return (
    <span
      onClick={onClick}
      className={`px-3 py-1 cursor-pointer text-sm font-medium text-white bg-blue-600 rounded-full shadow-md hover:bg-blue-700 transition ${className}`}
    >
      {tag.name}
    </span>
  );
};

const TagList = <T extends (...args: unknown[]) => void>({ tags, tagListClassName, tagClassName, onClick }:TagListProps<T>) => {
  if (!tags || tags.length === 0) return null;

  return (
    <div className={`flex flex-wrap gap-2 pb-2 ${tagListClassName}`}>
      {tags.map((tag) => (
        <ClickableTag key={tag.id} tag={tag} className={tagClassName} onClick={(e) => {
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
