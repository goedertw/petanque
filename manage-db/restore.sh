#!/bin/bash

debug=0

usage () {
    echo 
    echo "Usage: ./restore.sh <sql-file> <mysql-cfg-file>"
    echo
    exit 1
}

cleanup_and_abort () {
    if (( $debug == 1 )); then echo exitcode=$exitcode; fi
    rm -f $tmpcfgfile $tmperrfile
    echo "... Aborting!"; echo; exit 1
}

# check arguments and files
if [ "$1" = "" ]; then echo "ERROR: Give an 'sql-file' as 1st argument"; usage; fi
sqlfile="$1"
if [ ! -f "$sqlfile" ]; then echo "ERROR: '$sqlfile' is not a file"; usage; fi
if [ ! -s "$sqlfile" ]; then echo "ERROR: '$sqlfile' is an empty file"; usage; fi
if [ "$2" = "" ]; then echo "ERROR: Give a 'mysql-cfg-file' as 2nd argument"; usage; fi
cfgfile="$2"
if [ ! -f "$cfgfile" ]; then echo "ERROR: '$cfgfile' is not a file"; usage; fi
if [ ! -s "$cfgfile" ]; then echo "ERROR: '$cfgfile' is an empty file"; usage; fi

# extract some info from cfgfile + define tmperrfile
dbhost=$(eval echo $(grep -E "^host *=" $cfgfile|cut -d'=' -f2-))
dbname=$(eval echo $(grep -E "^database *=" $cfgfile|cut -d'=' -f2-))
timestamp=$(date +%F_%Hu%M)
tmperrfile="/tmp/mysql.err.$timestamp"

# create $tmpcfgfile: a copy of $cfgfile in /tmp/
# this fixes the "[Warning] World-writable config file is ignored" message, if $cfgfile is on an NTFS (eg. in case of WSL)
# we also remove the 'database=...' line, to prevent the "[ERROR] unknown variable 'database=...'" message
tmpcfgfile="/tmp/mysql.cfg.$timestamp"
grep -vP '^ *database *=' $cfgfile >$tmpcfgfile

# create an empty DB + additional check to prevent overwriting by accident
mysql --defaults-extra-file=$tmpcfgfile -e "CREATE DATABASE $dbname" 2>$tmperrfile
exitcode=$?
if grep -q "ERROR 1007 (HY000) at line 1: Can't create database '.*'; database exists" $tmperrfile; then
    read -p "WARNING: The database '$dbname' already exists (on $dbhost)! Overwrite? [y/N] " ans
    if [ "$ans" = "" ] || [ "${ans^}" = "N" ]; then cleanup_and_abort; fi
    mysql --defaults-extra-file=$tmpcfgfile -e "DROP DATABASE $dbname; CREATE DATABASE $dbname"
    exitcode=$?
    if (( $exitcode != 0 )); then cleanup_and_abort; fi
elif (( $exitcode != 0 )) || [ -s "$tmperrfile" ]; then
    cat $tmperrfile
    cleanup_and_abort
fi

# do the actual restore
echo "Restoring '$sqlfile' to '$dbname' (on $dbhost) ..."
mysql --defaults-extra-file=$tmpcfgfile $dbname < $sqlfile
exitcode=$?
if (( $exitcode != 0 )); then cleanup_and_abort; fi

echo "Done!"
echo
