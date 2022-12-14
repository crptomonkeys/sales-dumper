# AtomicHub Sales dumper

Dumps all actual and historical sales from AtomicHub into CSV file.

⚠ AtomicHub throttles API requests to 90 per minute (per IP). This tool "sleeps" automatically when needed, but your other AtomicHub activites (other tools, regular browsing) may be affected.

⚠ AtomicHub allows to sell several assets in one sale offer. This tool makes one line per each asset (repeating sale field values: Id, Price etc), so NEVER make Sum(Price) over original file - remove duplicates by SaleId field first.

## Requirements

* .NET SDK (v6.0 and later) from https://dotnet.microsoft.com/en-us/download

## Configuration

Not required.

## Running

1. Clone this repo
2. Open command prompt (`cmd.exe`) or shell
3. Type `dotnet run` and press `Enter`

Output like this means everything works.
![Sample output](sample.png)

At time of writing this tool (September 2002, ~190 templates) full download takes about 20 minutes.

Data are written into `crptomonkeys-sales.csv`.