# About

[![Build](https://github.com/rgl/opentelemetry-dotnet-playground/actions/workflows/build.yml/badge.svg)](https://github.com/rgl/opentelemetry-dotnet-playground/actions/workflows/build.yml)

This is a [OpenTelemetry .NET](https://github.com/open-telemetry/opentelemetry-dotnet) playground.

The following components are used:

![components](components.png)

# Usage (Ubuntu 22.04)

```bash
# create the environment defined in docker-compose.yml
# and leave it running in the background.
docker compose up --detach --build --wait

# show running containers.
docker compose ps

# show logs.
docker compose logs

# show the quotes container health check output log.
docker inspect --format '{{json .State.Health }}' \
  "$(docker compose ps quotes --format '{{.Name}}')" \
  | jq -r '.Log[].Output'

# open a container network interface in wireshark.
./wireshark.sh quotes

# open the quotes service swagger.
xdg-open http://localhost:8000/swagger

# make a request.
http \
  --verbose \
  http://localhost:8000/quote

# make a failing request.
http \
  --verbose \
  http://localhost:8000/quote?opsi=opsi

# make a request that includes a parent trace.
# NB the dotnet trace id will be set to the traceparent trace id.
# NB the tracestate does not seem to be stored or propagated anywhere.
# NB traceparent syntax: <version>-<trace-id>-<parent-id-aka-span-id>-<trace-flags>
http \
  --verbose \
  http://localhost:8000/quote \
  traceparent:00-10000000000000000000000000000000-1000000000000000-01 \
  tracestate:x.client.state=example

# make a request to quotetext, which in turn, makes a nested request to quote.
http \
  --verbose \
  http://localhost:8000/quotetext

# make a failing request to quotetext, which in turn, makes a nested failing
# request to quote.
http \
  --verbose \
  http://localhost:8000/quotetext?opsi=opsi

# make a request that includes a parent trace.
# NB the dotnet trace id will be set to the traceparent trace id.
# NB the tracestate does not seem to be stored or propagated anywhere.
# NB traceparent syntax: <version>-<trace-id>-<parent-id-aka-span-id>-<trace-flags>
http \
  --verbose \
  http://localhost:8000/quotetext \
  traceparent:00-20000000000000000000000000000000-1000000000000000-01 \
  tracestate:x.client.state=example

# open aspire dashboard (metrics/traces/logs).
xdg-open http://localhost:18888

# destroy the environment.
docker compose down --remove-orphans --volumes --timeout=0
```

List this repository dependencies (and which have newer versions):

```bash
GITHUB_COM_TOKEN='YOUR_GITHUB_PERSONAL_TOKEN' ./renovate.sh
```

# Notes

* .NET uses the [Activity class](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.activity?view=net-8.0) to encapsulate the [W3C Trace Context](https://www.w3.org/TR/trace-context/).
  * The Activity `Id` property contains to the [W3C `traceparent` header value](https://www.w3.org/TR/trace-context/#traceparent-header).
    * It looks alike `00-98d483b6d0e3a6d012b11e23737faa50-6ac18089ab13c12e-01`.
    * It has four fields: `version`, `trace-id`, `parent-id` (aka `span-id`), and `trace-flags`.

# Reference

* [W3C Trace Context](https://www.w3.org/TR/trace-context/).
* [OpenTelemetry Concepts](https://opentelemetry.io/docs/concepts/).
* [OpenTelemetry Semantic Conventions Specification](https://opentelemetry.io/docs/specs/semconv/).
* [OpenTelemetry Specifications](https://opentelemetry.io/docs/specs/status/).
* [opentelemetry-dotnet repository](https://github.com/open-telemetry/opentelemetry-dotnet).
* [OpenTelemetry in .NET documentation](https://opentelemetry.io/docs/languages/net/).
* [OpenTelemetry.Exporter.OpenTelemetryProtocol Environment Variables](https://github.com/open-telemetry/opentelemetry-dotnet/tree/main/src/OpenTelemetry.Exporter.OpenTelemetryProtocol#environment-variables).
