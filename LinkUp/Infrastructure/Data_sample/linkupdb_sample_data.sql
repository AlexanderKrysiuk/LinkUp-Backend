--
-- PostgreSQL database dump
--

-- Dumped from database version 15.1
-- Dumped by pg_dump version 15.1

-- Started on 2023-08-06 16:27:53

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

DROP DATABASE linkupdb;
--
-- TOC entry 3336 (class 1262 OID 33748)
-- Name: linkupdb; Type: DATABASE; Schema: -; Owner: linkupadmin
--

CREATE DATABASE linkupdb WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'Polish_Poland.1250';


ALTER DATABASE linkupdb OWNER TO linkupadmin;

\connect linkupdb

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 216 (class 1259 OID 33811)
-- Name: Clients; Type: TABLE; Schema: public; Owner: linkupadmin
--

CREATE TABLE public."Clients" (
    "Id" uuid NOT NULL,
    "Name" character varying(30) NOT NULL,
    "Email" character varying(30) NOT NULL,
    "Password" character varying(30) NOT NULL
);


ALTER TABLE public."Clients" OWNER TO linkupadmin;

--
-- TOC entry 215 (class 1259 OID 33806)
-- Name: Contractors; Type: TABLE; Schema: public; Owner: linkupadmin
--

CREATE TABLE public."Contractors" (
    "Id" uuid NOT NULL,
    "Name" character varying(30) NOT NULL,
    "Email" character varying(30) NOT NULL,
    "Password" character varying(30) NOT NULL
);


ALTER TABLE public."Contractors" OWNER TO linkupadmin;

--
-- TOC entry 214 (class 1259 OID 33801)
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: linkupadmin
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO linkupadmin;

--
-- TOC entry 3330 (class 0 OID 33811)
-- Dependencies: 216
-- Data for Name: Clients; Type: TABLE DATA; Schema: public; Owner: linkupadmin
--

INSERT INTO public."Clients" VALUES ('05487c11-1753-4887-b06d-cc329f607614', 'Saruman White', 'swhite@email.com', 'Let''sK!ll@llh0bbits');
INSERT INTO public."Clients" VALUES ('e421ffc2-05c5-44b6-88b6-cad710af7752', 'Fred Colon', 'fcolon@disc.com', 'Cl4tchSuck555!');
INSERT INTO public."Clients" VALUES ('f421ffc2-05c5-44b6-88b6-cad710af7752', 'Thor Odinsson', 'todinsson@yggdrasil.no', 'I4mth3m!ght!3st');
INSERT INTO public."Clients" VALUES ('037549a1-929e-46d9-b50a-1e2d1264a0b3', 'Meriadock Brandybuck', 'mbrandybuck@buckland.sh', 'H0wM@nyD!dY0u3@t?');
INSERT INTO public."Clients" VALUES ('068cb6a8-ca6c-470d-ab03-f2e4ce5558c5', 'Peregrin Took', 'ptook@tookland.sh', '!tC0m3s!nP!nt3s?');
INSERT INTO public."Clients" VALUES ('73a0df0c-857e-4c71-a5c1-2f98e5a7b0f0', 'Samwise Gamgee', 'sgamgee@hobbiton.sh', 'P0-t@y-t03s!');
INSERT INTO public."Clients" VALUES ('71e1f15a-28eb-4569-8856-9717d0532ff4', 'Bilbo Baggins', 'bbaggins@hobbiton.sh', 'B@rr3lR!d3r');
INSERT INTO public."Clients" VALUES ('418d7d67-0510-40b2-afaa-ec3bcebbcec0', 'Dudley Dursley', 'ddursley@muggle.uk', 'B!gD@k@D!nkyD!ddydums');
INSERT INTO public."Clients" VALUES ('2233c810-b737-4655-865f-f6366cce15a2', 'Argus Filch', 'afilch@hogwarts.hp', 'P33v3s0ut!');
INSERT INTO public."Clients" VALUES ('d40d967b-7b8f-4d46-a883-9185d66fd9e9', 'Mrs Norris', 'mnorris@mekitty.hp', '4Mr@@@uuuu!');


