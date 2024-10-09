#!/usr/bin/env bash
#
# run locally
#

set -o errexit
set -o pipefail
set -o nounset

ENV_FILE="${1:-./.env}"
if [ -f "${ENV_FILE}" ]; then
    echo "INFO: loading '${ENV_FILE}'!"
    export $(cat "${ENV_FILE}")
fi

# run the app
dotnet run .