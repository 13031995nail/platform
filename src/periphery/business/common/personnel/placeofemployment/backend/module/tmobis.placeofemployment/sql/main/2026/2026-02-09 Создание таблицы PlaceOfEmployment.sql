--liquibase formatted sql
--changeset nvaliullin:CreatePlaceOfEmployment
	
CREATE TABLE IF NOT EXISTS PlaceOfEmployment
(
    primarykey uuid NOT NULL,
    CreateTime TIMESTAMP(3) NULL,
	Creator VARCHAR(255) NULL,
	Editor VARCHAR(255) NULL,
	EditTime TIMESTAMP(3) NULL,
	Organization UUID NOT NULL,
	Position UUID NOT NULL,
	Employee UUID NOT NULL,
	Actuality boolean NOT NULL,
	Phone varchar(50) NULL,
    PRIMARY KEY (primaryKey));
	
ALTER TABLE PlaceOfEmployment DROP CONSTRAINT IF EXISTS PlaceOfEmployment_FOrganization_0;

ALTER TABLE PlaceOfEmployment DROP CONSTRAINT IF EXISTS PlaceOfEmployment_FPosition_0;

ALTER TABLE PlaceOfEmployment DROP CONSTRAINT IF EXISTS PlaceOfEmployment_FEmployee_0;

drop index IF EXISTS PlaceOfEmployment_IOrganization;

drop index IF EXISTS PlaceOfEmployment_IPosition;

drop index IF EXISTS PlaceOfEmployment_IEmployee;

ALTER TABLE PlaceOfEmployment ADD CONSTRAINT PlaceOfEmployment_FOrganization_0 FOREIGN KEY (Organization) REFERENCES Organization; 
CREATE INDEX PlaceOfEmployment_IOrganization on PlaceOfEmployment(Organization);

ALTER TABLE PlaceOfEmployment ADD CONSTRAINT PlaceOfEmployment_FPosition_0 FOREIGN KEY (Position) REFERENCES Position; 
CREATE INDEX PlaceOfEmployment_IPosition on PlaceOfEmployment(Position);

ALTER TABLE PlaceOfEmployment ADD CONSTRAINT PlaceOfEmployment_FEmployee_0 FOREIGN KEY (Employee) REFERENCES Employee; 
CREATE INDEX PlaceOfEmployment_IEmployee on PlaceOfEmployment (Employee);

--rollback ALTER TABLE PlaceOfEmployment DROP CONSTRAINT IF EXISTS PlaceOfEmployment_FOrganization_0; ALTER TABLE PlaceOfEmployment DROP CONSTRAINT IF EXISTS PlaceOfEmployment_FPosition_0; ALTER TABLE PlaceOfEmployment DROP CONSTRAINT IF EXISTS PlaceOfEmployment_FEmployee_0;DROP TABLE IF EXISTS PlaceOfEmployment;


