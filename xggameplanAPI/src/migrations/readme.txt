These folders contain files that will do the following;

Import data from bulk load MySQL files to give a dataset needed for testing purposes.

To run--

To create new SQL dump files(sql insertv files from a source database)
To call it you need in to in your MYSQL bin folder or have your path set up for MySQLDump.exe

sample call:

createfiles.bat yourserver youruserid yourpassword

To just dump the data as it is without creating new files.
To call it you need in to in your MYSQL bin folder or have your path set up for MySQL.exe
You must have a database on the target server called xggptenant

if you have issues with table does not exist errors:

Do you have the database created with tables?

You kay have to run the script "table to lower case.sql" then run the batch file.
If you need to do this, after the data moves you will need to run tables to uppercase.sql to set the table names back.
 

sample call:

dumpfiles.bat yourserver youruserid yourpassword
