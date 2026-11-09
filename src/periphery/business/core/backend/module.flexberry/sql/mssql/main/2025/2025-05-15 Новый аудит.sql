--liquibase formatted sql
--changeset BorovinVA:NewAudit

CREATE TABLE Audit (
    primaryKey UNIQUEIDENTIFIER NOT NULL,
    UserName VARCHAR(1024) NULL,
    UserLogin VARCHAR(1024) NULL,
    OperationId UNIQUEIDENTIFIER NULL,
    OperationTags VARCHAR(255) NULL,
    ObjectType VARCHAR(1024) NULL,
    ObjectPrimaryKey VARCHAR(38) NULL,
    OperationTime DATETIME NOT NULL,
    OperationType VARCHAR(255) NOT NULL,
    ExecutionStatus VARCHAR(10) NOT NULL,
    Source VARCHAR(255) NULL,
    SerializedFields TEXT NULL,
    HeadAuditEntity UNIQUEIDENTIFIER NULL,
    PRIMARY KEY (primaryKey));
    
ALTER TABLE Audit ADD CONSTRAINT FK2b809dfb762a4e9db0096d2b1fe889ae FOREIGN KEY (HeadAuditEntity) REFERENCES Audit; 
CREATE INDEX Indexa91db9c259624f959230d031bc90415c on Audit (HeadAuditEntity);
CREATE INDEX Indexa91db9c259624f959230d031bc90416c on Audit (OperationTime);