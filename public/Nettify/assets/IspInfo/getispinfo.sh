#!/bin/bash

# Some variables
DATABASEADDRESS=https://autoconfig.thunderbird.net/v1.1/
DBFILE=$(mktemp)
ROOT=$(dirname "$0")
GREEN=$(tput setaf 2)
RED=$(tput setaf 1)
RESET=$(tput sgr0)

# Download and parse the ISP list
printf "Downloading the ISP list...\n"
curl -s -o $DBFILE $DATABASEADDRESS
if [ $? -ne 0 ]
then
    ERROR=$?
    printf "${RED}Failed to download the ISP list.$RESET\n"
    exit $ERROR
fi
ISPS=($(cat $DBFILE | grep -oE 'href=".+?"' | cut -b 6- | sed 's/^"\([^"]*\).*/\1/' | tail -n +3))

# Form URLs and download their information one by one here
ISPCOUNT=${#ISPS[@]}
rm -f $ROOT/isps.txt
for ISPIDX in "${!ISPS[@]}"
do
    ISP=${ISPS[ISPIDX]}
    ISPADDRESS=$DATABASEADDRESS$ISP
    OUTPUTFILE=$ROOT/$ISP.xml

    # Download configuration information
    printf "[$((ISPIDX+1)) of $ISPCOUNT] Downloading $GREEN$ISPADDRESS$RESET...\n"
    curl -s -o $OUTPUTFILE $ISPADDRESS
    if [ $? -ne 0 ]
    then
        ERROR=$?
        printf "${RED}Failed to download the ISP configuration information.$RESET\n"
        exit $ERROR
    fi

    # Optimize the XML for .NET's strict XML parser
    sed -i '1 i\<?xml version=\"1.0\" encoding=\"UTF-8\"?>' $OUTPUTFILE

    # Write ISP name to file
    printf "${ISP}\n" >> $ROOT/isps.txt
done

# Done. User can now build Nettify.
printf "${GREEN}Done!$RESET\n"
