/*
================================================================================
Author: John Williams
Create Date: 08/17/2018
Description: Inserts a row into blob.PictureData with anti-dupe logic
--------------------------------------------------------------------------------

=== Change Log =================================================================
Date          Author    Description
-----------   -------   --------------------------------------------------------
08/17/2018     JHW      • Created This Stored Procedure
================================================================================ */
Create Proc blob.PictureData_Insert
  @RowCreator SysName
 ,@RowCreatorHost nVarChar(15)
 ,@RowCreatorHostName SysName
  ------------------------------------------------------------------------------
 ,@FileSource SysName
 ,@FileName SysName
 ,@FileCreated DateTime
 ,@FileModified DateTime
 ,@FileWidthPx Int
 ,@FileHeightPx Int
 ,@FilePageCount Int
 ,@FilePageIndex Int
  ------------------------------------------------------------------------------
 ,@FileBlob VarBinary(Max)
 ,@FileBlobType nVarChar(4)
  ------------------------------------------------------------------------------
As
--------------------------------------------------------------------------------
Insert blob.PictureData(
       RowCreator
      ,RowCreatorHost
      ,RowCreatorHostName
      ,FileSource
      ,FileName
      ,FileCreated
      ,FileModified
      ,FileWidthPx
      ,FileHeightPx
      ,FilePageCount
      ,FilePageIndex
      ,FileBlob
      ,FileBlobType
)
--------------------------------------------------------------------------------
Select x.*
  From ( Select RowCreator = @RowCreator
               ,RowCreatorHost = @RowCreatorHost
               ,RowCreatorHostName = @RowCreatorHostName
                ----------------------------------------------------------------
               ,FileSource = @FileSource
               ,FileName = @FileName
               ,FileCreated = @FileCreated
               ,FileModified = @FileModified
               ,FileWidthPx = @FileWidthPx
               ,FileHeightPx = @FileHeightPx
               ,FilePageCount = @FilePageCount
               ,FilePageIndex = @FilePageIndex
                ----------------------------------------------------------------
               ,FileBlob = @FileBlob
               ,FileBlobType = @FileBlobType
                ----------------------------------------------------------------
  ) x
--------------------------------------------------------------------------------
--- Left Join on duplicates then exclude them via the Where clause -------------
  Left Join blob.PictureData dup
    On dup.FileSource = x.FileSource
   And dup.FileModified = x.FileModified
   And dup.FilePageIndex = x.FilePageIndex
--------------------------------------------------------------------------------
 Where dup.RowID Is Null -- No Dupes
--------------------------------------------------------------------------------