--
-- TOC entry 3329 (class 0 OID 33806)
-- Dependencies: 215
-- Data for Name: Contractors; Type: TABLE DATA; Schema: public; Owner: linkupadmin
--

INSERT INTO public."Contractors" VALUES ('b65747e3-6aa5-4bad-b004-d763ffc5ce36', 'Samuel Vimes', 'svimes@disc.uk', 'S@muelV!me5');
INSERT INTO public."Contractors" VALUES ('bc1c550d-b5ae-454c-9078-4ef02a4e85e1', 'Frodo Baggins', 'fbaggins@hobbiton.sh', 'My!My!0wn');
INSERT INTO public."Contractors" VALUES ('bc1c550d-b5ae-454c-9078-4ef02a4e85e3', 'Han Solo', 'hsolo@millenium.fo', 'Il0v3Princ355L3!@');
INSERT INTO public."Contractors" VALUES ('19ae5da3-f301-4901-8a77-bc079a24b4b1', 'Esmeralda Weatherwax', 'eweatherwax@witch.la', 'He4d0logy!sth3k3y');
INSERT INTO public."Contractors" VALUES ('52f2b261-7ea4-41f2-9bec-c3a73497e1df', 'Gytha Ogg', 'gogg@witch.la', 'H3dg3h0gS0ng!');
INSERT INTO public."Contractors" VALUES ('41c3586f-d334-4b60-8ceb-28c6acc2bcc6', 'Magrat Garlick', 'mgarlick@witch.la', 'IL0v3Verenc3@');
INSERT INTO public."Contractors" VALUES ('44821624-5660-4133-95a5-ad71cbb5dea1', 'Agnes Nitt', 'anitt@witch.la', 'P3rd!t@!s!ns!deMe');
INSERT INTO public."Contractors" VALUES ('7d628c8c-7bdd-45d4-94d2-08da442ca993', 'Ben Kenobi', 'bkenobi@whosjedi.sw', 'H3ll0Th3r3!');
INSERT INTO public."Contractors" VALUES ('de6ce00f-27be-4877-9b65-27f7b82d177c', 'Luke Skywalker', 'lskywalker@mejedi.sw', '!H@v3@V3ryB@dF33l!ng@boutTh!s');
INSERT INTO public."Contractors" VALUES ('d88ec7c6-fe3b-4024-9a70-cb82cb953feb', 'Anakin Skywalker', 'askywalker@mesith.sw', '!@mY0urF@th3r');
INSERT INTO public."Contractors" VALUES ('329d66c0-60b2-4899-88fd-5ffe2b6fe3ff', 'Din Djarin', 'ddjarin@mandalore.sw', 'Th!s!sTh3W@y');
INSERT INTO public."Contractors" VALUES ('aececce9-57a8-4d98-9ba7-538ebf5a88fc', 'Lord Voldemort', 'lvoldemort@greatestwizard.hp', '7H0rcruXs3s''M@st3r');


--
-- TOC entry 3328 (class 0 OID 33801)
-- Dependencies: 214
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: linkupadmin
--

INSERT INTO public."__EFMigrationsHistory" VALUES ('20230805124859_Contractors_table', '7.0.9');
INSERT INTO public."__EFMigrationsHistory" VALUES ('20230805125015_Clients_table', '7.0.9');


--
-- TOC entry 3185 (class 2606 OID 33815)
-- Name: Clients PK_Clients; Type: CONSTRAINT; Schema: public; Owner: linkupadmin
--

ALTER TABLE ONLY public."Clients"
    ADD CONSTRAINT "PK_Clients" PRIMARY KEY ("Id");


--
-- TOC entry 3183 (class 2606 OID 33810)
-- Name: Contractors PK_Contractors; Type: CONSTRAINT; Schema: public; Owner: linkupadmin
--

ALTER TABLE ONLY public."Contractors"
    ADD CONSTRAINT "PK_Contractors" PRIMARY KEY ("Id");


--
-- TOC entry 3181 (class 2606 OID 33805)
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: linkupadmin
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


-- Completed on 2023-08-06 16:27:54

--
-- PostgreSQL database dump complete
--

