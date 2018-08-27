--------------------------------------------------------------------------------
Create Table blob.PictureData (
  ------------------------------------------------------------------------------
  RowID Int Identity Not Null Constraint pk_blobPictureData_RowID Primary Key
 ,RowGuID UniqueIdentifier Not Null Constraint uq_blobPictureData_RowGuID Unique RowGuidCol Constraint df_blobPictureData_GuID Default(NewID())
 ,RowTimeStamp DateTime Constraint df_blobPictureData_RowTimeStamp Default(GetDate())
 ,RowCreator SysName          Not Null
 ,RowCreatorHost nVarChar(15) Not Null --Constraint ck_blobPictureData_RowCreatorHost Check(RowCreatorHost Like '[1-255].[0-255].[0-255].[0-255]')
 ,RowCreatorHostName SysName  Not Null
  ------------------------------------------------------------------------------
 ,FileSource SysName Not Null
 ,FileName SysName Not Null
 ,FileCreated DateTime
 ,FileModified DateTime
 ,FileWidthPx Int
 ,FileHeightPx Int
 ,FilePageCount Int Not Null Constraint df_blobPictureData_FilePageCount Default(1)
 ,FilePageIndex Int Not Null --Constraint df_blobPictureData_FilePageIndex Default(1)
 ,FilePageNumber As FilePageIndex + 1
  ------------------------------------------------------------------------------
 ,FileBlob VarBinary(Max) FileStream
 ,FileBlobType nVarChar(4) Not Null Constraint df_blobPictureData_BlobType Default('PNG')
  ------------------------------------------------------------------------------
  --- Make Unique By FileSource + FileModified ---------------------------------
 ,Constraint uq_blobPictureData_FileSource Unique(FileSource, FileModified, FilePageIndex)
  ------------------------------------------------------------------------------
)
--------------------------------------------------------------------------------