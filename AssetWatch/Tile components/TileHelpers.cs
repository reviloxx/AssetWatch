using System;
using System.Collections.Generic;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="TileHelpers" />
    /// </summary>
    public static class TileHelpers
    {
        /// <summary>
        /// The GetValueString
        /// </summary>
        /// <param name="value">The value<see cref="double"/></param>
        /// <param name="forceSign">The forceSign<see cref="bool"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetValueString(double value, bool forceSign)
        {
            string sign = value > 0 ? "+" : string.Empty;
            string valueString;

            if (Math.Abs(value) < 0.01)
            {
                valueString = string.Format("{0:F6}", value);
            }
            else if (Math.Abs(value) < 0.1)
            {
                valueString = string.Format("{0:F5}", value);
            }
            else if (Math.Abs(value) < 1)
            {
                valueString = string.Format("{0:F4}", value);
            }
            else if (Math.Abs(value) < 10)
            {
                valueString = string.Format("{0:F3}", value);
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
                
        /// <summary>
        /// The CalculateInvest
        /// </summary>
        /// <param name="assetTileDataSet">The assetTileDataSet<see cref="List{AssetTileData}"/></param>
        /// <returns>The <see cref="double"/></returns>
        public static double CalculateInvest(List<AssetTileData> assetTileDataSet)
        {
            double investTotal = 0;

            assetTileDataSet.ForEach(asstiledata =>
            {
                investTotal += asstiledata.InvestedSum;
            });

            return investTotal;
        }

        /// <summary>
        /// The CalculateWorth
        /// </summary>
        /// <param name="assetTilesDataSet">The assetTilesDataSet<see cref="List{AssetTileData}"/></param>
        /// <returns>The <see cref="double"/></returns>
        public static double CalculateWorth(List<AssetTileData> assetTilesDataSet)
        {
            double worthTotal = 0;

            assetTilesDataSet.ForEach(asstiledata =>
            {
                worthTotal += asstiledata.HoldingsCount * asstiledata.Asset.Price;
            });

            return worthTotal;
        }

        /// <summary>
        /// The Calculate24hPercentage
        /// </summary>
        /// <param name="assetTilesDataSet">The assetTilesDataSet<see cref="List{AssetTileData}"/></param>
        /// <param name="worthTotal">The worthTotal<see cref="double"/></param>
        /// <returns>The <see cref="double"/></returns>
        public static bool Calculate24hPercentage(List<AssetTileData> assetTilesDataSet, double worthTotal, out double pctChange)
        {
            double tempPctChange = 0;
            bool calculationValid = true;

            assetTilesDataSet.ForEach(ass =>
            {
                if (ass.Asset.PercentChange24h < -100 || worthTotal <= 0)
                {
                    calculationValid = false;
                }

                double worth = ass.HoldingsCount * ass.Asset.Price;
                double weight = worth / worthTotal;
                tempPctChange += ass.Asset.PercentChange24h * weight;
            });

            pctChange = tempPctChange;
            return calculationValid;
        }

        /// <summary>
        /// The Calculate7dPercentage
        /// </summary>
        /// <param name="assetTilesDataSet">The assetTilesDataSet<see cref="List{AssetTileData}"/></param>
        /// <param name="worthTotal">The worthTotal<see cref="double"/></param>
        /// <returns>The <see cref="double"/></returns>
        public static bool Calculate7dPercentage(List<AssetTileData> assetTilesDataSet, double worthTotal, out double pctChange)
        {
            double tempPctChange = 0;
            bool calculationValid = true;

            assetTilesDataSet.ForEach(ass =>
            {
                if (ass.Asset.PercentChange7d < -100)
                {
                    calculationValid = false;
                }

                double worth = ass.HoldingsCount * ass.Asset.Price;
                double weight = worth / worthTotal;
                tempPctChange += ass.Asset.PercentChange7d * weight;
            });

            pctChange = tempPctChange;
            return calculationValid;
        }

        /// <summary>
        /// The CalculateWinLoss
        /// </summary>
        /// <param name="percentage">The percentage<see cref="double"/></param>
        /// <param name="worthTotal">The worthTotal<see cref="double"/></param>
        /// <returns>The <see cref="double"/></returns>
        public static double CalculateWinLoss(double percentage, double worthTotal)
        {
            double mult = percentage / 100;
            return worthTotal * mult;
        }
    }
}
