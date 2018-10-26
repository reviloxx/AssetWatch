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
                if (asstiledata.Asset.PriceConvert != null)
                {
                    worthTotal += asstiledata.HoldingsCount * double.Parse(asstiledata.Asset.PriceConvert);
                }                
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
        public static string FormatValueString(double value, bool forceSign)
        {
            string sign = value > 0 ? "+" : string.Empty;
            string valueString;

            if (Math.Abs(value) < 10)
            {
                valueString = string.Format("{0:F4}", value);
            }
            else if (Math.Abs(value) < 1)
            {
                valueString = string.Format("{0:F5}", value);
            }
            else if (Math.Abs(value) < 0.1)
            {
                valueString = string.Format("{0:F6}", value);
            }
            else
            {
                valueString = string.Format("{0:N}", value);
            }

            if (forceSign)
            {
                valueString = valueString.Insert(0, sign);
            }

            valueString = valueString.TrimEnd('0').TrimEnd(',');

            return valueString;
        }

        public static string FormatValueString(string value, bool forceSign)
        {
            double val;

            if (!double.TryParse(value, out val))
            {
                return string.Empty;
            }

            string sign = val > 0 ? "+" : string.Empty;
            string valueString;

            if (Math.Abs(val) < 10)
            {
                valueString = string.Format("{0:F4}", val);
            }
            else if (Math.Abs(val) < 1)
            {
                valueString = string.Format("{0:F5}", val);
            }
            else if (Math.Abs(val) < 0.1)
            {
                valueString = string.Format("{0:F6}", val);
            }
            else
            {
                valueString = string.Format("{0:N}", val);
            }

            if (forceSign)
            {
                valueString = valueString.Insert(0, sign);
            }

            valueString = valueString.TrimEnd('0').TrimEnd(',');

            return valueString;
        }
    }
}
