using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetWatch
{ 
    public static class PortfolioTileHelpers
    {
        public static double CalculateInvest(List<AssetTileData> assetTileDataSet)
        {
            double investTotal = 0;

            assetTileDataSet.ForEach(asstiledata =>
            {
                investTotal += asstiledata.InvestedSum;
            });

            return investTotal;
        }

        public static double CalculateWorth(List<AssetTileData> assetTilesDataSet)
        {
            double worthTotal = 0;

            assetTilesDataSet.ForEach(asstiledata =>
            {
                worthTotal += asstiledata.HoldingsCount * double.Parse(asstiledata.Asset.PriceConvert);
            });

            return worthTotal;
        }

        public static double Calculate24hPercentage(List<AssetTileData> assetTilesDataSet, double worthTotal)
        {
            double percentage = 0;

            assetTilesDataSet.ForEach(ass => {
                double worth = ass.HoldingsCount * double.Parse(ass.Asset.PriceConvert);
                double weight = worth / worthTotal;
                percentage += double.Parse(ass.Asset.PercentChange24h) * weight;
            });

            return Math.Round(percentage, 2);
        }

        public static double Calculate1WPercentage(List<AssetTileData> assetTilesDataSet, double worthTotal)
        {
            double percentage = 0;

            assetTilesDataSet.ForEach(ass => {
                double worth = ass.HoldingsCount * double.Parse(ass.Asset.PriceConvert);
                double weight = worth / worthTotal;
                percentage += double.Parse(ass.Asset.PercentChange7d) * weight;
            });

            return Math.Round(percentage, 2);
        }

        public static double CalculateWinLoss(double percentage, double worthTotal)
        {
            double mult = percentage / 100;
            return worthTotal * mult;
        }
    }

    public static class TileHelpers
    {
        public static string ConvertToValueString(double value)
        {
            if (Math.Abs(value) < 10)
            {
                return string.Format("{0:F4}", value);
            }
            if (Math.Abs(value) < 1)
            {
                return string.Format("{0:F5}", value);
            }
            if (Math.Abs(value) < 0.1)
            {
                return string.Format("{0:F6}", value);
            }

            return string.Format("{0:F2}", value);
        }
    }
}
