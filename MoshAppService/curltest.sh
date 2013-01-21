#!/bin/bash

# Note: Do not edit this file in Visual Studio; it seems to like 
# messing with the file encoding, rendering this script unusable.
# Instead use Notepad++, vim, nano, or basically any other text editor.

# Functions
function prompt {
  printf "\n\n-----\nPress <ENTER> to continue.\n-----\n"
  read
}
function clear {
  # Use Windows' clear screen command :p
  cmd /c cls
}
function entertocontinue {
  prompt
  clear
}
function die {
  echo $1
  exit 1
}
function write {
  printf "\n$1\n\n"
}

clear

# First check if the required applications are installed
which curl > /dev/null 2>&1
if [ $? -ne 0 ]; then
  die "curl is not installed."
fi

which cmd > /dev/null 2>&1
if [ $? -ne 0 ]; then
  die "cmd is not installed. This script should really be run on Windows+Cygwin :p"
fi

which awk > /dev/null 2>&1
if [ $? -ne 0 ]; then
  die "awk is not installed."
fi

# Set to 1 to do initial test
RUNTEST=0

# Service hostname and port
HOST="http://localhost:9000"

# Curl options
CURL="curl -v"

JSON_HEADER="Content-Type: application/json"
FORM_HEADER="Content-Type: application/x-www-form-urlencoded"

POST="-X POST"
GET="-X GET"
PUT="-X PUT"
DELETE="-X DELETE"

USER_JSON='-d {"userName":"tkim","password":"123456"}'
USER_FORM='-d userName=tkim -d password=123456'

if [ $RUNTEST -eq 1 ]; then
  # Login with JSON
  write "Login using application/JSON"
  $CURL $POST $HOST/authenticate $USER_JSON -H "$JSON_HEADER"

  entertocontinue

  # Login with "web form"
  write "Login using application/x-www-form-urlencoded"
  $CURL $POST $HOST/authenticate $USER_FORM -H "$FORM_HEADER"

  # Exit if login wasn't working properly
  if [ $? -ne 0 ]; then
    die "There was a problem logging in."
  fi

  entertocontinue
fi

write "Now logging in and using the returned session ID for the other features..."

# Now that we know that we can login, let's use the session ID it gives us to test the other features
sessionId=$(curl $POST $HOST/authenticate $USER_JSON -H "$JSON_HEADER" | awk -F '"' '{print $4}')

if [ $sessionId == "errorCode" ]; then
  die "Error logging in."
fi

write "Session ID is "$sessionId""

CURL=curl
AUTH="-b ss-id=$sessionId"

write "Retrieve user information: $HOST/users/0"
$CURL $GET $HOST/users/0 $AUTH

entertocontinue

write "Retrieve team information: $HOST/teams/0"
$CURL $GET $HOST/teams/0 $AUTH

entertocontinue

write "Retrieve team member information: $HOST/users/1"
$CURL $GET $HOST/users/1 $AUTH

entertocontinue

write "Try to retrieve information for user not in team: $HOST/users/5"
$CURL -v $GET $HOST/users/5 $AUTH

entertocontinue

write "Retrieve game information: $HOST/games/0"
$CURL $GET $HOST/games/0 $AUTH

entertocontinue

write "Retrieve task information: $HOST/tasks/0"
$CURL $GET $HOST/tasks/0 $AUTH
