# ITA-music-repo

## Opis sistema

Ta projekt predstavlja spletni sistem za odkrivanje in organiziranje glasbe. Uporabnikom omogoča iskanje pesmi, ustvarjanje lastnih seznamov predvajanja (playlists).

### Osnovna funkcionalnost

Glavna funkcionalnost sistema je pomagati uporabnikom odkrivati novo glasbo in organizirati svoje najljubše pesmi.

### Uporabniki

Glavni uporabniki sistema so:

ljubitelji glasbe, ki želijo organizirati svojo glasbeno zbirko.

### Komunikacija komponent

Sistem je sestavljen iz več komponent:

Frontend: uporabniški vmesnik, kjer uporabnik išče glasbo in upravlja playliste.

Backend: obdeluje zahteve uporabnikov, upravlja uporabniške podatke in generira priporočila.

Baza podatkov: shranjuje informacije o uporabnikih, pesmih, izvajalcih in playlistah.


## Mikrostoritve sistema

Sistem je razdeljen na tri glavne mikrostoritve, kjer vsaka pokriva določeno poslovno domeno.

### 1. Storitev za upravljanje uporabnikov

Ta mikrostoritev skrbi za vse podatke in funkcionalnosti, povezane z uporabniki sistema.

### 2. Storitev za upravljanje glasbe

Ta mikrostoritev upravlja glasbeno knjižnico sistema.

### 3. Storitev za playliste

Ta mikrostoritev omogoča uporabnikom organizacijo glasbe v sezname predvajanja.

## Arhitektura sistema

Sistem uporablja arhitekturo mikrostoritev. Uporabniški vmesnik komunicira z mikrostoritvami preko REST API klicev. Vsaka mikrostoritev ima svojo bazo podatkov, kar omogoča neodvisnost storitev.

### Način komunikacije

Komunikacija med Frontend in mikrostoritvami poteka preko REST API in gRPC.

Vsaka mikrostoritev ima svojo bazo podatkov.

Mikrostoritve lahko med seboj komunicirajo preko REST API.

<img width="587" height="512" alt="image" src="https://github.com/user-attachments/assets/113d7173-d129-491b-bf0b-007c5aeecd03" />

## Status implementacije

- `music-service` (mikrostoritev za upravljanje glasbe) je implementiran kot ASP.NET Core Web API.
- Vključuje PostgreSQL podatkovno bazo, Swagger/OpenAPI, logiranje, teste repozitorija in teste vseh API končnih točk.
- Docker podpora je na voljo preko `music-service/WorkerService1/WorkerService1/Dockerfile` in `music-service/docker-compose.yml`.
- Podrobnejša navodila za zagon so v `music-service/README.md`.


