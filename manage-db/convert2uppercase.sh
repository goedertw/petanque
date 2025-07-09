if [ $# -eq 0 ]; then echo "Give .sql-file as argument"; exit; fi
if [ ! -f $1 ]; then echo "ERROR: '$1' is not a file"; exit; fi

for i in Aanwezigheid Dagklassement Seizoen Seizoensklassement Speeldag Spel Speler Spelverdeling;
do
       sed -E "s/\`$i\`/\`$i\`/i" -i $1
done
