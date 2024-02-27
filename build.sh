#!/bin/bash
set -euxo pipefail

GITHUB_REPOSITORY="${GITHUB_REPOSITORY:-rgl/opentelemetry-dotnet-playground}"

QUOTES_SOURCE_URL="https://github.com/$GITHUB_REPOSITORY"
if [[ "${GITHUB_REF:-v0.0.0-dev}" =~ \/v([0-9]+(\.[0-9]+)+(-.+)?) ]]; then
  QUOTES_VERSION="${BASH_REMATCH[1]}"
else
  QUOTES_VERSION='0.0.0-dev'
fi
QUOTES_REVISION="${GITHUB_SHA:-0000000000000000000000000000000000000000}"
QUOTES_TITLE="$(basename "$GITHUB_REPOSITORY")"
QUOTES_DESCRIPTION="$(perl -ne 'print $1 if /\<Description\>(.+)\<\/Description\>/' <quotes/Quotes.csproj)"
QUOTES_LICENSE='ISC'
QUOTES_AUTHOR_NAME="$(perl -ne 'print $1 if /\<Company\>(.+)\<\/Company\>/' <quotes/Quotes.csproj)"
QUOTES_VENDOR="$QUOTES_AUTHOR_NAME"

function set-metadata {
  sed -i -E "s,(\<Version\>).+(\</Version\>),\1$QUOTES_VERSION\2,g" quotes/Quotes.csproj
}

function build {
  set-metadata
  docker compose build
}

function release {
  set-metadata
  pushd quotes
  local image="ghcr.io/$GITHUB_REPOSITORY:$QUOTES_VERSION"
  local image_created="$(date --utc '+%Y-%m-%dT%H:%M:%S.%NZ')"
  docker build \
    --label "org.opencontainers.image.created=$image_created" \
    --label "org.opencontainers.image.source=$QUOTES_SOURCE_URL" \
    --label "org.opencontainers.image.version=$QUOTES_VERSION" \
    --label "org.opencontainers.image.revision=$QUOTES_REVISION" \
    --label "org.opencontainers.image.title=$QUOTES_TITLE" \
    --label "org.opencontainers.image.description=$QUOTES_DESCRIPTION" \
    --label "org.opencontainers.image.licenses=$QUOTES_LICENSE" \
    --label "org.opencontainers.image.vendor=$QUOTES_VENDOR" \
    --label "org.opencontainers.image.authors=$QUOTES_AUTHOR_NAME" \
    -t "$image" \
    .
  docker push "$image"
  popd
  install -d dist
  cat >dist/release-notes.md <<EOF
# Container image

[$QUOTES_SOURCE_URL/pkgs/container/$QUOTES_TITLE]($QUOTES_SOURCE_URL/pkgs/container/$QUOTES_TITLE)

\`\`\`bash
docker pull $image
\`\`\`
EOF
  echo "$image" 
}

function main {
  local command="$1"; shift
  case "$command" in
    build)
      build "$@"
      ;;
    release)
      release "$@"
      ;;
    *)
      echo "ERROR: Unknown command $command"
      exit 1
      ;;
  esac
}

main "$@"
