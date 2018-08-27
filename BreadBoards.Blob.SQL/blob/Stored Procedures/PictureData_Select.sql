/*
================================================================================
Author: John Williams
Create Date: 08/17/2018
Description: Selects from blob.PictureData parameters optional default to all rows
--------------------------------------------------------------------------------

=== Change Log =================================================================
Date          Author    Description
-----------   -------   --------------------------------------------------------
08/17/2018     JHW      • Created This Stored Procedure
================================================================================ */
Create Proc blob.PictureData_Select
  @RowID Int = Null
 ,@RowGuID UniqueIdentifier = Null
 ,@FileName nVarChar(100) = Null
As
--------------------------------------------------------------------------------
Select RowID = pd.RowID
      ,RowGuID = pd.RowGuID
      ,RowTimeStamp = pd.RowTimeStamp
      ,RowCreator = pd.RowCreator
      ,RowCreatorHost = pd.RowCreatorHost
      ,RowCreatorHostName = pd.RowCreatorHostName
      ,FileSource = pd.FileSource
      ,FileName = pd.FileName
      ,FileCreated = pd.FileCreated
      ,FileModified = pd.FileModified
      ,FileWidthPx = pd.FileWidthPx
      ,FileHeightPx = pd.FileHeightPx
      ,FilePageCount = pd.FilePageCount
      ,FilePageIndex = pd.FilePageIndex
      ,FilePageNumber = pd.FilePageNumber
      ,FileBlob = pd.FileBlob
      ,FileBlobType = pd.FileBlobType
       -------------------------------------------------------------------------
      ,PathName = pd.FileBlob.PathName()
      ,FileContext =  Get_FileStream_Transaction_Context()
--------------------------------------------------------------------------------
  From blob.PictureData pd
--------------------------------------------------------------------------------
 Where pd.RowID = IsNull(@RowID, pd.RowID)
   And pd.RowGuid = IsNull(@RowGuid, pd.RowGuID)
   And pd.FileName Like IsNull(@FileName, '%')
--------------------------------------------------------------------------------