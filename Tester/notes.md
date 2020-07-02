# onenote2md
An automated tool to convert OneNote files to Markdown files. :+1:

# Features
- Convert Notebooks/Sections/Pages to folders and md files.
- Retains rich text formatting.
- Handles lists.
- Converts OneNote tables to md tables.
- Maps images to files and provide links to md files.


# OneNote object model
- OneNote uses a simple object model of nested hierarchical objects.
The base object is the `Notebook` which contain `Section` object which in turn contain `Page` objects.
- Page contain the actual note information including text, image, lists, table and other formatted content.
- Page itself is further decomposed into a hierachical structure containing page object `OEChildren` elements.
- Each Page specifies a `Title` and a `PageLevel`.
- Each OEChildren can further contain:
  - Text
  - List
  - Table
  - Image
  - Further OEChildren elements.
- Formatted text is maintained as CDATA HTML content.
- A table is specified with the following format:
```
Table:
<one:OE>
    <one:Table>
    <one:Columns>
        <one:Column index=0 width=>
        <one:Column index=n width=>
    </one:Columns>
    <one:Row>
        <one:Cell>
            <one:T>
        <one:Cell>
    </one:Row>
```

  
## OneNote object model synopsis:
```
Notebook
	Section
		Page
            OEChildren
```


# Mapping
- We map OneNote content to markdown using the following conventions:
- Rich text formatting (bold, italic, underline) is retained.

## Headers
- OneNote headers (Heading 1 to 6) are mapped to md headers (# to ######).

## Quotes and Citations
- Quotes and Citations a converted to *italics*.

## Code
- Code is converted to `single line` or
 ``` 
 multiline 
 ```

## Rich text
- Italics is converted to *italics*.
- Bold is converted to **bold**.
- Underline is converted to ``TBD``.
- Strikethrough is converted to ~~strikethrough~~.


## Lists
- Unordered lists use * for list beginning and white space nesting for nesting such as:
  - Sub list  
    - Sub list  
      - Sub list
        - Sub list 

- Ordered lists use 1 for list beginning and white space nesting for nesting such as:
  1. Sub list  
     1. Sub list  
     2. Sub list
  2. Sub list
     1. Sub list 
     2. Sub list

## Links
- [I'm an inline-style link](https://www.google.com)


## Images
- Images are converted to png/jpg and linked to md file as
- Inline-style: 
- ![alt text](https://github.com/adam-p/markdown-here/raw/master/src/common/images/icon48.png "Logo Title Text 1")

## Tables
- Tables are supported. `TBD`


## HTML
- Html can be embedded within md.


## Horizontal rule
---

## Videos
<a href="http://www.youtube.com/watch?feature=player_embedded&v=YOUTUBE_VIDEO_ID_HERE
" target="_blank"><img src="http://img.youtube.com/vi/YOUTUBE_VIDEO_ID_HERE/0.jpg" 
alt="IMAGE ALT TEXT HERE" width="240" height="180" border="10" /></a>

## Task lists
- Converted to md as: 
  - [ ] Task 1
  - [x] Task 2 completed

## Tag
- Converted using emojis such as :+1:

# Resources
## OneNote
- https://docs.microsoft.com/en-us/office/client-developer/onenote/onenote-home
- https://docs.microsoft.com/en-us/office/client-developer/onenote/onenote-developer-reference

## Markdown
- https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet
- https://guides.github.com/pdfs/markdown-cheatsheet-online.pdf
- https://www.markdownguide.org/cheat-sheet/
