#!/bin/bash

dumpdir="./sqldumps"
debug=0

usage () {
    echo 
    echo "Usage: ./backup.sh <mysql-cfg-file>"
    echo
    exit 1
}

# check argument and config file
if [ "$1" = "" ]; then echo "ERROR: Give a 'mysql-cfg-file' as argument"; usage; fi
cfgfile="$1"
if [ ! -f "$cfgfile" ]; then echo "ERROR: '$cfgfile' is not a file"; usage; fi

# create name of dumpfile, based on time and hostname/dbname in $cfgfile
timestamp=$(date +%F_%Hu%M)
dbhost=$(eval echo $(grep -E "^host *=" $cfgfile|cut -d'=' -f2-))
dbname=$(eval echo $(grep -E "^database *=" $cfgfile|cut -d'=' -f2-))
dumpfile="$dumpdir/$timestamp-$dbhost-$dbname.sql"

# create $tmpcfgfile: a copy of $cfgfile in /tmp/
# this fixes the "[Warning] World-writable config file is ignored" message, if $cfgfile is on an NTFS (eg. in case of WSL)
# we also remove the 'database=...' line, to prevent the "[ERROR] unknown variable 'database=...'" message
tmpcfgfile="/tmp/mysql.cfg.$timestamp"
grep -vP '^ *database *=' $cfgfile >$tmpcfgfile
   
# do the backup
cd "$(dirname $0)"
mkdir -p $dumpdir
echo "Dumping database to '$dumpfile' ..."
mysqldump --defaults-extra-file=$tmpcfgfile --column-statistics=0 --no-tablespaces $dbname --result-file=$dumpfile
exitcode=$?
if (( $debug == 1 )); then echo exitcode=$exitcode; fi
rm $tmpcfgfile

if (( $exitcode != 0 )); then
    if (( $debug != 1 )); then rm -f ${dumpfile}; fi
    echo "---"
    echo "ERROR: dump failed!"; echo; exit $exitcode
fi
echo "Done!"
echo
