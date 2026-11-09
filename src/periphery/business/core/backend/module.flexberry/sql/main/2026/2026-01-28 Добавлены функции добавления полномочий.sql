--liquibase formatted sql
--changeset MenshchikovaKA:createFuncForCreatePermissions endDelimiter:$$ splitStatements:false

CREATE OR REPLACE FUNCTION public.GetPermissionClass(classname character varying)
 RETURNS uuid
 LANGUAGE plpgsql
AS $function$
        declare 
        sPK uuid;
		begin
		select primaryKey into sPK from "storms"
                where  name = className; 
        return sPK;
		end;
        $function$
;

CREATE OR REPLACE FUNCTION public.CreatePermissionClass(classname character varying)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
        declare 
        sPK uuid;
		begin
		INSERT INTO STORMS(primarykey, name, type, isattribute, isoperation, isview, isclass, sharedoper, createtime, creator, edittime, editor, comment)
		values (uuid_generate_v4(), classname, null, false, false, false, true, true, NOW(), null, null, null, null);
        end;
        $function$
;

CREATE OR REPLACE FUNCTION public.GetPermissionfor(agentname character varying, classname character varying)
 RETURNS uuid
 LANGUAGE plpgsql
AS $function$
        declare 
        pPK uuid;
		begin
		select primaryKey into pPK from "stormp"
                where  Subject_m0 = (SELECT primaryKey from "storms" where Name = className) 
                and Agent_m0 = (SELECT primaryKey from "stormag" where Name = agentName and isrole = true); 
        return pPK;
		end;
        $function$
;

CREATE OR REPLACE FUNCTION public.CreatePermissionFor(agent character varying, class character varying)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
        declare 
        sPK uuid; agPK uuid;
		begin
		select primaryKey into sPK from "storms" where  Name = class; 
        select primaryKey into agPK from "stormag" where  Name = agent and isrole = true; 
        insert into "stormp"(primaryKey,Subject_m0, Agent_m0, CreateTime)
            VALUES (uuid_generate_v4(), sPK, AgPK, now());
		end;
        $function$
;

CREATE OR REPLACE FUNCTION public.CreatePermissionAccess(access character varying, agent character varying, class character varying)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
        declare 
        pPK uuid;
		begin
		select GetPermissionFor(agent, class) into pPK;
        insert into "stormac"(primaryKey,TypeAccess, Permition_m0, CreateTime)
            VALUES (uuid_generate_v4(), access, pPK, now());
		end;
        $function$
;


CREATE OR REPLACE FUNCTION public.DeletePermissionFor(agent character varying, class character varying)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
        declare 
        pPK uuid;
		begin
		select GetPermissionFor(agent, class) into pPK; 
		DELETE FROM "stormac"
			WHERE Permition_m0 = pPK;
		DELETE FROM "stormp"
			WHERE Permition_m0 = pPK;
		end;
        $function$
;

CREATE OR REPLACE FUNCTION public.ChangeOrCreatePermissionFor(access character varying, agent character varying, class character varying)
 RETURNS void
 LANGUAGE plpgsql
AS $function$ 
        declare  
       	sPK uuid;
        pPK uuid;
		begin
		select GetPermissionClass(class) into sPK;
		if (sPK is NULL) then 
				execute CreatePermissionClass(class);
				select GetPermissionClass(class) into sPK;	
		end if;
		select GetPermissionFor(agent, class) into pPK; 
		if (pPK is NULL) then 
				execute CreatePermissionFor(agent, class);
				execute CreatePermissionAccess(access, agent, class);	
		end if;
		select GetPermissionFor(agent, class) into pPK;
		if(access = '')	then 
			execute DeletePermissionFor(agent, class);
		else 
			update "stormac" set TypeAccess= access where Permition_m0= pPK;
		end if;
		end;
        $function$
;
--rollback DROP FUNCTION public.changeorcreatepermissionfor(varchar, varchar, varchar); DROP FUNCTION public.deletepermissionfor(varchar, varchar); DROP FUNCTION public.createpermissionaccess(varchar, varchar, varchar); DROP FUNCTION public.createpermissionfor(varchar, varchar); DROP FUNCTION public.getpermissionfor(varchar, varchar); DROP FUNCTION public.CreatePermissionClass(varchar, varchar, varchar); DROP FUNCTION public.GetPermissionClass(varchar, varchar, varchar);
--comment createFuncForCreatePermissions
