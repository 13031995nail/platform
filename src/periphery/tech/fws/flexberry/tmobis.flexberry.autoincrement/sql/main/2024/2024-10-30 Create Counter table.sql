
--liquibase formatted sql
--changeset PolezhaevIV:Counter
 CREATE TABLE IF NOT EXISTS counter (
   primarykey UUID NOT NULL,
   key VARCHAR NOT NULL,
   value INT NOT NULL,
   increment INT NOT NULL,
   name VARCHAR NULL,
   createtime TIMESTAMP(3) NULL,
   creator VARCHAR(255) NULL,
   edittime TIMESTAMP(3) NULL,
   editor VARCHAR(255) NULL,
   PRIMARY KEY (primarykey)
);
 --rollback drop table Counter
 --comment Increment Table Counter

