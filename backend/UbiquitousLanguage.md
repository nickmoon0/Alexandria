# Invariants

---

## User
1. User has a name
2. User creates Entry
3. User owns Entry
4. User creates Characters
5. User creates Collection

## Entry
1. Entry contains Document (File/Image/Video/Etc.)
2. Entry contains Description (optional free text)
3. Entry contains list of related characters
4. Users can leave Comments on Entry

## Document
1. Document has Name
2. Document has File Data
3. Document has a File Path
4. Document has a parent entry

## Comment
1. Users write comments
2. Comments have body text
3. Comment has a parent Entry

## Character
1. Character has name
2. Character has description
3. All Users are also Characters, not all Characters are Users

## Collection
1. Collection maintains a list of Entries
2. Collection has a name