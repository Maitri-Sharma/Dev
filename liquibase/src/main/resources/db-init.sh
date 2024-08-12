#!/bin/sh

psql -d postgres -U postgres -c  'drop database if exists "pumardf";'
psql -d postgres -U postgres -c  'drop user if exists "puma_user";'


psql -d postgres -U postgres -c  "create role puma_user with login password 'Pass1234';"
psql -d postgres -U postgres -c  'create database "pumardf";'

psql -d pumardf -U postgres  -c  '\connect pumardf puma_user'
psql -d pumardf -U postgres  -c  'create schema kspu_app authorization puma_user;'
psql -d pumardf -U postgres  -c  'create schema kspu_db authorization puma_user;'
psql -d pumardf -U postgres  -c  'create schema kspu_gdb authorization puma_user;'

psql -d pumardf -U postgres  -c  'GRANT ALL PRIVILEGES ON SCHEMA kspu_app,kspu_db,kspu_gdb TO puma_user;'
