# Pokédex API

A small REST API that returns Pokémon information, with a "fun translation" of the
description. Built with **.NET 10 Minimal API**.

It uses two public APIs to do the heavy lifting:

- [PokéAPI](https://pokeapi.co/) — Pokémon data (`pokemon-species`).
- FunTranslations (the challenge-provided mirror, `https://api.funtranslations.mercxry.me`) — Yoda / Shakespeare translations.

## Endpoints

### `GET /pokemon/{name}`

Standard Pokémon information.

```bash
curl http://localhost:5108/pokemon/mewtwo
```

```json
{
  "name": "mewtwo",
  "description": "It was created by a scientist after years of horrific gene splicing and DNA engineering experiments.",
  "habitat": "rare",
  "isLegendary": true
}
```

### `GET /pokemon/translated/{name}`

Same information, but with a translated description:

- **Yoda** if the habitat is `cave` **or** the Pokémon is legendary;
- **Shakespeare** otherwise.

```bash
curl http://localhost:5108/pokemon/translated/mewtwo
```

```json
{
  "name": "mewtwo",
  "description": "Created by a scientist after years of horrific gene splicing and dna engineering experiments, it was.",
  "habitat": "rare",
  "isLegendary": true
}
```

**Graceful degradation:** the translation API is rate-limited (5 requests/minute on the
free tier). If the translation can't be obtained — for *any* reason (rate limit, timeout,
malformed response) — the endpoint still returns `200` with the **standard** description.
The translation is a best-effort enhancement, never a hard dependency.

### Status codes

| Situation | Status |
|---|---|
| Found | `200 OK` |
| Pokémon does not exist | `404 Not Found` |
| Upstream (PokéAPI) failure | `502 Bad Gateway` |
| Translation unavailable | `200 OK` (standard description) |

## Prerequisites

You need **[git](https://git-scm.com/downloads)** to clone the repository, plus **one** of:

- **[Docker](https://docs.docker.com/get-docker/)** — run the app with nothing else installed (recommended); or
- **[.NET SDK 10.0](https://dotnet.microsoft.com/download)** — run the app directly, and run the test suite.

## Running the app

Clone the repository first:

```bash
git clone <repository-url>
cd Pokedex
```

### With Docker (only Docker required)

The .NET runtime lives inside the image, so nothing else needs to be installed.

```bash
docker build -t pokedex .
docker run -p 8080:8080 pokedex
```

The API is now available on `http://localhost:8080`:

```bash
curl http://localhost:8080/pokemon/mewtwo
```

### Without Docker (.NET SDK)

```bash
dotnet run --project src/Pokedex.Api --launch-profile http
```

The API listens on `http://localhost:5108` (from `launchSettings.json`):

```bash
curl http://localhost:5108/pokemon/mewtwo
```

## Running the tests

Tests require the **.NET SDK** — they are not part of the Docker image (a production image
should not ship its tests). The suite is split into fast **deterministic** tests and a couple
of opt-in **live** smoke tests:

```bash
# Deterministic suite — unit + in-process E2E (network faked). The CI default.
dotnet test --filter "Category!=Live"

# Live smoke tests — hit the REAL PokéAPI and mirror. Opt-in, may be rate-limited.
dotnet test --filter "Category=Live"
```

The live tests are tagged `[Trait("Category", "Live")]` so a pipeline can toggle them on or
off. They assert **invariants** (status, response shape) rather than the volatile translated
text, which depends on whether the live translation succeeded.

## Design notes

- **Layering:** thin endpoint → `PokemonService` (orchestration + fallback) → typed
  `HttpClient` per external API (`PokemonApiClient`, `TranslationApiClient`). External JSON
  is mapped onto small domain types immediately; only the fields we consume are modelled.
- **Right-sized:** Minimal API, `record` domain models, the Options pattern, typed clients.
  No DDD/CQRS/MediatR — the task doesn't warrant them.
- **Translation choice** lives in a pure, unit-tested policy (`PokemonTranslationPolicy`);
  the translation client is *best-effort* (returns `string?`, `null` on any failure), and
  the service turns that `null` into the fallback. Failure handling is verified at both
  layers (client produces `null`, service consumes it).

## What I'd do differently for production

- **Centralized error handling:** an `IExceptionHandler` emitting RFC 7807 `ProblemDetails`
  instead of per-endpoint `try/catch`, as the number of endpoints grows.
- **Caching:** cache PokéAPI responses and translations (with a TTL). This cuts latency and,
  crucially, keeps the translation calls under the rate limit.
- **Resilience:** Polly policies (retry with backoff, circuit breaker) on the typed clients,
  honoring the `retry_after` the mirror returns on `429`.
- **Observability:** structured logging, distributed tracing and metrics around the external
  calls (especially translation success vs fallback rate).
- **Secrets/config:** if the paid translation tier were used, the API key would be injected
  from a secrets store (never committed); the client is already auth-ready in shape.
- **CI:** run the deterministic suite on every PR; run the `Live` smoke tests on a schedule
  (e.g. nightly), not on the PR gate, so flaky third parties never block a merge.
