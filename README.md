# QuickGrid URL state

Blazor Web App sample for URL-based sorting and paging with QuickGrid on .NET 11. Static SSR pages keep sort, direction, and page in the query string.

Tested with the .NET SDK `11.0.100-rc.1.26425.128` and `Microsoft.AspNetCore.Components.QuickGrid` `11.0.0-rc.1.26425.128`.

## Run

```bash
dotnet run --project src/QuickGridUrlState
```

The launch profile listens on `http://localhost:5110`.

| Page | What it shows |
|---|---|
| `/people` | Static SSR grid. Sort and page live in the URL. |
| `/people?country=Germany&sort=Age&direction=asc&page=2` | An app-owned filter parameter next to QuickGrid parameters. |
| `/multi` | Two grids. The second uses the `c_` query-parameter prefix. |
| `/provider` | `GridItemsProvider` paging. Each call is written to the log. |
| `/people-interactive` | The same URL contract on an interactive server page. |

Set `QUICKGRID_DISABLE_URLNAV=1` before `dotnet run` to render the .NET 10-style `<button>` controls. The URL is still read.

## Test

```bash
dotnet test
```
