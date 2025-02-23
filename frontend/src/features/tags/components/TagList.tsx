import React from 'react';
import { Tag } from '@/types/app';
import ClickableTag from '@/features/tags/components/ClickableTag';

export interface TagListProps {
  tags:Tag[];
  tagListClassName?:string;
  tagClassName?:string;
  onClick?: (tag:Tag) => void;
}

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
