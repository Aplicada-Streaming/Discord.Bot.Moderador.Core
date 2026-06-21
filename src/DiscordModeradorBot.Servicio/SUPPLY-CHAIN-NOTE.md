# Nota de cadena de suministro (supply-chain)

## NU1903 — SQLitePCLRaw.lib.e_sqlite3 2.1.11 (GHSA-2m69-gcr7-jv3q)

### Contexto

El paquete `Microsoft.EntityFrameworkCore.Sqlite` 10.0.9 arrastra de forma
transitiva `SQLitePCLRaw.lib.e_sqlite3` 2.1.11, que empaqueta una versión de la
biblioteca nativa SQLite afectada por la vulnerabilidad de gravedad **alta**
[GHSA-2m69-gcr7-jv3q](https://github.com/advisories/GHSA-2m69-gcr7-jv3q). El
quality gate del intake (§17 P.8) exige **0 vulnerabilidades altas** para mergear.

### Resolución aplicada (preferida: actualización, no excepción)

Se actualizó la dependencia a una versión parcheada agregando referencias
**directas** en `DiscordModeradorBot.Servicio.csproj`:

```xml
<PackageReference Include="SQLitePCLRaw.bundle_e_sqlite3" Version="3.0.3" />
<PackageReference Include="SQLitePCLRaw.lib.e_sqlite3" Version="3.50.3" />
```

La línea 3.x de SQLitePCLRaw empaqueta SQLite 3.50.x, donde el fallo está
corregido. Al ser referencias directas, tienen prioridad sobre la versión
transitiva 2.1.11 traída por EF Core, eliminando el aviso `NU1903`.

### Verificación

- `dotnet build -c Debug` compila con **0 advertencias** de vulnerabilidad.
- `dotnet list package --vulnerable --include-transitive` no reporta paquetes
  con vulnerabilidad alta.

### Seguimiento

No se requiere excepción a la DoD: la vulnerabilidad se resolvió por
actualización. Cuando una versión futura de EF Core actualice su dependencia
transitiva a la línea 3.x, estas referencias directas podrán retirarse.
