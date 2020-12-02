CREATE TABLE [dbo].[Orders] (
    [Id]               INT           IDENTITY (1, 1) NOT NULL,
    [OrderNo]          NCHAR (8)     NOT NULL,
    [StudyDate]        NCHAR (8)     NOT NULL,
    [ProcessingType]   NCHAR (1)     NOT NULL,
    [InspectionTypeCode]   NVARCHAR (8)  NOT NULL,
    [InspectionTypeName]   NVARCHAR (32) NOT NULL,
    [PatientId]        NCHAR (10)    NOT NULL,
    [PatientNameKanji] NVARCHAR (64) NOT NULL,
    [PatientNameKana]  NVARCHAR (64) NOT NULL,
    [PatientBirth]     NCHAR (8)     NOT NULL,
    [PatientSex]       NCHAR (1)     NOT NULL,
    CONSTRAINT [PK__tmp_ms_x__C3907C744516F376] PRIMARY KEY CLUSTERED ([OrderNo] ASC) 
);

