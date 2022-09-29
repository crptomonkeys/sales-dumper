using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AtomicAssetsClient;
using Microsoft.Extensions.Logging;

namespace SalesDumper
{
    public class DumpService
    {
        private const string CollectionName = "crptomonkeys";

        private readonly ILogger logger;
        private readonly IAtomicClient atomicClient;

        public DumpService(ILogger<DumpService> logger, IAtomicClient atomicClient)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.atomicClient = atomicClient ?? throw new ArgumentNullException(nameof(atomicClient));
        }

        public async Task Run()
        {
            var timer = Stopwatch.StartNew();

            var templates = await atomicClient.GetTemplates(collectionName: CollectionName).ConfigureAwait(false);

            logger.LogInformation("Found {Count} templates in {Collection}, {Interval} elapsed", templates.Count, CollectionName, timer.Elapsed.Truncate(TimeSpan.FromSeconds(1)));

            var fileName = Path.GetFullPath("crptomonkeys-sales.csv");
            using var file = File.OpenWrite(fileName);
            using var textWriter = new StreamWriter(file);
            using var csvWriter = new CsvHelper.CsvWriter(textWriter, CultureInfo.InvariantCulture);

            var templatesToGo = templates.Count;
            var counter = 0;

            foreach (var template in templates)
            {
                var sales = await atomicClient.GetSales(templateId: template.TemplateId).ConfigureAwait(false);

                var data = sales
                    .Where(x => x.Assets != null)
                    .SelectMany(x => x.Assets!.Select(z => new { Sale = x, Asset = z }))
                    .Select(x => new
                    {
                        x.Sale.SaleId,
                        x.Sale.Seller,
                        x.Sale.CreatedAtTime,
                        x.Sale.State,
                        PriceWax = Math.Round(x.Sale.Price.Amount / (decimal)Math.Pow(10, x.Sale.Price.TokenPrecision), 2),
                        Buyer = x.Sale.Buyer ?? default,
                        x.Asset.AssetId,
                        x.Asset.Schema?.SchemaName,
                        x.Asset.Template?.TemplateId,
                        x.Asset.Name,
                        x.Asset.TemplateMint,
                    })
                    .ToList();

                await csvWriter.WriteRecordsAsync(data).ConfigureAwait(false);

                counter += sales.Count;
                templatesToGo--;

                logger.LogInformation(
                    "Dumped {Count} sales for template {Id} ({Schema} / {Name}), {Interval} elapsed, {Count} templates left",
                    sales.Count, template.TemplateId,
                    template.Schema?.SchemaName,
                    template.Name,
                    timer.Elapsed.Truncate(TimeSpan.FromSeconds(1)),
                    templatesToGo);
            }

            logger.LogInformation(
                "Dumped {Count} sales into {FileName}, {Elapsed} elapsed",
                counter,
                fileName,
                timer.Elapsed.Truncate(TimeSpan.FromSeconds(1)));
        }
    }
}
