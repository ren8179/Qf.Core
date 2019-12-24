
/****** Object:  StoredProcedure [dbo].[ProcGetPageData]    Script Date: 2019-12-24 09:58:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ProcGetPageData]
(  @TableName VARCHAR(3000), --表名,多表是请使用 tA a inner join tB b On a.AID = b.AID
   @PrimaryKey NVARCHAR(100),    --主键，可以带表头 a.AID
   @Fields NVARCHAR(2000) = '*',--读取字段
   @Condition NVARCHAR(3000) = '',--Where条件
   @CurrentPage INT = 1,    --开始页码
   @PageSize INT = 10,        --页大小
   @Sort NVARCHAR(200) = '', --排序字段
   @RecordCount INT = 0 OUT
)
AS
DECLARE @strWhere VARCHAR(2000)
DECLARE @strsql NVARCHAR(3900)
IF @Condition IS NOT NULL AND len(ltrim(rtrim(@Condition)))>0
  BEGIN
   SET @strWhere = ' WHERE ' + @Condition + ' '
  END
ELSE
  BEGIN
   SET @strWhere = ''
  END
        
IF (charindex(ltrim(rtrim(@PrimaryKey)),@Sort)=0)
BEGIN
    IF(@Sort='')
        SET @Sort = @PrimaryKey + ' DESC '
    ELSE
        SET @Sort = @Sort
END
SET @strsql = 'SELECT @RecordCount = Count(1) FROM ' + @TableName + @strWhere  
EXECUTE sp_executesql @strsql ,N'@RecordCount INT output',@RecordCount OUTPUT
IF @CurrentPage = 1 --第一页提高性能
BEGIN 
  SET @strsql = 'SELECT TOP ' + str(@PageSize) +' '+@Fields
              + '  FROM ' + @TableName + ' ' + @strWhere + ' ORDER BY  '+ @Sort
END 
ELSE
  BEGIN
    /* Execute dynamic query */    
    DECLARE @START_ID NVARCHAR(50)
    DECLARE @END_ID NVARCHAR(50)
    SET @START_ID = CONVERT(NVARCHAR(50),(@CurrentPage - 1) * @PageSize + 1)
    SET @END_ID = CONVERT(NVARCHAR(50),@CurrentPage * @PageSize)
    SET @strsql =  ' SELECT *
   FROM (SELECT ROW_NUMBER() OVER(ORDER BY '+@Sort+') AS rownum, 
     '+@Fields+ '
      FROM '+@TableName + @strWhere +') AS XX
   WHERE rownum BETWEEN '+@START_ID+' AND ' +@END_ID +' ORDER BY XX.rownum ASC'
  END
EXEC(@strsql)
RETURN
GO


