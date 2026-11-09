--liquibase formatted sql
--changeset vborovin:2024-10-11-admin-pass
update stormag set pwd = '6AA8A04A97EDDF1FBFC9D4B6355C324F596D2ADBCF826BDEC8DA88311C662991' where primaryKey = '35baa42a-a10d-464c-8469-b2ead2f6890c';