--liquibase formatted sql
--changeset BorovinVA:FileDeleteProtection

ALTER TABLE "file" ADD IsDeleteProtected BOOLEAN NOT NULL DEFAULT false;