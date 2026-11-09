--liquibase formatted sql
--changeset kmenshchikova:CreateSignType

CREATE TABLE IF NOT EXISTS SignType
(
    primarykey uuid NOT NULL,
    name VARCHAR(200) NOT NULL,
	code VARCHAR(10) NULL,
	actuality boolean NOT NULL,
    createTime TIMESTAMP(3) NULL,
	creator VARCHAR(255) NULL,
	editor VARCHAR(255) NULL,
	editTime TIMESTAMP(3) NULL,
    PRIMARY KEY (primaryKey));

select ChangeOrCreatePermissionFor('Full', 'admin', 'Tmobis.Business.Process.Documents.Flexberry.SignType');
select ChangeOrCreatePermissionFor('Full', 'Пользователь', 'Tmobis.Business.Process.Documents.Flexberry.SignType');
	
--rollback DROP TABLE IF EXISTS SignType;
